using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElecNetKit.Convenience
{
    static class IEnumerableExt
    {
        // usage: someObject.SingleItemAsEnumerable();
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}
