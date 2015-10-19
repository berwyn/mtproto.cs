using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTProto.TL
{
    public class TLBool : TLObject
    {
        public const uint BoolTrue = 0x997275b5;
        public const uint BoolFalse = 0xbc799737;

        private bool _value;

        public bool Value
        {
            get { return _value; }
            set { _value = value; NotifyPropertyChanged(nameof(Value)); }
        }

        public override TLObject FromBytes(byte[] bytes, ref int position)
        {
            var buffer = new byte[4];
            Array.Copy(bytes, position, buffer, 0, 4);
            Parse(buffer, ref position);
            return this;
        }

        public override TLObject FromStream(Stream input, ref int position)
        {
            var buffer = new byte[4];
            input.Read(buffer, position, 4);
            Parse(buffer, ref position);
            return this;
        }

        public override byte[] ToBytes()
        {
            return BitConverter.GetBytes(_value ? BoolTrue : BoolFalse);
        }

        public override void ToStream(Stream input)
        {
            var buffer = ToBytes();
            for(var i = 0; i < 4; i++)
            {
                input.WriteByte(buffer[i]);
            }
        }

        private void Parse(byte[] buffer, ref int position)
        {
            var i = BitConverter.ToUInt32(buffer, position);
            switch(i)
            {
                case BoolTrue:
                    Value = true;
                    break;
                case BoolFalse:
                    Value = false;
                    break;
                default:
                    throw new InvalidDataException("Attempted to parse TLBool but no valid value found");
            }

            position += 4;
        }
    }
}
