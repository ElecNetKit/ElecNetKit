using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ElecNetKit.Experimentation;
using System.IO;
using System.ComponentModel.Composition;

namespace ElecNetKit.Experimentation
{
    /// <summary>
    /// Chains multiple experimentors together. Each sub-experimentor is given
    /// the same initial input network, and should provide commands that will be
    /// run on the network in the order given by <see cref="Experimentors"/>.
    /// </summary>
    public class ChainExperimentor : IExperimentor
    {
        /// <summary>
        /// A list of sub-experiments. Sub-experiments will be executed in order.
        /// </summary>
        public List<IExperimentor> Experimentors { protected set; get; }

        /// <summary>
        /// Instantiates a new <see cref="ChainExperimentor"/> with an empty experiment chain.
        /// </summary>
        public ChainExperimentor()
        {
            Experimentors = new List<IExperimentor>();
        }

        /// <summary>
        /// Obtains simulator-specific experiment commands pertaining to each
        /// <see cref="IExperimentor"/> in order and aggregates them into a
        /// single experiment.
        /// </summary>
        /// <param name="Network">The network to experiment upon.</param>
        /// <returns>A list of simulator-specific experiment commands.</returns>
        public List<string> Experiment(NetworkModelling.NetworkModel Network)
        {
            return Experimentors.Aggregate(new List<String>(), (lst, exp) => { lst.AddRange(exp.Experiment(Network)); return lst; });
        }
    }
}
