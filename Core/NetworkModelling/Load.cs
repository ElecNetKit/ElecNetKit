using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ElecNetKit.Util;using System.Runtime.Serialization;
using System.Numerics;
using ElecNetKit.NetworkModelling.Phasing;

namespace ElecNetKit.NetworkModelling
{
    /// <summary>
    /// A kVA-absorbing load on the network.
    /// </summary>
    [Serializable]
    public class Load : PowerConversionElement
    {
        /// <summary>
        /// The kVA absorbed by the load. A positive imaginary quantity
        /// corresponds to a lagging power factor, a negative imaginary
        /// quantity corresponds to a leading power factor.
        /// </summary>
        public Complex ActualKVA { set { ActualKVAPhased[1] = value; } get { return ActualKVAPhased[1]; } }

        public Phased<Complex> ActualKVAPhased { private set; get; }
                
        /// <summary>
        /// Instantiates a new <see cref="Load"/>.
        /// </summary>
        /// <param name="ID">The ID of the load.</param>
        /// <param name="ActualKVA">The kVA absorbed by the load.</param>
        public Load(String ID, Complex ActualKVA)
        {
            this.ID = ID;
            this.ActualKVAPhased = new PhasedValues<Complex>();
            this.ActualKVA = ActualKVA;
        }

        public Load(String ID, Phased<Complex> ActualKVAPhased)
        {
            this.ID = ID;
            this.ActualKVAPhased = ActualKVAPhased;
        }
    }
}
