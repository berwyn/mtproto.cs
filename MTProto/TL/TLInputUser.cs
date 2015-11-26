using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTProto.TL
{
    public class TLInputUser : TLObject
    {

        public enum Signature : uint
        {
            InputUserEmpty = 0xb98886cf,
            InputUserSelf = 0xf7c1b13f,
            InputUserContact = 0x86e94f65,
            InputUserForeign = 0x655e74ff
        };

        private Signature _signature;
        public Signature SIGNATURE
        {
            get { return _signature; }
            set
            {
                _signature = value;
                NotifyPropertyChanged(nameof(SIGNATURE));
            }
        }

        private TLInt _userID;
        public TLInt UserID
        {
            get { return _userID; }
            set
            {
                _userID = value;
                NotifyPropertyChanged(nameof(UserID));
            }
        }

        private TLLong _accessHash;
        public TLLong AccessHash
        {
            get { return _accessHash; }
            set
            {
                _accessHash = value;
                NotifyPropertyChanged(nameof(AccessHash));
            }
        }

        public TLInputUser() { }

        public TLInputUser(byte[] bytes, ref int position)
        {
            FromBytes(bytes, ref position);
        }

        public TLInputUser(Stream input, ref int position)
        {
            FromStream(input, ref position);
        }

        public override TLObject FromBytes(byte[] bytes, ref int position)
        {
            checkSignature(ref position, buffer: bytes);
            parse(ref position, buffer: bytes);
            return this;
        }

        public override byte[] ToBytes()
        {
            List<byte[]> bytes = new List<byte[]>();
            bytes.Add(BitConverter.GetBytes((uint)SIGNATURE));

            if (SIGNATURE == Signature.InputUserForeign || SIGNATURE == Signature.InputUserContact)
            {
                bytes.Add(UserID.ToBytes());
            }

            if (SIGNATURE == Signature.InputUserForeign)
            {
                bytes.Add(AccessHash.ToBytes());
            }

            return bytes.SelectMany(x => x).ToArray();
        }

        public override TLObject FromStream(Stream input, ref int position)
        {
            checkSignature(ref position, input: input);
            parse(ref position, input: input);
            return this;
        }

        public override void ToStream(Stream input)
        {
            var buffer = ToBytes();
            input.Write(buffer, 0, buffer.Length);
        }

        private void checkSignature(ref int position, byte[] buffer = null, Stream input = null)
        {
            if (buffer == null && input == null) throw new InvalidDataException();

            bool found = false;
            foreach (Signature value in Enum.GetValues(typeof(Signature)))
            {
                if (buffer != null)
                {
                    found = buffer.IsSignatureValid(position, (int)value);
                }
                else
                {
                    found = input.IsSignatureValid(position, (int)value);
                }

                if (found)
                {
                    this.SIGNATURE = value;

                    // Because the check doesn't mutate the position, we need to
                    position += 4;
                    if (input != null) input.Position = position;
                    return;
                }
            }

            throw new InvalidDataException("No valid TLInputUser signature found");
        }

        private void parse(ref int position, byte[] buffer = null, Stream input = null)
        {
            if (buffer == null && input == null) throw new InvalidDataException();

            switch (this.SIGNATURE)
            {
                case Signature.InputUserEmpty:
                case Signature.InputUserSelf:
                    break;

                case Signature.InputUserForeign:
                    UserID = buffer == null ? new TLInt(input, ref position) : new TLInt(buffer, ref position);
                    AccessHash = buffer == null ? new TLLong(input, ref position) : new TLLong(buffer, ref position);
                    break;

                case Signature.InputUserContact:
                    UserID = buffer == null ? new TLInt(input, ref position) : new TLInt(buffer, ref position);
                    break;
            }
        }
    }
}
