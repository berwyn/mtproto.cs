﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTProto.TL
{
    public class TLUint : TLObject
    {
        private uint _value;
        public uint Value
        {
            get { return _value; }
            set
            {
                _value = value;
                NotifyPropertyChanged(nameof(Value));
            }
        }

        public TLUint() { }

        public TLUint(uint value)
        {
            _value = value;
        }

        public TLUint(byte[] buffer, ref int position)
        {
            FromBytes(buffer, ref position);
        }

        public TLUint(Stream input, ref int position)
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
            input.Write(buffer, 0, buffer.Length);
        }

        private void parse(byte[] buffer, ref int position)
        {
            Value = BitConverter.ToUInt32(buffer, 0);
            position += 4;
        }
    }
}
