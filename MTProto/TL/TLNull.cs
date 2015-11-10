using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTProto.TL
{
    public class TLNull : TLObject
    {

        private const int SIGNATURE = 0x56730bcc;

        public object Value { get; } = null;

        public TLNull() { }

        public TLNull(byte[] bytes, ref int position)
        {
            FromBytes(bytes, ref position);
        }

        public TLNull(Stream input, ref int position)
        {
            FromStream(input, ref position);
        }

        public override TLObject FromBytes(byte[] bytes, ref int position)
        {
            bytes.ThrowIfIncorrectSignature(ref position, SIGNATURE);
            return this;
        }

        public override byte[] ToBytes()
        {
            return BitConverter.GetBytes(SIGNATURE);
        }

        public override TLObject FromStream(Stream input, ref int position)
        {
            var buffer = new byte[4];
            input.Read(buffer, 0, 4);
            buffer.ThrowIfIncorrectSignature(ref position, SIGNATURE);
            return this;
        }

        public override void ToStream(Stream input)
        {
            var buffer = ToBytes();
            input.Write(buffer, 0, 4);
        }
    }
}
