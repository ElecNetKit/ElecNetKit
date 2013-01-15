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
    public abstract class PowerConversionElement : NetworkElement
    {
        /// <summary>
        /// Connects a phase of a power conversion element between two phases of a single <see cref="Bus"/>.
        /// </summary>
        /// <param name="thisPhase">The phase of this <see cref="PowerConversionElement"/> to connect between phases of the bus.</param>
        /// <param name="connectTo">The bus to connect <paramref name="thisPhase"/> of this <see cref="PowerConversionElement"/> to.</param>
        /// <param name="connectToPhasePrimary">The primary phase of the bus to connect to. Should be an active phase.</param>
        /// <param name="connectToPhaseSecondary">The secondary or neutral phase of the bus to connect to.</param>
        public void Connect(int thisPhase, Bus connectTo, int connectToPhasePrimary, int connectToPhaseSecondary)
        {
            this.ConnectBetween(thisPhase, connectTo, connectToPhasePrimary, connectTo, connectToPhaseSecondary);
        }

        /// <summary>
        /// Connects this power conversion element in Wye to <paramref name="connectTo"/>. Each phase of
        /// the <see cref="PowerConversionElement"/> specified in <paramref name="phases"/> will be connected
        /// between the corresponding active phase and neutral (phase 0).
        /// </summary>
        /// <param name="connectTo">The <see cref="Bus"/> that the <see cref="PowerConversionElement"/> should connect to.</param>
        /// <param name="phases">The phases of the <see cref="PowerConversionElement"/> and the active phases of the <see cref="Bus"/>
        /// to connect on.</param>
        public void ConnectWye(Bus connectTo, params int[] phases)
        {
            ConnectWye(connectTo, phases, phases);
        }

        /// <summary>
        /// Connects this power conversion element in Wye to <paramref name="connectTo"/>. Each phase of the
        /// <see cref="PowerConversionElement"/>in <paramref name="pcElementPhases"/> is connected between the phase of the same index in
        /// <paramref name="busPhases"/> and neutral (phase 0).
        /// </summary>
        /// <param name="connectTo">The <see cref="Bus"/> to connect this <see cref="PowerConversionElement"/> to.</param>
        /// <param name="pcElementPhases">The phases of this element that should be connected.</param>
        /// <param name="busPhases">The phases of <paramref name="connectTo"/> to connect to.</param>
        public void ConnectWye(Bus connectTo, IEnumerable<int> pcElementPhases, IEnumerable<int> busPhases)
        {
            foreach (var phasePair in pcElementPhases.Zip(busPhases,(pcElementPhase,busPhase) => new {pcElementPhase,busPhase}))
            {
                Connect(phasePair.pcElementPhase, connectTo, phasePair.busPhase, 0);
            }
        }

        /// <summary>
        /// Connects this power conversion element in Delta to <paramref name="connectTo"/>. Each phase of the <see cref="PowerConversionElement"/>
        /// in <paramref name="phases"/> is connected between the same and the next phase in <paramref name="phases"/>.
        /// </summary>
        /// <example>The following code connects a <see cref="PowerConversionElement"/> in delta
        /// to a <see cref="Bus"/>, with phase 1 connected between bus phase 1 and 2, phase 2 connected between bus phase 2 and 3,
        /// and phase 3 connected between bus phase 3 and 1.
        /// <code>myPowerConversionElement.ConnectDelta(myBus,1,2,3);</code>
        /// </example>
        /// <param name="connectTo">The <see cref="Bus"/> to connect to.</param>
        /// <param name="phases">The phases of the <see cref="PowerConversionElement"/> and <see cref="Bus"/> to connect.</param>
        public void ConnectDelta(Bus connectTo, params int[] phases)
        {
            ConnectDelta(connectTo, phases, phases);
        }

        /// <summary>
        /// Connects this power conversion element in Delta to <paramref name="connectTo"/>. Each phase of the <see cref="PowerConversionElement"/>
        /// in <paramref name="pcElementPhases"/> is connected between the phase of the same index and the phase of the next index in <paramref name="busPhases"/>.
        /// </summary>
        /// <example>The following code connects a <see cref="PowerConversionElement"/> in delta
        /// to a <see cref="Bus"/>, with phase 1 connected between bus phase 1 and 2, phase 2 connected between bus phase 2 and 3,
        /// and phase 3 connected between bus phase 3 and 1.
        /// <code>myPowerConversionElement.ConnectDelta(myBus,new[]{1,2,3}, new[]{1,2,3});</code>
        /// Similarly, for the sake of argument, the following code connects a <see cref="PowerConversionElement"/> in delta
        /// to a <see cref="Bus"/>, with phase 1 connected between bus phase 4 and 5, phase 2 connected between bus phase 5 and 1,
        /// and phase 3 connected between bus phase 1 and 4.
        /// <code>myPowerConversionElement.ConnectDelta(myBus,new[]{1,2,3}, new[]{4,5,1});</code>
        /// </example>
        /// <param name="connectTo">The <see cref="Bus"/> to connect to.</param>
        /// <param name="pcElementPhases">The phases of the <see cref="PowerConversionElement"/> to connect.</param>
        /// <param name="busPhases">The phases of the <see cref="Bus"/> <paramref name="connectTo"/> to connect to in Delta.</param>
        public void ConnectDelta(Bus connectTo, IEnumerable<int> pcElementPhases, IEnumerable<int> busPhases)
        {
            var busPhasePairs = busPhases.Zip(busPhases.Skip(1).Concat(busPhases.Take(1)), (phase1,phase2) => new {phase1,phase2});
            var phaseMappings = pcElementPhases.Zip(busPhasePairs, (pcPhase, busPhasePair) => new {pcPhase,busPhasePair});
            foreach (var phaseMap in phaseMappings)
            {
                this.ConnectBetween(phaseMap.pcPhase, connectTo, phaseMap.busPhasePair.phase1, connectTo, phaseMap.busPhasePair.phase2);
            }
        }

        /// <summary>
        /// Connects this <see cref="PowerConversionElement"/> to a <see cref="Bus"/>.
        /// This is the connection method for three-phase balanced networks. Use
        /// <see cref="O:ConnectWye"/>, <see cref="O:ConnectDelta"/> and <see cref="Connect(int,Bus,int,int)"/>
        /// for arbitrarily-phased networks.
        /// </summary>
        /// <param name="connectTo">The <see cref="Bus"/> to connect to.</param>
        public void Connect(Bus connectTo)
        {
            ConnectWye(connectTo, 1, 2, 3);
        }
    }
}
