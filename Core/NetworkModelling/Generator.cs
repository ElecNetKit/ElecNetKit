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
        /// The total kVA that the <see cref="Generator"/> injects into the network.
        /// The real component represents the real power (kW) and
        /// the imaginary component represents the injected reactive (imaginary)
        /// power (kVAr).
        /// </summary>
         public Complex Generation
         {
             set
             {
                 if (GenerationPhased.Count != 1)
                     throw new NotSupportedException("Can't set Generation for a multi-phase generator. Use GenerationPhased instead.");
                 GenerationPhased[GenerationPhased.Keys.Single()] = value;
             }
             get { return GenerationPhased.Values.Aggregate((seed, elem) => seed + elem); }
         }

         /// <summary>
         /// The kVA that the <see cref="Generator"/> injects into the network.
         /// This is a phased value, of which the real component represents the real power (kW) and
         /// the imaginary component represents the injected reactive (imaginary)
         /// power (kVAr).
         /// </summary>
        public Phased<Complex> GenerationPhased { protected set; get; }

        /// <summary>
        /// Instantiates a new <see cref="Generator"/>. This constructor is
        /// for building balanced networks only.
        /// </summary>
        /// <param name="ID">The ID of the generator. Must be unique among
        /// generators, but not among network elements.</param>
        /// <param name="Generation">The total generation (in kVA) of
        /// the generator.</param>
        /// <param name="NumPhases">The number of phases to split the total generation between.</param>
        public Generator(String ID, Complex Generation, int NumPhases = 3)
        {
            this.ID = ID;
            this.GenerationPhased = new PhasedValues<Complex>();
            foreach (int phase in Enumerable.Range(1, NumPhases))
                GenerationPhased[phase] = Generation / NumPhases;
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
