using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElecNetKit.Graphing.AdaptiveGradients
{
    /// <summary>
    /// Maps inputs to coloured objects based on an <see cref="AdaptiveGradient"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to map to and output.</typeparam>
    public class AdaptiveGradientMap<T>
    {
        double[] targetValues;
        T[] options;

        /// <summary>
        /// Obtain the nearest coloured match to part of the gradient for the
        /// specified value.
        /// </summary>
        /// <param name="value">The value to interpolate into the gradient.</param>
        /// <returns>The nearest mapped object.</returns>
        public T Map(double value)
        {
            int idx = Array.BinarySearch<double>(targetValues, value);
            if (idx < 0)
                idx = ~idx;
            idx = Math.Min(idx, options.Length - 1);
            return options[idx];
        }

        //hidden constructor!
        private AdaptiveGradientMap() { }

        /// <summary>
        /// Make a new AdaptiveGradientMap
        /// </summary>
        /// <param name="targetValues">A set of target values to interpolate
        /// between</param>
        /// <param name="options">A set of options to choose from (interpolate
        /// to).</param>
        public AdaptiveGradientMap(double[] targetValues, T[] options)
        {
            this.targetValues = targetValues;
            this.options = options;
        }
    }
}
