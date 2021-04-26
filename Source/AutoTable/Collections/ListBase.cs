using System.Collections;
using System.Collections.Generic;

namespace AutoTable
{
    public abstract class ListBase<T> : IList<T>
    {
        protected abstract List<T> List { get; }

        public int Count => List.Count;

        public void Add(T item)
        {
            List.Add(item);
        }

        public void Clear()
        {
            List.Clear();
        }

        public bool Contains(T item)
        {
            return List.Contains(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        public bool Remove(T item)
        {
            return List.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T this[int index] { get => List[index]; set => List[index] = value; }

        public bool IsReadOnly => false;

        public void CopyTo(T[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            return List.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            List.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
        }
    }
}