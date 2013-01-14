using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElecNetKit.Util
{
    /// <summary>
    /// Automatically determines the minimum and maximum values
    /// for a data set. Manual limits (<see cref="LimitMin"/> and
    /// <see cref="LimitMax"/>) can also be specified, and then values can be
    /// scaled from the Auto-space to the Manual-space, and vice-versa.
    /// </summary>
    public class Limits
    {
        /// <summary>
        /// Instantiates a new <see cref="Limits"/>.
        /// </summary>
        public Limits()
        {
            AutoDataReset();
        }

        /// <summary>
        /// How far between the Minimum and Maximum a value is, as a fraction.
        /// </summary>
        /// <param name="value">The value in question.</param>
        /// <returns>A fractional value indicating how far between the Minimum and Maximum the value is.</returns>
        private double PercentWayThroughAutoData(double value)
        {
            if (AutoMax == AutoMin)
                return value < AutoMax ? 0 : 1;

            return (value - AutoMin) / (AutoMax - AutoMin);
        }

        /// <summary>
        /// Scales a value from the Auto-limits space to the manual-limits space.
        /// </summary>
        /// <param name="value">A value in the auto-limits space, in the interval [<see cref="AutoMin"/>,<see cref="AutoMax"/>].</param>
        /// <returns>A value the same percentage of the way through the interval [<see cref="LimitMin"/>,<see cref="LimitMax"/>].</returns>
        public double ValueScaledToLimits(double value)
        {
            return PercentWayThroughAutoData(value) * (LimitMax - LimitMin) + LimitMin;
        }

        /// <summary>
        /// Processes a single data point.
        /// </summary>
        /// <param name="value">A piece of data to consider into the limits.</param>
        public void ProcessData(double value)
        {
            if (value > AutoMax)
                AutoMax = value;
            if (value < AutoMin)
                AutoMin = value;
            Count++;
        }

        /// <summary>
        /// Processes a set of data.
        /// </summary>
        /// <param name="values">A set of data to consider into the limits.</param>
        public void ProcessData(IEnumerable<double> values)
        {
            AutoMax = Math.Max(AutoMax, values.Max());
            AutoMin = Math.Min(AutoMin, values.Min());
            Count += values.Count();
        }

        /// <summary>
        /// Resets the Auto limits.
        /// </summary>
        public void AutoDataReset()
        {
            AutoMin = double.PositiveInfinity;
            AutoMax = double.NegativeInfinity;
            Count = 0;
        }

        /// <summary>
        /// Returns the automatically determined minimum value of the processed data.
        /// </summary>
        public double AutoMin { private set; get; }

        /// <summary>
        /// Returns the automatically determined maximum value of the processed data.
        /// </summary>
        public double AutoMax { private set; get; }

        /// <summary>
        /// A custom minimum for scaling values in the auto-space to a custom space.
        /// </summary>
        public double LimitMin { set; get; }

        /// <summary>
        /// A custom maximum for scaling values in the auto-space to a custom space.
        /// </summary>
        public double LimitMax { set; get; }

        /// <summary>
        /// Returns the number of data points processed by the <see cref="Limits"/>.
        /// </summary>
        public double Count { private set; get; }

        /// <summary>
        /// Unscales a value from the manual/custom space to the auto-space
        /// </summary>
        /// <param name="p">A value in the manual/custom space, in the interval [<see cref="LimitMin"/>,<see cref="LimitMax"/>]</param>
        /// <returns>A value the same percentage of the way through the interval [<see cref="AutoMin"/>,<see cref="AutoMax"/>].</returns>
        public double ValueUnscaledFromLimits(double p)
        {
            return (p-LimitMin)/(LimitMax-LimitMin) * (AutoMax-AutoMin) + AutoMin;
        }
    }
}
