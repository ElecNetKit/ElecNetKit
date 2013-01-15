using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.NetworkModelling.Phasing
{
    public class QueryableCollection<TBase, TTo> : ICollection<TTo>
    {
        Func<TBase, TTo> transform;
        Func<TTo, TBase> reverseTransform;
        ICollection<TBase> theBase;

        public QueryableCollection(Func<TBase, TTo> transform, Func<TTo, TBase> reverseTransform, ICollection<TBase> theBase)
        {
            this.transform = transform;
            this.reverseTransform = reverseTransform;
            this.theBase = theBase;
        }

        public void Add(TTo item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(TTo item)
        {
            return theBase.Contains(reverseTransform(item));
        }

        public void CopyTo(TTo[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public int Count
        {
            get { return theBase.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(TTo item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<TTo> GetEnumerator()
        {
            return theBase.Select(transform).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)GetEnumerator();
        }
    }
}
