using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ElecNetKit.Util;
using System.Runtime.Serialization;
using System.Numerics;
using ElecNetKit.NetworkModelling.Phasing;
using ElecNetKit.Convenience;

namespace ElecNetKit.NetworkModelling
{
    /// <summary>
    /// A kVA-absorbing load on the network.
    /// </summary>
    [Serializable]
    public class Load : PowerConversionElement
    {
        /// <summary>
        /// The total kVA absorbed by the load. A positive imaginary quantity
        /// corresponds to a lagging power factor, a negative imaginary
        /// quantity corresponds to a leading power factor.
        /// </summary>
        /// <seealso cref="ActualKVAPhased"/>
        public Complex ActualKVA
        {
            set
            {
                if (ActualKVAPhased.Count != 1)
                    throw new NotSupportedException("Can't set ActualKVA for a multi-phase load. Use ActualKVAPhased instead.");
                ActualKVAPhased[ActualKVAPhased.Keys.Single()] = value;
            }
            get { return ActualKVAPhased.Values.Sum(); }
        }

        /// <summary>
        /// The kVA absorbed by the load, by phase. A positive imaginary quantity
        /// corresponds to a lagging power factor, a negative imaginary
        /// quantity corresponds to a leading power factor.
        /// </summary>
        public Phased<Complex> ActualKVAPhased { private set; get; }
                
        /// <summary>
        /// Instantiates a new <see cref="Load"/>. This constructor is
        /// for building balanced networks only.
        /// </summary>
        /// <param name="ID">The ID of the load.</param>
        /// <param name="ActualKVA">The total kVA absorbed by the load.</param>
        /// <param name="NumPhases">The number of phases to split <paramref name="ActualKVA"/> between.</param>
        public Load(String ID, Complex ActualKVA, int NumPhases = 3)
        {
            this.ID = ID;
            this.ActualKVAPhased = new PhasedValues<Complex>();
            foreach (int phase in Enumerable.Range(1, NumPhases))
                ActualKVAPhased[phase] = ActualKVA / NumPhases;
        }

        /// <summary>
        /// Instantiates a new <see cref="Load"/>. This constructor is
        /// for building arbitrarily-phased networks.
        /// </summary>
        /// <param name="ID">The ID of the load.</param>
        /// <param name="ActualKVAPhased">The phased kVA absorbed by the load.</param>
        public Load(String ID, Phased<Complex> ActualKVAPhased)
        {
            this.ID = ID;
            this.ActualKVAPhased = ActualKVAPhased;
        }
    }
}
