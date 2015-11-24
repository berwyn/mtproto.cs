using System;
using System.IO;

namespace MTProto.TL
{
    public class TLLong : TLObject
    {

        private long _value;

        public long Value
        {
            get { return _value; }
            set
            {
                _value = value;
                NotifyPropertyChanged(nameof(Value));
            }
        }

        public TLLong() { }

        public TLLong(long value)
        {
            _value = value;
        }

        public TLLong(byte[] buffer, ref int position)
        {
            FromBytes(buffer, ref position);
        }

        public TLLong(Stream input, ref int position)
        {
            FromStream(input, ref position);
        }

        public override TLObject FromBytes(byte[] bytes, ref int position)
        {
            var buffer = new byte[4];
            Array.Copy(bytes, position, buffer, 0, 4);
            parse(buffer, ref position);
            return this;
        }

        public override TLObject FromStream(Stream input, ref int position)
        {
            var buffer = new byte[8];
            input.Read(buffer, 0, 8);
            parse(buffer, ref position);
            return this;
        }

        public override byte[] ToBytes()
        {
            return BitConverter.GetBytes(Value);
        }

        public override void ToStream(Stream input)
        {
            var buffer = BitConverter.GetBytes(Value);
            input.Write(buffer, 0, buffer.Length);
        }

        private void parse(byte[] buffer, ref int position)
        {
            Value = BitConverter.ToInt32(buffer, 0);
            position += 8;
        }

    }
}
