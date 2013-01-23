using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using ElecNetKit.NetworkModelling;

namespace ElecNetKit.Simulator
{
    /// <summary>
    /// Defines an electrical network simulator. Simulators must be able to run
    /// textual commands and return network models.
    /// </summary>
    /// <example>
    /// For an example implemention, see the <c>ElecNetKit.Engines.OpenDSS</c> assembly.</example>
    public interface ISimulator
    {
        /// <summary>
        /// Run a single command through the simulator text interface.
        /// </summary>
        /// <param name="command">The command to run.</param>
        void RunCommand(String command);

        /// <summary>
        /// Signals that the <see cref="ISimulator"/> should load a network
        /// model from <paramref name="filename"/>, ready to be returned as a
        /// <see cref="NetworkModel"/> or experimented upon.
        /// </summary>
        /// <param name="filename">The filename of the network to prepare.</param>
        void PrepareNetwork(String filename);

        /// <summary>
        /// Finalises all experiment commands, solves and retrieves all results
        /// and returns a <see cref="NetworkModel"/>, characterising the electrical 
        /// network.
        /// </summary>
        /// <returns>The complete electrical network model.</returns>
        NetworkModel GetNetworkModel();
    }
}
