using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ElecNetKit.NetworkModelling;


namespace ElecNetKit.Transform
{
    /// <summary>
    /// Classes that implement <see cref="IResultsTransform"/> are used for
    /// observing and modifying the electrical network model pre- and post- 
    /// experiment.
    /// They meaningfully reinterpret the results obtained by experimentation
    /// in order to be better visualised or stored.
    /// </summary>
    /// <example>For an example implementation, see
    /// <see cref="DifferenceTransform"/>.</example>
    public interface IResultsTransform
    {
        /// <summary>
        /// Called by a <see cref="ElecNetKit.Simulator.NetworkController"/>
        /// before experiment commands are run, providing the
        /// <see cref="IResultsTransform"/> with a chance to measure or
        /// manipulate network parameters before experiment.
        /// </summary>
        /// <param name="Network">The un-experimented network.</param>
        void PreExperimentHook(NetworkModel Network);

        /// <summary>
        /// Called by a <see cref="ElecNetKit.Simulator.NetworkController"/>
        /// after experiment commands are run. Implementors should meaningfully
        /// measure or modify the <see cref="NetworkModel"/> to enable better
        /// analysis of results.
        /// </summary>
        /// <param name="Network">The electrical network model, post-experiment.
        /// </param>
        void PostExperimentHook(NetworkModel Network);
    }
}
