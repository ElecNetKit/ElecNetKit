using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ElecNetKit.Util;using System.Runtime.Serialization;
using System.Numerics;

namespace ElecNetKit.NetworkModelling
{
    /// <summary>
    /// A kVA-absorbing load on the network.
    /// </summary>
    [Serializable]
    public class Load : NetworkElement
    {
        /// <summary>
        /// The kVA absorbed by the load. A positive imaginary quantity
        /// corresponds to a lagging power factor, a negative imaginary
        /// quantity corresponds to a leading power factor.
        /// </summary>
        public Complex ActualKVA { protected set; get; }
        
        /// <summary>
        /// Instantiates a new <see cref="Load"/>.
        /// </summary>
        /// <param name="ID">The ID of the load.</param>
        /// <param name="ActualKVA">The kVA absorbed by the load.</param>
        public Load(String ID, Complex ActualKVA)
        {
            this.ID = ID;
            this.ActualKVA = ActualKVA;
        }
    }
}
