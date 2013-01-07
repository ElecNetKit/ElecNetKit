using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElecNetKit.Util
{
    /// <summary>
    /// A Normal Distribution. Generates normally-distributed random data from
    /// a mean and a standard deviation.
    /// </summary>
    [Serializable]
    public class NormalDist
    {
        /// <summary>
        /// The Mean of the normally distributed data (μ).
        /// </summary>
        public double Mean { set; get; }

        /// <summary>
        /// The Standard Deviation of the normally distributed data (σ).
        /// </summary>
        public double StdDev { set; get; }

        /// <summary>
        /// The variance of the normally distributed data (σ²).
        /// </summary>
        public double Variance
        {
            set
            {
                StdDev = Math.Sqrt(value);
            }
            get
            {
                return StdDev * StdDev;
            }
        }

        /// <summary>
        /// Obtains the next random value from the distribution with the given
        /// μ and σ.
        /// </summary>
        /// <returns>A random value from this distribution.</returns>
        public double GetNextValue()
        {
            return Mean + GetZ() * StdDev;
        }

        /// <summary>
        /// Instantiates a new Normal Distribution, with Mean = 0 and Std Dev = 1.
        /// </summary>
        public NormalDist()
        {
            Mean = 0;
            StdDev = 1;
        }

        /// <summary>
        /// Instantiates a new Normal Distribution.
        /// </summary>
        /// <param name="Mean">The Mean (μ) of the distribution.</param>
        /// <param name="StdDev">The Standard Deviation (σ) of the distribution.</param>
        public NormalDist(double Mean, double StdDev)
        {
            this.Mean = Mean;
            this.StdDev = StdDev;
        }

        /// <summary>
        /// Instantiates a new Normal Distribution. Fits a Normal Distribution based on an array of data.
        /// </summary>
        /// <param name="data">The data to fit the Normal Distribution to.</param>
        public NormalDist(IEnumerable<double> data)
        {
            this.Mean = data.Average();
            this.StdDev = Math.Sqrt(data.Sum(x => (x - Mean) * (x - Mean)) / (data.Count() - 1));
        }

        // Used for generating random numbers.
        private static Random rand;

        static NormalDist()
        {
            rand = new Random();
        }
        
        //holds the next random data (as the Box
        private static double r2;
        private static bool haveNextRand = false;

        /// <summary>
        /// Generates a random value for the N(0,1) distribution.
        /// </summary>
        /// <returns>A random value from the N(0,1) distribution.</returns>
        public static double GetZ()
        {
            //Uses the Box-Mueller transform as documented here http://mathworld.wolfram.com/Box-MullerTransformation.html
            // Also caches and recycles one generated value for efficiency.
            if (haveNextRand)
            {
                haveNextRand = false;
                return r2;
            }
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double mult = Math.Sqrt(-2.0 * Math.Log(u1));
            double r1 =  mult * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            r2 = mult * Math.Cos(2.0 * Math.PI * u2);
            haveNextRand = true;
            return r1;
        }

        /// <summary>
        /// Converts to String.
        /// </summary>
        /// <returns>A string representation of the normal distribution.</returns>
        public override string ToString()
        {
            return String.Format("N(μ={0:F3},σ={1:F3})", Mean, StdDev);
        }
    }
}
