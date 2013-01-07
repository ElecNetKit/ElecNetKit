using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;

namespace ElecNetKit.Graphing.AdaptiveGradients
{
    /// <summary>
    /// Provides functions for building gradients around data. Colours can be
    /// anchored to not only fixed data points (see <see cref="StaticValue"/>),
    /// but also to the minimum value (<see cref="MinValue"/>) and maximum value
    /// (<see cref="MaxValue"/>) of the data-set, for example.
    /// </summary>
    public class AdaptiveGradient : List<AdaptiveGradientStop>
    {
        /// <summary>
        /// Instantiates a new <see cref="AdaptiveGradient"/>.
        /// </summary>
        /// <remarks>
        /// Note: If you're after a default gradient,
        /// use <see cref="AdaptiveGradient.BlueRedGradient()"/> or
        /// <see cref="AdaptiveGradient.BlueBlackRedGradient()"/>.</remarks>
        public AdaptiveGradient()
        {

        }

        /// <summary>
        /// Generate a default gradient, with <see cref="Colors.Blue"/> at the
        /// minimum value of the dataset and <see cref="Colors.Red"/> at the
        /// maximum.
        /// </summary>
        /// <returns>A default gradient, auto scaled from blue to red.</returns>
        public static AdaptiveGradient BlueRedGradient()
        {
            AdaptiveGradient ag = new AdaptiveGradient();
            //default blue = low, red = high.
            ag.Add(new AdaptiveGradientStop(Colors.Blue, new MinValue()));
            ag.Add(new AdaptiveGradientStop(Colors.Red, new MaxValue()));

            return ag;
        }

        /// <summary>
        /// Generate a gradient, with <see cref="Colors.Blue"/> at the
        /// minimum value of the dataset, <see cref="Colors.Black"/> at zero,
        /// and <see cref="Colors.Red"/> at the maximum. Useful for
        /// differential measures.
        /// </summary>
        /// <returns>An <see cref="AdaptiveGradient"/>, ready to go!</returns>
        public static AdaptiveGradient BlueBlackRedGradient()
        {
            AdaptiveGradient ag = new AdaptiveGradient();
            //default blue = low, red = high.
            ag.Add(new AdaptiveGradientStop(Colors.Blue, new MinValue()));
            ag.Add(new AdaptiveGradientStop(Colors.Black, new StaticValue(0)));
            ag.Add(new AdaptiveGradientStop(Colors.Red, new MaxValue()));

            return ag;
        }

        /// <summary>
        /// Resets all Auto Data in the gradient.
        /// </summary>
        public void ResetAutoData()
        {
            foreach (AdaptiveGradientStop stop in this)
            {
                stop.Anchor.Reset();
            }
        }

        /// <summary>
        /// Takes a piece of data to adapt the gradient to, and sends it to all
        /// child <see cref="AdaptiveGradientStop"/>s, for adaptation to
        /// the new data.
        /// </summary>
        /// <param name="data">The data to process.</param>
        public void ProcessData(double data)
        {
            foreach (AdaptiveGradientStop stop in this)
            {
                stop.Anchor.ProcessData(data);
            }
        }
    }
}
