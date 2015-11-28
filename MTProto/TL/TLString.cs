using System;
using System.Collections.Generic;
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
            if (padding == 4) padding = 0;
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
            if (padding == 4) padding = 0;
            var stringBytes = new byte[len];

            input.Read(stringBytes, 0, len);
            position += len + padding;
            input.Position += padding;
            stringBytes.Reverse(); // Bytes come in as little-endian

            Value = Encoding.UTF8.GetString(stringBytes, 0, len);

            return this;
        }

        public override byte[] ToBytes()
        {
            byte[] header;
            byte[] bytes = Encoding.UTF8.GetBytes(Value);
            var byteCount = bytes.Length;
            var padding = 4 - (byteCount % 4);

            if (byteCount < 254)
            {
                header = new byte[1];
                header[0] = (byte)byteCount;
            }
            else
            {
                header = BitConverter.GetBytes(byteCount).Reverse().ToArray(); // lenBits are little-endian
                header[0] = 254; // When L >= 254, the first bit is constant
            }

            var output = new List<byte[]> { header, bytes };
            if (padding > 0 && padding < 4)
                output.Add(new byte[padding]);

            return output.SelectMany(x => x).ToArray();
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

            int len = input == null ? buffer[position] : input.ReadByte();
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
                    for (var i = 0; i < 3; i++)
                    {
                        lenBytes[i + 1] = buffer[i + position];
                    }
                }

                len = BitConverter.ToInt32(lenBytes.Reverse().ToArray(), 0);
                position += 3;
            }

            return len;
        }
    }
}
