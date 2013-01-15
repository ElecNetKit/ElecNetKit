using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.NetworkModelling
{
    public abstract class PowerDeliveryElement : NetworkElement
    {
        public void Connect(int thisPhase, Bus bus1, int bus1Phase, Bus bus2, int bus2Phase)
        {
            NetworkElement.ConnectBetween(this, thisPhase, bus1, bus1Phase, bus2, bus2Phase);
        }

        public void Connect3PhaseN(Bus bus1, Bus bus2)
        {
            Connect(0, bus1, 0, bus2,0);
            Connect3Phase(bus1, bus2);
        }

        public void Connect3Phase(Bus bus1, Bus bus2)
        {
            for (int i = 1; i <= 3; i++)
                Connect(i, bus1, i, bus2, i);
        }

        public void Connect(Bus connectSideA, Bus connectSideB)
        {
            Connect3PhaseN(connectSideA, connectSideB);
        }
    }
}
