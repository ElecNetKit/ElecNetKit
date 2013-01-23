using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.NetworkModelling.Phasing
{
    /// <summary>
    /// <see cref="PhasedEvaluated{TFrom, TTo}"/> applies transforms to the values from an underlying <see cref="Phased{T}"/>
    /// in order to obtain new values.
    /// </summary>
    /// <remarks><see cref="PhasedEvaluated{TFrom, TTo}"/> is useful for providing phased convenience properties to classes when there
    /// is no difference in underlying data.</remarks>
    /// <example>Consider the two properties of electrical network buses: voltage, and per-unit voltage. There is no
    /// point in storing these values twice in a network element model: The per-unit voltage would be redundant,
    /// as it is defined as (Voltage/BaseVoltage), and both of these values are stored in the network element.
    /// In such cases, it is best to use a <see cref="PhasedEvaluated{TFrom, TTo}"/> for implementation:
    /// <code source="../Examples/Snippets/PhasedEvaluated.cs" language="c#" /></example>
    /// <typeparam name="TFrom">The type of the base phased object.</typeparam>
    /// <typeparam name="TTo">The type of the values of this phased object.</typeparam>
    public class PhasedEvaluated<TFrom, TTo> : Phased<TTo>
        where TFrom : struct
        where TTo : struct
    {
        Func<TFrom, TTo> getTransform;
        Func<TTo, TFrom> setTransform;
        Phased<TFrom> basePhased;

        private PhasedEvaluated() { }

        /// <summary>
        /// Instantiates a new <see cref="PhasedEvaluated{TFrom, TTo}"/> with the specified transforms and base phased object.
        /// </summary>
        /// <param name="getTransform">A transform translating values from <paramref name="basePhased"/> to this <see cref="PhasedEvaluated{TFrom, TTo}"/>, for get operations.</param>
        /// <param name="setTransform">A transform translating values from this <see cref="PhasedEvaluated{TFrom, TTo}"/> to <paramref name="basePhased"/>, for set operations.</param>
        /// <param name="basePhased">The base phased object from which to get and set values from and to.</param>
        public PhasedEvaluated(Func<TFrom, TTo> getTransform, Func<TTo, TFrom> setTransform, Phased<TFrom> basePhased)
        {
            this.getTransform = getTransform;
            this.setTransform = setTransform;
            this.basePhased = basePhased;
        }

        /// <inheritdoc />
        public void Add(int key, TTo value)
        {
            basePhased.Add(key, setTransform(value));
        }

        /// <inheritdoc />
        public bool ContainsKey(int key)
        {
            return basePhased.ContainsKey(key);
        }

        /// <inheritdoc />
        public ICollection<int> Keys
        {
            get { return basePhased.Keys; }
        }

        /// <inheritdoc />
        public bool Remove(int key)
        {
            return basePhased.Remove(key);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public ICollection<TTo> Values
        {
            get { return new QueryableCollection<TFrom, TTo>(getTransform, basePhased.Values); }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void Add(KeyValuePair<int, TTo> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc />
        public void Clear()
        {
            basePhased.Clear();
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<int, TTo> item)
        {
            return basePhased.Contains(new KeyValuePair<int, TFrom>(item.Key, setTransform(item.Value)));
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<int, TTo>[] array, int arrayIndex)
        {
            basePhased.CopyTo(array.Select(kvp => new KeyValuePair<int, TFrom>(kvp.Key, setTransform(kvp.Value))).ToArray(), arrayIndex);
        }

        /// <inheritdoc />
        public int Count
        {
            get { return basePhased.Count; }
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get { return basePhased.IsReadOnly; }
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<int, TTo> item)
        {
            return basePhased.Remove(new KeyValuePair<int, TFrom>(item.Key, setTransform(item.Value)));
        }

        /// <inheritdoc />
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
