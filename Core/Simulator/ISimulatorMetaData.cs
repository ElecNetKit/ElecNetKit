using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElecNetKit.Simulator
{
    /// <summary>
    /// Holds information as to the nature of the simulator.
    /// </summary>
    /// <remarks>
    /// When implementing an <see cref="ISimulator"/> for detection by
    /// ElecNetKit, ensure that you mark the implementing class with
    /// <code>
    /// [Export(typeof(ISimulator))]
    /// [ExportMetadata("Name","(Name of Simulator)")]
    /// </code>
    /// This enables the <see cref="NetworkController"/> to make decisions
    /// on which simulator to use based upon <see cref="NetworkController.SimulatorTarget"/>.
    /// </remarks>
    public interface ISimulatorMetaData
    {
        /// <summary>
        /// A unique string identifying the simulator.
        /// </summary>
        String Name { get; }
    }
}
