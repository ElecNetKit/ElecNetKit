using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.Graphing.Util
{
    
    /// <summary>
    /// Provides axis spacing with 'convenient' numbers, for given dataset characteristics.
    /// </summary>
    /// <remarks>
    /// Based in part off the work from http://search.cpan.org/dist/Chart-Math-Axis/lib/Chart/Math/Axis.pm.
    /// </remarks>
    public class MagicGridSpacer
    {
        /// <summary>
        /// The Minimum value of the dataset.
        /// </summary>
        public double MinValue { set; get; }

        /// <summary>
        /// The Maximum value of the data set.
        /// </summary>
        public double MaxValue { set; get; }

        /// <summary>
        /// The maximum number of lines / ticks etc that should be on the axis.
        /// </summary>
        public uint MaxLines { set; get; }

        /// <summary>
        /// Builds a set of <see cref="MagicGridSpacings"/> based upon the provided inputs.
        /// </summary>
        /// <returns>A set of <see cref="MagicGridSpacings"/>.</returns>
        public MagicGridSpacings GetGridSpacings()
        {
            if (MinValue == MaxValue)
            {
                return new MagicGridSpacings(MinValue - 1, 2 / MaxLines, MinValue + 1);
            }

            //find largest order of magnitude.
            int largestOrderOfMag = Math.Max(OrderOfMagnitude(MinValue),
                                                OrderOfMagnitude(MaxValue));

            doubleME interval = new doubleME(largestOrderOfMag + 1, 1);

            double upperLimit = 0, lowerLimit = 0;
            int loopCount = 0;
            while (true)
            {
                var nextInterval = ShrinkInterval(interval);

                var nextUpperLimit = RoundUpTo(MaxValue, nextInterval.theDouble);
                var nextLowerLimit = RoundDownTo(MinValue, nextInterval.theDouble);

                var numNextIntervalsInRange = (nextUpperLimit - nextLowerLimit) / nextInterval.theDouble;

                if (numNextIntervalsInRange > MaxLines)
                {
                    //wrap it up.
                    if (loopCount == 0)
                    {
                        interval = new doubleME(interval.Exponent + 1, interval.Mantissa);
                        continue;
                    }
                    return new MagicGridSpacings(lowerLimit, interval.theDouble, upperLimit);
                }

                //tighten it all up.
                interval = nextInterval;
                upperLimit = nextUpperLimit;
                lowerLimit = nextLowerLimit;

                if (++loopCount > 100)
                    throw new Exception();
            }
        }

        private doubleME ShrinkInterval(doubleME interval)
        {
            // go from 5 --> 2 --> 1 --> .5 --> .2 --> .1 etc
            switch (interval.Mantissa)
            {
                case 5:
                    return new doubleME(interval.Exponent, 2);
                case 2:
                    return new doubleME(interval.Exponent, 1);
                case 1:
                    return new doubleME(interval.Exponent - 1, 5);
            }
            throw new Exception();
        }

        private double RoundUpTo(double value, double roundToMultipleOf)
        {
            return Math.Ceiling(value / roundToMultipleOf) * roundToMultipleOf;
        }

        private double RoundDownTo(double value, double roundToMultipleOf)
        {
            return Math.Floor(value / roundToMultipleOf) * roundToMultipleOf;
        }

        private int OrderOfMagnitude(double value)
        {
            if (value == 0)
                return int.MinValue;
            return (int)(Math.Log(Math.Abs(value)));
        }
    }

    /// <summary>
    /// A set of ticks that characterize an axis on a graph.
    /// </summary>
    [Serializable]
    public class MagicGridSpacings
    {
        /// <summary>
        /// The upper bound of the axis.
        /// </summary>
        public double UpperLimit { set; get; }
        /// <summary>
        /// The lower bound of the axis.
        /// </summary>
        public double LowerLimit { set; get; }
        /// <summary>
        /// The interval between ticks on the axis.
        /// </summary>
        public double Interval { set; get; }

        /// <summary>
        /// Instantiates a new <see cref="MagicGridSpacings"/>.
        /// </summary>
        /// <param name="LowerLimit">The lower bound of the axis.</param>
        /// <param name="Interval">The interval between ticks on the axis.</param>
        /// <param name="UpperLimit">The upper bound of the axis.</param>
        public MagicGridSpacings(double LowerLimit, double Interval, double UpperLimit)
        {
            this.UpperLimit = UpperLimit;
            this.Interval = Interval;
            this.LowerLimit = LowerLimit;
        }

        /// <summary>
        /// Constructs a set of ticks between <see cref="LowerLimit"/> and <see cref="UpperLimit"/>
        /// and spaced by <see cref="Interval"/>.
        /// </summary>
        /// <returns></returns>
        public double[] GetTicks()
        {
            double[] ticks = new double[(int)Math.Round((UpperLimit - LowerLimit) / Interval) + 1];
            for (int i = 0; i < ticks.Length; i++)
            {
                ticks[i] = LowerLimit + Interval * i;
            }
            return ticks;
        }
    }

    /// <summary>
    /// Support class for building <see cref="double"/>s in an Exponent-Mantissa
    /// format.
    /// </summary>
    class doubleME
    {
        public int Exponent { private set; get; }
        public int Mantissa {private set; get;}
        public double theDouble { private set; get; }

        public doubleME(int Exponent, int Mantissa)
        {
            this.Exponent = Exponent;
            this.Mantissa = Mantissa;
            this.theDouble = Math.Pow(10, Exponent) * Mantissa;
        }

    }
}
