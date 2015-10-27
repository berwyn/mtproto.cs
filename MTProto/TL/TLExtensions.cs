using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTProto.TL
{
    public static class TLExtensions
    {
        public static void ThrowIfIncorrectSignature(this byte[] bytes, ref int position, int signature)
        {
            int localSig = new TLInt(bytes, ref position).Value;
            if (localSig != signature)
            {
                throw new InvalidDataException();
            }
        }

        public static void ThrowIfIncorrectSignature(this Stream input, ref int position, int signature)
        {
            int localSig = new TLInt(input, ref position).Value;
            if(localSig != signature)
            {
                throw new InvalidDataException();
            }
        }
    }
}
