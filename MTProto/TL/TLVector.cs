using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTProto.TL
{
    public class TLVector<T> : TLObject, ICollection<T> where T : TLObject, new()
    {
        private const int Signatiure = 0x1cb5c415;

        private List<T> _collection;

        public List<T> Value
        {
            get { return _collection; }
            set
            {
                _collection = value;
                NotifyPropertyChanged(nameof(Value));
            }
        }

        public TLVector()
        {
            _collection = new List<T>();
        }

        public TLVector(int capacity)
        {
            _collection = new List<T>(capacity);
        }

        public TLVector(byte[] bytes, ref int position) : this()
        {
            this.FromBytes(bytes, ref position);
        }

        public TLVector(Stream input, ref int position) : this()
        {
            this.FromStream(input, ref position);
        }

        public T this[int index] => _collection[index];

        public int Count => _collection.Count();

        public bool IsReadOnly { get; } = false;

        public void Add(T item)
        {
            _collection.Add(item);
            NotifyPropertyChanged(nameof(Value));
        }

        public void Clear()
        {
            _collection.Clear();
            NotifyPropertyChanged(nameof(Value));
        }

        public bool Contains(T item) => _collection.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);

        public override TLObject FromBytes(byte[] bytes, ref int position)
        {
            bytes.ThrowIfIncorrectSignature(ref position, Signatiure);
            int length = new TLInt(bytes, ref position).Value;
            for(var i = 0; i < length; i++)
            {
                T item = new T();
                item.FromBytes(bytes, ref position);
                Add(item);
            }
            return this;
        }

        public override TLObject FromStream(Stream input, ref int position)
        {
            input.ThrowIfIncorrectSignature(ref position, Signatiure);
            int length = new TLInt(input, ref position).Value;
            for(var i = 0; i < length; i++)
            {
                T item = new T();
                item.FromStream(input, ref position);
                Add(item);
            }
            return this;
        }

        public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

        public bool Remove(T item)
        {
            var removed = _collection.Remove(item);
            if(removed) NotifyPropertyChanged(nameof(Value));
            return removed;
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Signatiure));
            bytes.AddRange(new TLInt(Count).ToBytes());
            for(int i = 0, len = Count; i < len; i++)
            {
                bytes.AddRange(this[i].ToBytes());
            }
            return bytes.ToArray();
        }

        public override void ToStream(Stream input)
        {
            var bytes = ToBytes();
            input.Write(bytes, 0, bytes.Length);
        }

        IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();
    }
}
