using System.IO;

namespace MTProto.TL
{
    public static class TLExtensions
    {
        /// <summary>
        /// When called on a byte array, this method will read in the TLInt signature
        /// of the buffer, and throw an exception if it doesn't match the expected
        /// signature. This will advance the position.
        /// </summary>
        /// <param name="bytes">The buffer to operate on</param>
        /// <param name="position">The position in the buffer to start reading at</param>
        /// <param name="signature">The signature to check for</param>
        public static void ThrowIfIncorrectSignature(this byte[] bytes, ref int position, uint signature)
        {
            uint localSig = new TLUint(bytes, ref position).Value;
            if (localSig != signature)
            {
                throw new InvalidDataException();
            }
        }

        /// <summary>
        /// Given a signature to check for, this method will read in the TLint signature
        /// of the buffer at the provided index and check if it matches. This does not
        /// advance the position.
        /// </summary>
        /// <param name="bytes">The buffer to operate on</param>
        /// <param name="position">The position in the buffer to start reading at</param>
        /// <param name="signature">The signature to check for</param>
        /// <returns>Whether or not the signature was found</returns>
        public static bool IsSignatureValid(this byte[] bytes, int position, int signature)
        {
            int localSig = new TLInt(bytes, ref position).Value;
            return localSig == signature;
        }

        /// <summary>
        /// When called on a byte array, this method will read in the TLInt signature
        /// of the stream, and throw an exception if it doesn't match the expected
        /// signature. This will advance the position.
        /// </summary>
        /// <param name="input">The stream to operate on</param>
        /// <param name="position">The position in the stream to start reading at</param>
        /// <param name="signature">The signature to check for</param>
        public static void ThrowIfIncorrectSignature(this Stream input, ref int position, uint signature)
        {
            uint localSig = new TLUint(input, ref position).Value;
            if (localSig != signature)
            {
                throw new InvalidDataException();
            }
        }

        /// <summary>
        /// Given a signature to check for, this method will read in the TLint signature
        /// of the stream at the provided index and check if it matches. This does not
        /// advance the position.
        /// </summary>
        /// <param name="input">The stream to operate on</param>
        /// <param name="position">The position in the stream to start reading at</param>
        /// <param name="signature">The signature to check for</param>
        /// <returns>Whether or not the signature was found</returns>
        public static bool IsSignatureValid(this Stream input, int position, int signature)
        {
            int localSig = new TLInt(input, ref position).Value;
            input.Position -= 4; // Move the stream position back to its starting location
            return localSig == signature;
        }
    }
}
