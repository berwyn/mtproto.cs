using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var padding = len % 4; // TLString has to be padded to a 4-byte boundary
            var stringBytes = new byte[len + padding];

            Array.Copy(bytes, position, stringBytes, 0, len + padding);
            position += stringBytes.Length;
            stringBytes.Reverse(); // Bytes come in as little-endian

            Value = Encoding.UTF8.GetString(stringBytes, padding, len);

            return this;
        }

        public override TLObject FromStream(Stream input, ref int position)
        {
            int len = parseLen(ref position, input: input);
            int padding = len % 4; // TLString has to be padded to a 4-byte boundary
            var stringBytes = new byte[len + padding];

            input.Read(stringBytes, 0, len + padding);
            position += stringBytes.Length;
            stringBytes.Reverse();

            Value = Encoding.UTF8.GetString(stringBytes, padding, len);

            return this;
        }

        public override byte[] ToBytes()
        {
            throw new NotImplementedException();
        }

        public override void ToStream(Stream input)
        {
            throw new NotImplementedException();
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
                byte[] lenBytes = { 0x0, 0x0, 0x0, 0x0 };
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

                len = BitConverter.ToInt32(lenBytes, 0);
                position += 3;
            }

            return len;
        }
    }
}
