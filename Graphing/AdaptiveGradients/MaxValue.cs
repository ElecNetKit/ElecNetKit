using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElecNetKit.Graphing.AdaptiveGradients
{
    /// <summary>
    /// Anchors a gradient stop at the Maximum value in the data set.
    /// </summary>
    public class MaxValue : IAdaptiveGradientAnchor
    {
        private double _max;

        /// <summary>
        /// Initialises a new <see cref="MaxValue"/>.
        /// </summary>
        public MaxValue()
        {
            ((IAdaptiveGradientAnchor)this).Reset();
        }

        /// <inheritdoc />
        public void ProcessData(double data)
        {
            if (data > _max)
            {
                _max = data;
            }
        }

        /// <inheritdoc />
        public void Reset()
        {
            _max = double.NegativeInfinity;
        }

        /// <inheritdoc />
        public double AnchorValue
        {
            get { return _max; }
        }
    }
}
