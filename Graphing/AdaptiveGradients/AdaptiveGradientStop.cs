using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;


namespace ElecNetKit.Graphing.AdaptiveGradients
{
    /// <summary>
    /// Represents a Gradient Stop, comprised of a <see cref="Color"/> and an <see cref="Anchor"/>.
    /// </summary>
    public class AdaptiveGradientStop
    {
        /// <summary>
        /// 
        /// </summary>
        public Color Color { private set; get; }

        /// <summary>
        /// The location of the color stop. Because it's an *Adaptive* Gradient,
        /// can be anything that implements the <see cref="IAdaptiveGradientAnchor"/> interface
        /// (any function of a dataset).
        /// </summary>
        public IAdaptiveGradientAnchor Anchor { private set; get; }

        // Private, so that it can't be accessed publicly. Use the other
        // constructor instead.
        private AdaptiveGradientStop() { }

        /// <summary>
        /// Instantiates a new <see cref="AdaptiveGradientStop"/>.
        /// </summary>
        /// <param name="Color">The Color of the stop.</param>
        /// <param name="Anchor">The location of the stop.</param>
        public AdaptiveGradientStop(Color Color, IAdaptiveGradientAnchor Anchor)
        {
            this.Color = Color;
            this.Anchor = Anchor;
        }
    }
}
