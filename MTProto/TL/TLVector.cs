using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTProto.TL
{
    class TLVector<T> : TLObject, ICollection<T>
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
            throw new NotImplementedException();
        }

        public override TLObject FromStream(Stream input, ref int position)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override void ToStream(Stream input)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();
    }
}
