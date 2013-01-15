using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.NetworkModelling
{
    public abstract class PowerConversionElement : NetworkElement
    {
        /// <summary>
        /// Connects a phase of a power conversion element between two phases of the same bus.
        /// </summary>
        /// <param name="thisPhase"></param>
        /// <param name="connectTo"></param>
        /// <param name="connectToPhasePrimary"></param>
        /// <param name="connectToPhaseSecondary"></param>
        public void Connect(int thisPhase, Bus connectTo, int connectToPhasePrimary, int connectToPhaseSecondary)
        {
            this.ConnectBetween(thisPhase, connectTo, connectToPhasePrimary, connectTo, connectToPhaseSecondary);
        }

        public void ConnectWye(Bus connectTo, params int[] phases)
        {
            ConnectWye(connectTo, phases, phases);
        }

        public void ConnectWye(Bus connectTo, int[] pcElementPhases, int[] busPhases)
        {
            foreach (var phasePair in pcElementPhases.Zip(busPhases,(pcElementPhase,busPhase) => new {pcElementPhase,busPhase}))
            {
                Connect(phasePair.pcElementPhase, connectTo, phasePair.busPhase, 0);
            }
        }

        public void ConnectDelta(Bus connectTo, params int[] phases)
        {
            ConnectDelta(connectTo, phases, phases);
        }

        public void ConnectDelta(Bus connectedTo, int[] pcElementPhases, int[] busPhases)
        {
            var busPhasePairs = busPhases.Zip(busPhases.Skip(1).Concat(busPhases.Take(1)), (phase1,phase2) => new {phase1,phase2});
            var phaseMappings = pcElementPhases.Zip(busPhasePairs, (pcPhase, busPhasePair) => new {pcPhase,busPhasePair});
            foreach (var phaseMap in phaseMappings)
            {
                this.ConnectBetween(phaseMap.pcPhase, connectedTo, phaseMap.busPhasePair.phase1, connectedTo, phaseMap.busPhasePair.phase2);
            }
        }

        public void Connect(Bus connectTo)
        {
            ConnectWye(connectTo, 1, 2, 3);
        }
    }
}
