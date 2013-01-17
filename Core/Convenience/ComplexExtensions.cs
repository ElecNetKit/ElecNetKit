using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.Convenience
{
    /// <summary>
    /// Utility functions for <see cref="IEnumerable{Complex}"/>.
    /// </summary>
    public static class ComplexExtensions
    {
        /// <summary>
        /// Returns the sum of a set of complex numbers.
        /// </summary>
        /// <param name="enumerable">The set of complex numbers to sum.</param>
        /// <returns>The sum of the complex numbers in <paramref name="enumerable"/>.</returns>
        public static Complex Sum(this IEnumerable<Complex> enumerable)
        {
            return enumerable.Aggregate((seed, elem) => seed + elem);
        }
    }
}
