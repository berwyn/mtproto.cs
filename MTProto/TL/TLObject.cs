using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTProto.TL
{
    public abstract class TLObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public abstract TLObject FromBytes(byte[] bytes, ref int position);
        public abstract byte[] ToBytes();
        public abstract TLObject FromStream(Stream input, ref int position);
        public abstract void ToStream(Stream input);

        protected void NotifyPropertyChanged(string name)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
