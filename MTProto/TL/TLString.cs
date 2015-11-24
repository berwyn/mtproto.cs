using System;
using System.IO;
using System.Linq;
using System.Text;

namespace MTProto.TL
{
    public class TLString : TLObject
    {
        private string _value;

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                NotifyPropertyChanged(nameof(Value));
            }
        }

        public TLString() { }

        public TLString(string str)
        {
            Value = str;
        }

        public TLString(byte[] bytes, ref int position)
        {
            FromBytes(bytes, ref position);
        }

        public TLString(Stream input, ref int position)
        {
            FromStream(input, ref position);
        }

        public override TLObject FromBytes(byte[] bytes, ref int position)
        {
            int len = parseLen(ref position, buffer: bytes);
            var padding = 4 - (len % 4); // TLString has to be padded to a 4-byte boundary
            var stringBytes = new byte[len];

            Array.Copy(bytes, position, stringBytes, 0, len);
            position += len + padding;
            stringBytes.Reverse(); // Bytes come in as little-endian

            Value = Encoding.UTF8.GetString(stringBytes, 0, len);

            return this;
        }

        public override TLObject FromStream(Stream input, ref int position)
        {
            int len = parseLen(ref position, input: input);
            int padding = 4 - (len % 4); // TLString has to be padded to a 4-byte boundary
            var stringBytes = new byte[len];

            input.Read(stringBytes, 0, len);
            position += len + padding;
            stringBytes.Reverse();

            Value = Encoding.UTF8.GetString(stringBytes, 0, len);

            return this;
        }

        public override byte[] ToBytes()
        {
            byte[] output;
            byte[] bytes = Encoding.UTF8.GetBytes(Value);
            var byteCount = bytes.Length;
            var padding = 4 - (byteCount % 4);

            if (byteCount < 254)
            {
                output = new byte[1 + byteCount + padding];

                output[0] = (byte)byteCount;
                Array.Copy(bytes, 0, output, 1, byteCount);
            }
            else
            {
                output = new byte[4 + byteCount + padding];
                output[0] = 254; // When L >= 254, the first bit is constant
                byte[] lenBits = BitConverter.GetBytes(byteCount).Take(3).Reverse().ToArray(); // lenBits are little-endian

                Array.Copy(lenBits, 0, output, 1, lenBits.Length);
                Array.Copy(bytes, 0, output, 4, byteCount);
            }

            return output;
        }

        public override void ToStream(Stream input)
        {
            var bytes = ToBytes();
            input.Write(bytes, 0, bytes.Length);
        }

        private int parseLen(ref int position, Stream input = null, byte[] buffer = null)
        {
            if (input == null && buffer == null)
            {
                throw new ArgumentException("Must provide either input or buffer");
            }

            int len = input == null ? buffer[0] : input.ReadByte();
            position += 1;
            if (len >= 254)
            {
                byte[] lenBytes = new byte[4];
                if (input != null)
                {
                    input.Read(lenBytes, 1, 3);
                }
                else
                {
                    for (var i = 1; i <= 3; i++)
                    {
                        lenBytes[i] = buffer[i];
                    }
                }

                len = BitConverter.ToInt32(lenBytes.Reverse().ToArray(), 0);
                position += 3;
            }

            return len;
        }
    }
}
