using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.NetworkModelling.Phasing
{
    /// <summary>
    /// An implementation of <see cref="Phased{T}"/> that is read-only and works with objects
    /// (as opposed to <see cref="PhasedEvaluated{TFrom,T}"/>, which only works with value types).
    /// </summary>
    /// <typeparam name="TFrom">The type to convert from.</typeparam>
    /// <typeparam name="T">The type fo convert to.</typeparam>
    public class PhasedReadOnlyEvaluated<TFrom,T> : Phased<T>
    {
        /// <summary>
        /// The transform that is used to return values of type <typeparamref name="T"/>
        /// from the backing <see cref="Phased{TFrom}"/>.
        /// </summary>
        protected Func<TFrom, T> getTransform;

        /// <summary>
        /// The backing <see cref="Phased{TFrom}"/>.
        /// </summary>
        protected Phased<TFrom> basePhased;

        /// <summary>
        /// Instantiates a new <see cref="PhasedReadOnlyEvaluated{TFrom,T}"/>.
        /// </summary>
        protected PhasedReadOnlyEvaluated() { }

        /// <summary>
        /// Instantiates a new <see cref="PhasedReadOnlyEvaluated{TFrom,T}"/>.
        /// </summary>
        /// <param name="getTransform">A function that converts from <typeparamref name="TFrom"/> to <typeparamref name="T"/>.</param>
        /// <param name="basePhased">The backing <see cref="Phased{TFrom}"/>.</param>
        public PhasedReadOnlyEvaluated(Func<TFrom, T> getTransform, Phased<TFrom> basePhased)
        {
            this.getTransform = getTransform;
            this.basePhased = basePhased;
        }

        /// <summary>
        /// Not supported. Always throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="key">Not used.</param>
        /// <param name="value">Not used.</param>
        public void Add(int key, T value)
        {
            throw new NotSupportedException();
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

        /// <summary>
        /// Not supported. Always throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="key">Not used.</param>
        /// <returns>Never returns.</returns>
        public bool Remove(int key)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public ICollection<T> Values
        {
            get { return new QueryableCollection<TFrom, T>(getTransform, basePhased.Values); }
        }

        /// <inheritdoc />
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

        /// <summary>
        /// Not supported. Always throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="item">Not used.</param>
        public void Add(KeyValuePair<int, T> item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported. Always throws a <see cref="NotSupportedException"/>.
        /// </summary>
        public void Clear()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<int, T> item)
        {
            return basePhased.ContainsKey(item.Key) && getTransform(basePhased[item.Key]).Equals(item.Value);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public int Count
        {
            get { return basePhased.Count; }
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Not supported. Always throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="item">Not used.</param>
        /// <returns>Never returns.</returns>
        public bool Remove(KeyValuePair<int, T> item)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
        {
            return basePhased.Select(kvp => new KeyValuePair<int, T>(kvp.Key, getTransform(kvp.Value))).GetEnumerator();
        }

        /// <inheritdoc />
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)(GetEnumerator());
        }
    }
}
