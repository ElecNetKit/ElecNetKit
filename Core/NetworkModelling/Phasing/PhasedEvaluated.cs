using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.NetworkModelling.Phasing
{
    public class PhasedEvaluated<TFrom, TTo> : Phased<TTo>
        where TFrom : struct
        where TTo : struct
    {
        Func<TFrom, TTo> getTransform;
        Func<TTo, TFrom> setTransform;
        Phased<TFrom> basePhased;

        private PhasedEvaluated() { }

        public PhasedEvaluated(Func<TFrom, TTo> getTransform, Func<TTo, TFrom> setTransform, Phased<TFrom> basePhased)
        {
            this.getTransform = getTransform;
            this.setTransform = setTransform;
            this.basePhased = basePhased;
        }

        public void Add(int key, TTo value)
        {
            basePhased.Add(key, setTransform(value));
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
            return basePhased.Remove(key);
        }

        public bool TryGetValue(int key, out TTo value)
        {
            TFrom tempValue;
            bool success = basePhased.TryGetValue(key, out tempValue);
            if (success)
                value = getTransform(tempValue);
            else
                value = default(TTo);
            return success;
        }

        public ICollection<TTo> Values
        {
            get { return new QueryableCollection<TFrom, TTo>(getTransform, setTransform, basePhased.Values); }
        }

        public TTo this[int key]
        {
            get
            {
                return getTransform(basePhased[key]);
            }
            set
            {
                basePhased[key] = setTransform(value);
            }
        }

        public void Add(KeyValuePair<int, TTo> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            basePhased.Clear();
        }

        public bool Contains(KeyValuePair<int, TTo> item)
        {
            return basePhased.Contains(new KeyValuePair<int, TFrom>(item.Key, setTransform(item.Value)));
        }

        public void CopyTo(KeyValuePair<int, TTo>[] array, int arrayIndex)
        {
            basePhased.CopyTo(array.Select(kvp => new KeyValuePair<int, TFrom>(kvp.Key, setTransform(kvp.Value))).ToArray(), arrayIndex);
        }

        public int Count
        {
            get { return basePhased.Count; }
        }

        public bool IsReadOnly
        {
            get { return basePhased.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<int, TTo> item)
        {
            return basePhased.Remove(new KeyValuePair<int, TFrom>(item.Key, setTransform(item.Value)));
        }

        public IEnumerator<KeyValuePair<int, TTo>> GetEnumerator()
        {
            return basePhased.Select(kvp => new KeyValuePair<int, TTo>(kvp.Key, getTransform(kvp.Value))).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)(GetEnumerator());
        }
    }
}
