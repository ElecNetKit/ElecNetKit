using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.NetworkModelling
{
    /// <summary>
    /// A <see cref="NetworkElement"/> that converts power from one form to another. Think
    /// <see cref="Load"/>s, <see cref="Generator"/>s, etc.
    /// </summary>
    /// <remarks>As far as the connection model goes, <see cref="PowerConversionElement"/>s are
    /// connected to a single <see cref="Bus"/>, with each <see cref="PowerConversionElement"/> phase being connected between <see cref="Bus"/>
    /// phases. This should be viewed in contrast with <seealso cref="PowerDeliveryElement"/>.</remarks>
    [Serializable]
    public abstract class PowerConversionElement : NetworkElement
    {
        /// <summary>
        /// Connects a phase of a power conversion element between two phases of a single <see cref="Bus"/>.
        /// </summary>
        /// <param name="thisPhase">The phase of this <see cref="PowerConversionElement"/> to connect between phases of the bus.</param>
        /// <param name="connectTo">The bus to connect <paramref name="thisPhase"/> of this <see cref="PowerConversionElement"/> to.</param>
        /// <param name="connectToPhasePrimary">The primary phase of the bus to connect to. Should be an active phase.</param>
        public void Connect(int thisPhase, Bus connectTo, int connectToPhasePrimary)
        {
            base.Connect(thisPhase, connectTo, connectToPhasePrimary);
        }

        /// <summary>
        /// Connects this power conversion element in Wye to <paramref name="connectTo"/>. Each phase of
        /// the <see cref="PowerConversionElement"/> specified in <paramref name="phases"/> will be connected
        /// to the corresponding active phase.
        /// </summary>
        /// <param name="connectTo">The <see cref="Bus"/> that the <see cref="PowerConversionElement"/> should connect to.</param>
        /// <param name="phases">The phases of the <see cref="PowerConversionElement"/> and the active phases of the <see cref="Bus"/>
        /// to connect on.</param>
        /// <overloads>There are multiple overloads for this method:</overloads>
        public void ConnectWye(Bus connectTo, params int[] phases)
        {
            if (phases.Length == 0)
                phases = new[] { 1, 2, 3 };
            ConnectWye(connectTo, phases, phases);
        }

        /// <summary>
        /// Connects this power conversion element in Wye to <paramref name="connectTo"/>. Each phase of the
        /// <see cref="PowerConversionElement"/>in <paramref name="pcElementPhases"/> is connected to the phase of the same index in
        /// <paramref name="busPhases"/>.
        /// </summary>
        /// <param name="connectTo">The <see cref="Bus"/> to connect this <see cref="PowerConversionElement"/> to.</param>
        /// <param name="pcElementPhases">The phases of this element that should be connected.</param>
        /// <param name="busPhases">The phases of <paramref name="connectTo"/> to connect to.</param>
        public void ConnectWye(Bus connectTo, IEnumerable<int> pcElementPhases, IEnumerable<int> busPhases)
        {
            foreach (var phasePair in pcElementPhases.Zip(busPhases,(pcElementPhase,busPhase) => new {pcElementPhase,busPhase}))
            {
                Connect(phasePair.pcElementPhase, connectTo, phasePair.busPhase);
            }
        }

        /// <summary>
        /// Connects this <see cref="PowerConversionElement"/> to a <see cref="Bus"/>.
        /// This is the connection method for three-phase balanced networks. Use
        /// <see cref="O:ElecNetKit.NetworkModelling.PowerConversionElement.ConnectWye" /> and <see cref="Connect(int,Bus,int)"/>
        /// for arbitrarily-phased networks.
        /// </summary>
        /// <param name="connectTo">The <see cref="Bus"/> to connect to.</param>
        /// <overloads>There are multiple overloads for this method:</overloads>
        public void Connect(Bus connectTo)
        {
            ConnectWye(connectTo, 1, 2, 3);
        }
    }
}
