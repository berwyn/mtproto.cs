using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MTProto.TL
{
    public class TLInputPhoneContact : TLObject
    {

        public const uint SIGNATURE = 0xf392b7f4;

        private TLLong _clientID;
        public TLLong ClientID
        {
            get { return _clientID; }
            set
            {
                _clientID = value;
                NotifyPropertyChanged(nameof(ClientID));
            }
        }

        private TLString _phone;
        public TLString Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                NotifyPropertyChanged(nameof(Phone));
            }
        }

        private TLString _firstName;
        public TLString FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                NotifyPropertyChanged(nameof(FirstName));
            }
        }

        private TLString _lastName;
        public TLString LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                NotifyPropertyChanged(nameof(LastName));
            }
        }

        public TLInputPhoneContact(byte[] buffer, ref int position)
        {
            FromBytes(buffer, ref position);
        }

        public TLInputPhoneContact(Stream input, ref int position)
        {
            FromStream(input, ref position);
        }

        public TLInputPhoneContact(long? clientID = null, string phone = null, string firstName = null, string lastName = null)
        {
            if (clientID != null) this.ClientID = new TLLong(clientID.Value);
            if (phone != null) this.Phone = new TLString(phone);
            if (firstName != null) this.FirstName = new TLString(firstName);
            if (lastName != null) this.LastName = new TLString(lastName);
        }

        public override TLObject FromBytes(byte[] bytes, ref int position)
        {
            bytes.ThrowIfIncorrectSignature(ref position, SIGNATURE);

            ClientID = new TLLong(bytes, ref position);
            Phone = new TLString(bytes, ref position);
            FirstName = new TLString(bytes, ref position);
            LastName = new TLString(bytes, ref position);

            return this;
        }

        public override byte[] ToBytes()
        {
            return new List<byte[]>
            {
                BitConverter.GetBytes(SIGNATURE),
                ClientID.ToBytes(),
                Phone.ToBytes(),
                FirstName.ToBytes(),
                LastName.ToBytes()
            }.SelectMany(x => x).ToArray();
        }

        public override TLObject FromStream(Stream input, ref int position)
        {
            input.ThrowIfIncorrectSignature(ref position, SIGNATURE);

            ClientID = new TLLong(input, ref position);
            Phone = new TLString(input, ref position);
            FirstName = new TLString(input, ref position);
            LastName = new TLString(input, ref position);

            return this;
        }

        public override void ToStream(Stream input)
        {
            var bytes = ToBytes();
            input.Write(bytes, 0, bytes.Length);
        }
    }
}
