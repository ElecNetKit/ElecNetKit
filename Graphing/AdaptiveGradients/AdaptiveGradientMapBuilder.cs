using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;

namespace ElecNetKit.Graphing.AdaptiveGradients
{
    /// <summary>
    /// Builds <see cref="AdaptiveGradientMap{T}"/>s from <see cref="AdaptiveGradient"/>s.
    /// </summary>
    public class AdaptiveGradientMapBuilder
    {
        /// <summary>
        /// The number of samples to provide in the gradient map.
        /// </summary>
        public uint NumSamples { protected set; get; }

        Color[] colorValues;
        double[] targetValues;

        //sneaky hidden thingy!
        private AdaptiveGradientMapBuilder() { }

        /// <summary>
        /// Generates <see cref="AdaptiveGradientMap{T}"/>s based upon the specified
        /// <see cref="AdaptiveGradient"/> and a number of samples.
        /// </summary>
        /// <param name="gradient">The gradient to generate a map with.</param>
        /// <param name="numSamples">The number of samples to interpolate on the
        /// gradient.</param>
        public AdaptiveGradientMapBuilder(AdaptiveGradient gradient, uint numSamples = 10)
        {
            NumSamples = numSamples;

            targetValues = GenerateTargetValues(gradient, numSamples);
            colorValues = GenerateColors(gradient, numSamples);
        }

        /// <summary>
        /// Builds a gradient map with a transform function.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="AdaptiveGradientMap{T}"/>
        /// to build (think <see cref="Brush"/>, <see cref="Pen"/>, etc).</typeparam>
        /// <param name="TransformFunction">A function that transforms a <see cref="Color"/>
        /// into a <typeparamref name="T"/>.</param>
        /// <returns>The corresponding map.</returns>
        public AdaptiveGradientMap<T> BuildGradientMap<T>(Func<Color,T> TransformFunction)
        {
            //we'll use the terminology key->value for the map.
            T[] outputValues = new T[NumSamples];
            for (int i = 0; i < NumSamples; i++)
            {
                outputValues[i] = TransformFunction(colorValues[i]);
            }
            return new AdaptiveGradientMap<T>(targetValues, outputValues);
        }

        static Color[] GenerateColors(AdaptiveGradient gradient, uint numSamples)
        {
            double leftAnchor = gradient.First().Anchor.AnchorValue;
            double rightAnchor = gradient.Last().Anchor.AnchorValue;

            //we need to make sure it spreads out across the whole range, so we set the
            //increment such that we include the end two colors.
            double increment = (rightAnchor - leftAnchor) / (numSamples - 1);

            Color[] colors = new Color[numSamples];

            double targetValue;


            for (int i = 0; i < numSamples; i++)
            {
                targetValue = leftAnchor + increment * i;
                //consider stops in pairs, and if we're in the right pair
                // interpolate between the stops.
                for (int j = 0; j < gradient.Count - 1; j++)
                {
                    //because of the way we're counting, if this condition
                    // is true, we're between a pair of gradient stops
                    // with the left one smaller and the right one larger.
                    if (targetValue >= gradient[j].Anchor.AnchorValue)
                    {
                        colors[i] = InterpolateBetweenStops(targetValue, gradient[j], gradient[j + 1]);
                        continue;
                    }
                }
            }
            return colors;
        }

        static Color InterpolateBetweenStops(double value, AdaptiveGradientStop left, AdaptiveGradientStop right)
        {
            //work out how far between the two `value` is
            double value_frac = (value - left.Anchor.AnchorValue) /
                                (right.Anchor.AnchorValue - left.Anchor.AnchorValue);

            //just use RGB mode for the time being.
            int r_diff = right.Color.R - left.Color.R;
            int g_diff = right.Color.G - left.Color.G;
            int b_diff = right.Color.B - left.Color.B;

            return Color.FromRgb((byte)(left.Color.R + r_diff * value_frac),
                                    (byte)(left.Color.G + g_diff * value_frac),
                                    (byte)(left.Color.B + b_diff * value_frac));
        }

        static double[] GenerateTargetValues(AdaptiveGradient gradient, uint numSamples)
        {
            double leftAnchor = gradient.First().Anchor.AnchorValue;
            double rightAnchor = gradient.Last().Anchor.AnchorValue;
            double increment = (rightAnchor - leftAnchor) / numSamples;

            double[] targetValues = new double[numSamples];

            //generate sample sets. Note that each block is represented by the max value in the block. 
            for (int i = 1; i <= numSamples; i++)
            {
                targetValues[i - 1] = leftAnchor + increment * i;
            }

            return targetValues;
        }
    }
}
