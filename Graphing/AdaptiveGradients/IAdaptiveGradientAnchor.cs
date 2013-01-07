using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElecNetKit.Graphing.AdaptiveGradients
{
    /// <summary>
    /// An Adaptive Gradient Anchor. Adaptive Gradients use these to determine
    /// postition of Gradient Stops. <see cref="IAdaptiveGradientAnchor"/>s 
    /// process a data set and spit out a specific anchor value.
    /// Can be reset and used again.
    /// </summary>
    public interface IAdaptiveGradientAnchor
    {
        /// <summary>
        /// Process a single item of data. Do whatever you need to do with it
        /// To spit out a dataset-adapted value.
        /// </summary>
        /// <param name="data">The data to process.</param>
        void ProcessData(double data);

        /// <summary>
        /// The adapted Anchor value, after all the data has been processed.
        /// </summary>
        double AnchorValue {get;}

        /// <summary>
        /// Prime the Anchor to work on a new set of data.
        /// </summary>
        void Reset();
    }
}
