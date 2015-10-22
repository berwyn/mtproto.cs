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
            var buffer = new byte[4];
            Array.Copy(bytes, position, buffer, 0, 4);

            if(!BitConverter.ToInt32(buffer, 0).Equals(signature))
            {
                // TODO: Messaging?
                throw new InvalidDataException();
            }

            position += 4;
        }
    }
}
