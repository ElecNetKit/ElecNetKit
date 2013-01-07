using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElecNetKit.Graphing.AdaptiveGradients
{
    /// <summary>
    /// Anchors a gradient stop at the Minimum value in the data set.
    /// </summary>
    public class MinValue : IAdaptiveGradientAnchor
    {
        private double _min;

        /// <summary>
        /// Initialises a new MinValue.
        /// </summary>
        public MinValue()
        {
            ((IAdaptiveGradientAnchor)this).Reset();
        }

        /// <inheritdoc />
        public void ProcessData(double data)
        {
            if (data < _min)
                _min = data;
        }

        /// <inheritdoc />
        public void Reset()
        {
            _min = double.PositiveInfinity;
        }

        /// <inheritdoc />
        public double AnchorValue
        {
            get { return _min; }
        }
    }
}
