using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.NetworkModelling
{
    /// <summary>
    /// A <see cref="NetworkElement"/> that delivers power to other <see cref="NetworkElement"/>s. Think
    /// <see cref="Line"/>s, Transformers, etc.
    /// </summary>
    /// <remarks>As far as the connection model goes, <see cref="PowerDeliveryElement"/>s are
    /// connected to between different <see cref="Bus"/>es. This should be viewed in contrast with
    /// <seealso cref="PowerDeliveryElement"/>.</remarks>
    [Serializable]
    public abstract class PowerDeliveryElement : NetworkElement
    {
        /// <summary>
        /// Connects <paramref name="thisPhase"/> of this <see cref="PowerDeliveryElement"/> between
        /// <paramref name="bus1Phase"/> of <paramref name="bus1"/> and <paramref name="bus2Phase"/>
        /// of <paramref name="bus2"/>.
        /// </summary>
        /// <param name="thisPhase">The phase of this element to connect.</param>
        /// <param name="bus1">The <see cref="Bus"/> to connect on one side of this <see cref="PowerDeliveryElement"/>.</param>
        /// <param name="bus1Phase">The phase of <paramref name="bus1"/> to connect to.</param>
        /// <param name="bus2">The <see cref="Bus"/> to connect on the other side of this <see cref="PowerDeliveryElement"/>.</param>
        /// <param name="bus2Phase">The phase of <paramref name="bus2"/> to connect to.</param>
        public void Connect(int thisPhase, Bus bus1, int bus1Phase, Bus bus2, int bus2Phase)
        {
            NetworkElement.ConnectBetween(this, thisPhase, bus1, bus1Phase, bus2, bus2Phase);
        }

        /// <summary>
        /// Connects the <see cref="PowerDeliveryElement"/> on the phases in <paramref name="bus1AndLinePhases"/>
        /// to the same phases on <paramref name="bus1"/>, and to the correspondingly-indexed phases <paramref name="bus2Phases"/>
        /// on <paramref name="bus2"/>.
        /// </summary>
        /// <example>
        /// The following code connects the <see cref="PowerDeliveryElement"/> to <paramref name="bus1"/>
        /// and <paramref name="bus2"/>, with the following phasing:
        /// <code>
        /// // key: line phase -> bus1 phase, bus2 phase
        /// // 1 -> 1, 2
        /// // 2 -> 2, 3
        /// // 3 -> 3, 1
        /// myPowerDeliveryElement.Connect(bus1, new[] {1,2,3}, bus2, new[] {2,3,1});
        /// </code></example>
        /// <param name="bus1">The first <see cref="Bus"/> to connect to. Shares common phases with the <see cref="PowerDeliveryElement"/>.</param>
        /// <param name="bus1AndLinePhases">The common phases to connect between the <see cref="PowerDeliveryElement"/> and the <see cref="Bus"/>.</param>
        /// <param name="bus2">The second <see cref="Bus"/> to connect to.</param>
        /// <param name="bus2Phases">Phase connections for <paramref name="bus2"/>.</param>
        public void Connect(Bus bus1, IEnumerable<int> bus1AndLinePhases, Bus bus2, IEnumerable<int> bus2Phases)
        {
            foreach (var phasePair in bus1AndLinePhases.Zip(bus2Phases, (phase1, phase2) => new { Bus1AndLinePhase = phase1, Bus2Phase = phase2 }))
            {
                NetworkElement.ConnectBetween(this,
                    phasePair.Bus1AndLinePhase,
                    bus1,
                    phasePair.Bus1AndLinePhase,
                    bus2,
                    phasePair.Bus2Phase
                    );
            }

        }

        /// <summary>
        /// Connects this <see cref="PowerDeliveryElement"/> between <paramref name="bus1"/>
        /// and <paramref name="bus2"/> on matching <paramref name="phases"/>.
        /// </summary>
        /// <param name="bus1">The first <see cref="Bus"/> to connect to.</param>
        /// <param name="bus2">The second <see cref="Bus"/> to connect to.</param>
        /// <param name="phases">The phases to connect the <see cref="PowerDeliveryElement"/>
        /// to the <see cref="Bus"/>es on.</param>
        public void Connect(Bus bus1, Bus bus2, params int[] phases)
        {
            foreach (var phase in phases)
            {
                Connect(phase, bus1, phase, bus2, phase);
            }
        }

        /// <summary>
        /// Connects this <see cref="PowerDeliveryElement"/> between two <see cref="Bus"/>es,
        /// on phases 1,2,3 and neutral (Phase 0).
        /// </summary>
        ///<param name="bus1">The <see cref="Bus"/> to connect on one side of this <see cref="PowerDeliveryElement"/>.</param>
        /// <param name="bus2">The <see cref="Bus"/> to connect on the other side of this <see cref="PowerDeliveryElement"/>.</param>
        public void Connect3PhaseN(Bus bus1, Bus bus2)
        {
            Connect(bus1, bus2, 0, 1, 2, 3);
        }

        /// <summary>
        /// Connects this <see cref="PowerDeliveryElement"/> between two <see cref="Bus"/>es,
        /// on phases 1,2,3.
        /// </summary>
        ///<param name="bus1">The <see cref="Bus"/> to connect on one side of this <see cref="PowerDeliveryElement"/>.</param>
        /// <param name="bus2">The <see cref="Bus"/> to connect on the other side of this <see cref="PowerDeliveryElement"/>.</param>
        public void Connect3Phase(Bus bus1, Bus bus2)
        {
            Connect(bus1, bus2, 1, 2, 3);
        }

        /// <summary>
        /// Connects this <see cref="PowerDeliveryElement"/> between two <see cref="Bus"/>es. This method is for
        /// building three-phase balanced networks. Use
        /// <see cref="Connect3Phase"/>, <see cref="Connect3PhaseN"/> and <see cref="Connect(int,Bus,int,Bus,int)"/>
        /// for arbitrarily-phased networks.
        /// </summary>
        ///<param name="bus1">The <see cref="Bus"/> to connect on one side of this <see cref="PowerDeliveryElement"/>.</param>
        /// <param name="bus2">The <see cref="Bus"/> to connect on the other side of this <see cref="PowerDeliveryElement"/>.</param>
        public void Connect(Bus bus1, Bus bus2)
        {
            Connect3PhaseN(bus1, bus2);
        }
    }
}
