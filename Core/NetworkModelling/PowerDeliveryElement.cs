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
        /// Connects this <see cref="PowerDeliveryElement"/> between two <see cref="Bus"/>es,
        /// on phases 1,2,3 and neutral (Phase 0).
        /// </summary>
        ///<param name="bus1">The <see cref="Bus"/> to connect on one side of this <see cref="PowerDeliveryElement"/>.</param>
        /// <param name="bus2">The <see cref="Bus"/> to connect on the other side of this <see cref="PowerDeliveryElement"/>.</param>
        public void Connect3PhaseN(Bus bus1, Bus bus2)
        {
            Connect(0, bus1, 0, bus2,0);
            Connect3Phase(bus1, bus2);
        }

        /// <summary>
        /// Connects this <see cref="PowerDeliveryElement"/> between two <see cref="Bus"/>es,
        /// on phases 1,2,3.
        /// </summary>
        ///<param name="bus1">The <see cref="Bus"/> to connect on one side of this <see cref="PowerDeliveryElement"/>.</param>
        /// <param name="bus2">The <see cref="Bus"/> to connect on the other side of this <see cref="PowerDeliveryElement"/>.</param>
        public void Connect3Phase(Bus bus1, Bus bus2)
        {
            for (int i = 1; i <= 3; i++)
                Connect(i, bus1, i, bus2, i);
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
