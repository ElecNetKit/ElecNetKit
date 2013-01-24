using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ElecNetKit.NetworkModelling;
using ElecNetKit.Util;
using ElecNetKit.Transform;
using System.Numerics;
using System.ComponentModel.Composition;

namespace ElecNetKit.Transform
{
    /// <summary>
    /// Measures the change in voltage magnitudes and phases between a pre- and
    /// post- experiment network. Saves the computed differences back into the
    /// final <see cref="NetworkModel"/> for processing, traversing or graphing.
    /// </summary>
    [Export(typeof(IResultsTransform))]
    public class DifferenceTransform : IResultsTransform
    {
        Dictionary<string,Complex> oldVoltages;

        /// <summary>
        /// Store original bus voltage magnitudes by BusID in oldVoltages.
        /// See <see cref="IResultsTransform.PreExperimentHook"/> for how this
        /// fits into the system.
        /// </summary>
        /// <param name="Network">The original, unexperimented network.</param>
        public void PreExperimentHook(NetworkModel Network)
        {
            
            oldVoltages = new Dictionary<string, Complex>();
            foreach (KeyValuePair<String, Bus> kvp in Network.Buses)
            {
                Bus bus = kvp.Value;
                oldVoltages.Add(bus.ID, bus.Voltage);
            }
        }

        /// <summary>
        /// Subtracts old bus voltage magnitudes from new bus voltage
        /// magnitudes. Note that this will screw around with the phases
        /// due to subtraction of magnitudes, not of complex vectors.
        /// </summary>
        /// <param name="Network">The network to transform.</param>
        public void PostExperimentHook(NetworkModel Network)
        {
            foreach (KeyValuePair<String, Bus> kvp in Network.Buses)
            {
                Bus bus = kvp.Value;
                if (oldVoltages.ContainsKey(bus.ID))
                {
                    bus.Voltage = Complex.FromPolarCoordinates(bus.Voltage.Magnitude - oldVoltages[bus.ID].Magnitude, 
                        bus.Voltage.Phase - oldVoltages[bus.ID].Phase);
                }
            }
        }
    }
}
