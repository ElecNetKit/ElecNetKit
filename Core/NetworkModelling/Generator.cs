using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ElecNetKit.Util;
using System.Runtime.Serialization;
using System.Numerics;

namespace ElecNetKit.NetworkModelling
{
    /// <summary>
    /// An electrical network element that injects power back
    /// in to the network.
    /// </summary>
     [Serializable]
    public class Generator : NetworkElement
    {
        /// <summary>
        /// The kVA that the <see cref="Generator"/> injects into the network.
        /// the real component represents the real power (kW) and
        /// the imaginary component represents the injected reactive (imaginary)
        /// power (kVAr).
        /// </summary>
        public Complex Generation { protected set; get; }

        public Phased<Complex> GenerationPhased { protected set; get; }

        /// <summary>
        /// Instantiates a new <see cref="Generator"/>.
        /// </summary>
        /// <param name="ID">The ID of the generator. Must be unique among
        /// generators, but not among network elements.</param>
        /// <param name="Generation">The absolute generation (in kVA) of
        /// the generator.</param>
        public Generator(String ID, Complex Generation)
        {
            this.ID = ID;
            this.Generation = Generation;
        }
    }
}
