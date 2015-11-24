using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MTProto.TL
{
    public class TLInputPeer : TLObject
    {

        public enum Signature : uint
        {
            InputPeerEmpty = 0x7f3b18ea,
            InputPeerSelf = 0x7da07ec9,
            InputPeerContact = 0x1023dbe8,
            InputPeerForeign = 0x9b447325,
            InputPeerChat = 0x179be863
        }

        public Signature SIGNATURE { get; set; }

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

        private TLInt _chatID;
        public TLInt ChatID
        {
            get { return _chatID; }
            set
            {
                _chatID = value;
                NotifyPropertyChanged(nameof(ChatID));
            }
        }

        public TLInputPeer() { }

        public TLInputPeer(byte[] bytes, ref int position)
        {
            FromBytes(bytes, ref position);
        }

        public TLInputPeer(Stream input, ref int position)
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
            bytes.Add(BitConverter.GetBytes((int)SIGNATURE));

            if (SIGNATURE == Signature.InputPeerForeign || SIGNATURE == Signature.InputPeerContact)
            {
                bytes.Add(UserID.ToBytes());
            }

            if (SIGNATURE == Signature.InputPeerForeign)
            {
                bytes.Add(AccessHash.ToBytes());
            }

            if (SIGNATURE == Signature.InputPeerChat)
            {
                bytes.Add(ChatID.ToBytes());
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
            var bytes = ToBytes();
            input.Write(bytes, 0, bytes.Length);
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

            throw new InvalidDataException("No valid TLInputPeer signature found");
        }

        private void parse(ref int position, byte[] buffer = null, Stream input = null)
        {
            if (buffer == null && input == null) throw new InvalidDataException();

            switch (this.SIGNATURE)
            {
                case Signature.InputPeerEmpty:
                case Signature.InputPeerSelf:
                    break;

                case Signature.InputPeerForeign:
                    UserID = buffer == null ? new TLInt(input, ref position) : new TLInt(buffer, ref position);
                    AccessHash = buffer == null ? new TLLong(input, ref position) : new TLLong(buffer, ref position);
                    break;

                case Signature.InputPeerContact:
                    UserID = buffer == null ? new TLInt(input, ref position) : new TLInt(buffer, ref position);
                    break;

                case Signature.InputPeerChat:
                    ChatID = buffer == null ? new TLInt(input, ref position) : new TLInt(buffer, ref position);
                    break;
            }
        }
    }
}
