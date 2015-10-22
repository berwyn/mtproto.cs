using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTProto.TL
{
    public class TLInt : TLObject
    {

        private int _value;

        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                NotifyPropertyChanged(nameof(Value));
            }
        }

        public TLInt() { }

        public TLInt(int value)
        {
            _value = value;
        }

        public TLInt(byte[] buffer, ref int position)
        {
            FromBytes(buffer, ref position);
        }

        public TLInt(Stream input, ref int position)
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
            var buffer = new byte[4];
            input.Read(buffer, 0, 4);
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
            for(int i = 0, len = buffer.Length; i < len; i++)
            {
                input.WriteByte(buffer[i]);
            }
        }

        private void parse(byte[] buffer, ref int position)
        {
            Value = BitConverter.ToInt32(buffer, 0);
            position += 4;
        }
    }
}
