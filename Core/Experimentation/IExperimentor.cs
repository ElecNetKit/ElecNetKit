using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ElecNetKit.NetworkModelling;

namespace ElecNetKit.Experimentation
{
    /// <summary>
    /// Defines an interface for conducting experiments on the network.
    /// </summary>
    /// <remarks>
    /// When the <see cref="ElecNetKit.Simulator.NetworkController"/> runs a simulation, it calls <see cref="IExperimentor.Experiment"/>,
    /// passing a <see cref="NetworkModel"/> and expecting a list of simulator-specific commands to manipulate the network.
    /// Results are then fed through to a Results Core.Transform or a graph.
    /// </remarks>
    public interface IExperimentor
    {
        /// <summary>
        /// Examines the network model and issues commands in order to
        /// manipulate or experiment upon the network model.
        /// Called by <see cref="ElecNetKit.Simulator.NetworkController.Execute"/>.
        /// </summary>
        /// <param name="Network">A network to experiment on.</param>
        /// <returns>A list of simulator-specific commands to be fed into the
        /// simulator by the associated <see cref="ElecNetKit.Simulator.NetworkController"/>.</returns>
        List<String> Experiment(NetworkModel Network);
    }
}
