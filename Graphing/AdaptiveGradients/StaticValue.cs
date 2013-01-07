using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElecNetKit.Graphing.AdaptiveGradients
{
    /// <summary>
    /// Anchors a gradient stop to a static value.
    /// </summary>
    public class StaticValue : IAdaptiveGradientAnchor
    {
        private StaticValue() { }

        private double _anchorValue;

        /// <summary>
        /// Initialise a new StaticValue gradient anchor.
        /// </summary>
        /// <param name="d">The fixed value to anchor the gradient stop to.</param>
        public StaticValue(double d)
        {
            _anchorValue = d;
        }

        /// <inheritdoc />
        public void ProcessData(double data)
        {
        }

        /// <inheritdoc />
        public void Reset()
        {
            //do nothing.
        }

        /// <summary>
        /// The value that the anchor is fixed to.
        /// </summary>
        public double AnchorValue
        {
            get { return _anchorValue; }
            set { _anchorValue = value; }
        }
    }
}
