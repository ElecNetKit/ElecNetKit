using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.NetworkModelling.Phasing
{
    /// <summary>
    /// An implementation of <see cref="Phased{T}"/> that is read-only, works with objects
    /// (as opposed to <see cref="PhasedEvaluated{TFrom,T}"/>, which only works with value types)
    /// and returns 
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class PhasedReadOnlyEvaluated<TFrom,T> : Phased<T>
    {
        protected Func<TFrom, T> getTransform;
        protected Phased<TFrom> basePhased;

        protected PhasedReadOnlyEvaluated() { }

        public PhasedReadOnlyEvaluated(Func<TFrom, T> getTransform, Phased<TFrom> basePhased)
        {
            this.getTransform = getTransform;
            this.basePhased = basePhased;
        }

        public void Add(int key, T value)
        {
            throw new NotSupportedException();
        }

        public bool ContainsKey(int key)
        {
            return basePhased.ContainsKey(key);
        }

        public ICollection<int> Keys
        {
            get { return basePhased.Keys; }
        }

        public bool Remove(int key)
        {
            throw new NotSupportedException();
        }

        public bool TryGetValue(int key, out T value)
        {
            TFrom val;
            bool success = basePhased.TryGetValue(key, out val);
            if (success)
                value = getTransform(val);
            else
                value = default(T);
            return success;
        }

        public ICollection<T> Values
        {
            get { return new QueryableCollection<TFrom, T>(getTransform, basePhased.Values); }
        }

        public T this[int key]
        {
            get
            {
                return getTransform(basePhased[key]);
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public void Add(KeyValuePair<int, T> item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(KeyValuePair<int, T> item)
        {
            return basePhased.ContainsKey(item.Key) && getTransform(basePhased[item.Key]).Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<int, T>[] array, int arrayIndex)
        {
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException();
            if (array == null)
                throw new ArgumentNullException();

            foreach (var kvp in basePhased)
            {
                if (arrayIndex >= array.Length)
                    throw new ArgumentException();
                array[arrayIndex] = new KeyValuePair<int, T>(kvp.Key, getTransform(kvp.Value));
                arrayIndex++;
            }

        }

        public int Count
        {
            get { return basePhased.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(KeyValuePair<int, T> item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
        {
            return basePhased.Select(kvp => new KeyValuePair<int, T>(kvp.Key, getTransform(kvp.Value))).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)(GetEnumerator());
        }
    }
}
