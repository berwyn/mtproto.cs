using System;
using System.IO;

namespace MTProto.TL
{
    public class TLTrue : TLObject
    {
        private const int SIGNATURE = 0x3fedd339;

        public bool Value { get; } = true;

        public TLTrue() { }

        public TLTrue(byte[] bytes, ref int position)
        {
            FromBytes(bytes, ref position);
        }

        public TLTrue(Stream input, ref int position)
        {
            FromStream(input, ref position);
        }

        public override TLObject FromBytes(byte[] bytes, ref int position)
        {
            bytes.ThrowIfIncorrectSignature(ref position, SIGNATURE);
            return this;
        }

        public override TLObject FromStream(Stream input, ref int position)
        {
            var buffer = new byte[4];
            input.Read(buffer, 0, 4);
            buffer.ThrowIfIncorrectSignature(ref position, SIGNATURE);
            return this;
        }

        public override byte[] ToBytes()
        {
            return BitConverter.GetBytes(SIGNATURE);
        }

        public override void ToStream(Stream input)
        {
            var bytes = ToBytes();
            input.Write(bytes, 0, bytes.Length);
        }
    }
}
