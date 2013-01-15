using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ElecNetKit.Util;
using System.Runtime.Serialization;
using System.Numerics;
using ElecNetKit.NetworkModelling.Phasing;

namespace ElecNetKit.NetworkModelling
{
    /// <summary>
    /// An electrical network element that injects power
    /// in to the network.
    /// </summary>
     [Serializable]
    public class Generator : PowerConversionElement
    {
        /// <summary>
        /// The single-phase kVA that the <see cref="Generator"/> injects into the network for
        /// a balanced three-phase or single-phase network.
        /// The real component represents the real power (kW) and
        /// the imaginary component represents the injected reactive (imaginary)
        /// power (kVAr).
        /// </summary>
         public Complex Generation { get { return GenerationPhased[1]; } set { GenerationPhased[1] = value; } }

         /// <summary>
         /// The total generation of the <see cref="Generator"/>, across all phases, in kVA.
         /// </summary>
         public Complex TotalGeneration { get { return GenerationPhased.Values.Aggregate((seed, element) => seed+element); } }

         /// <summary>
         /// The kVA that the <see cref="Generator"/> injects into the network.
         /// This is a phased value, of which the real component represents the real power (kW) and
         /// the imaginary component represents the injected reactive (imaginary)
         /// power (kVAr).
         /// </summary>
        public Phased<Complex> GenerationPhased { protected set; get; }

        /// <summary>
        /// Instantiates a new <see cref="Generator"/>. This constructor is
        /// for building single phase or 3-phase balanced networks only.
        /// </summary>
        /// <param name="ID">The ID of the generator. Must be unique among
        /// generators, but not among network elements.</param>
        /// <param name="Generation">The single-phase generation (in kVA) of
        /// the generator.</param>
        public Generator(String ID, Complex Generation)
        {
            this.ID = ID;
            this.GenerationPhased = new PhasedValues<Complex>();
            this.Generation = Generation;
        }

        /// <summary>
        /// Instantiates a new <see cref="Generator"/>. This constructor is
        /// for building arbitrarily-phased networks.
        /// </summary>
        /// <param name="ID">The ID of the generator. Must be unique among
        /// generators, but not among network elements.</param>
        /// <param name="Generation">The phased generation (in kVA) of
        /// the generator.</param>
        public Generator(String ID, Phased<Complex> Generation)
        {
            this.ID = ID;
            this.GenerationPhased = Generation;
        }
    }
}
