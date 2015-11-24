using System.ComponentModel;
using System.IO;

namespace MTProto.TL
{
    public abstract class TLObject: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public abstract TLObject FromBytes(byte[] bytes, ref int position);
        public abstract byte[] ToBytes();
        public abstract TLObject FromStream(Stream input, ref int position);
        public abstract void ToStream(Stream input);

        protected void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
