using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.NetworkModelling.Phasing
{
    /// <summary>
    /// A read-only collection that uses a transform function to
    /// transform the individual elements of another collection.
    /// </summary>
    /// <typeparam name="TBase">The type of the underlying <see cref="ICollection{T}"/>.</typeparam>
    /// <typeparam name="TTo">The type of this collection.</typeparam>
    public class QueryableCollection<TBase, TTo> : ICollection<TTo>
    {
        Func<TBase, TTo> transform;
        Func<TTo, TBase> reverseTransform;
        ICollection<TBase> theBase;

        /// <summary>
        /// Instantiates a new <see cref="QueryableCollection{TBase,TTo}"/> with the specified translation functions and base <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="transform">A function that transforms the elements of the base collection to <typeparamref name="TTo"/>.</param>
        /// <param name="reverseTransform">A function that transforms elements of this collection back to the corresponding elements in the base collection.</param>
        /// <param name="theBase">The base collection.</param>
        public QueryableCollection(Func<TBase, TTo> transform, Func<TTo, TBase> reverseTransform, ICollection<TBase> theBase)
        {
            this.transform = transform;
            this.reverseTransform = reverseTransform;
            this.theBase = theBase;
        }

        /// <summary>
        /// Not supported. Always throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="item">Not used.</param>
        public void Add(TTo item)
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

        ///<inheritdoc />
        public bool Contains(TTo item)
        {
            return theBase.Contains(reverseTransform(item));
        }

        /// <summary>
        /// Not supported. Always throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="array">Not used.</param>
        /// <param name="arrayIndex">Not used.</param>
        public void CopyTo(TTo[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        ///<inheritdoc />
        public int Count
        {
            get { return theBase.Count; }
        }

        ///<inheritdoc />
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Not supported. Always throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="item">Not used.</param>
        public bool Remove(TTo item)
        {
            throw new NotSupportedException();
        }

        ///<inheritdoc />
        public IEnumerator<TTo> GetEnumerator()
        {
            return theBase.Select(transform).GetEnumerator();
        }

        ///<inheritdoc />
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)GetEnumerator();
        }
    }
}
