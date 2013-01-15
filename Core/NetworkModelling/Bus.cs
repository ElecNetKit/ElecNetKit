using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using ElecNetKit.Util;

using System.Runtime.Serialization;

using System.Numerics;
using ElecNetKit.NetworkModelling.Phasing;


namespace ElecNetKit.NetworkModelling
{
    /// <summary>
    /// <see cref="Bus"/>es are the 'glue' that hold other network
    /// elements together, and as such, every other network element must be
    /// connected to at least one <see cref="Bus"/>. <see cref="Bus"/>es also
    /// store information on load-flow voltages, and have a location that is
    /// useful for graphing.
    /// </summary>
    [Serializable]
    public class Bus : NetworkElement, IDeserializationCallback
    {
        /// <summary>
        /// The single-phase (Line-Neutral) voltage (in complex phasor notation) of the bus. A single-phase, or balanced three-phase network is assumed.
        /// </summary>
        public Complex Voltage { set { VoltagePhased[1] = value; } get { return VoltagePhased[1]; } }

        /// <summary>
        /// The voltage of each bus phase, in complex notation.
        /// </summary>
        public Phased<Complex> VoltagePhased { private set; get; }

        /// <summary>
        /// The XY location of the bus, in network coordinates, useful for graphing.
        /// </summary>
        public Point? Location { set; get; }

        /// <summary>
        /// The Single-Phase base voltage of the bus (e.g. 11/sqrt(3) kV,
        /// 0.23 kV).
        /// </summary>
        public double BaseVoltage { set; get; }

        /// <summary>
        /// The single-phase voltage of the bus in p.u. terms, defined as
        /// <see cref="Voltage"/>/<see cref="BaseVoltage"/>. Assumes a single-phase or balanced three-phase network.
        /// </summary>
        public Complex VoltagePU { set { VoltagePUPhased[1] = value; } get { return VoltagePUPhased[1]; } }

        [NonSerialized]
        Phased<Complex> _VoltagePUPhased;

        /// <summary>
        /// The voltage of the bus in p.u. terms for each phase, defined as
        /// <see cref="VoltagePhased"/>/<see cref="BaseVoltage"/>.
        /// </summary>
        public Phased<Complex> VoltagePUPhased { get { return _VoltagePUPhased; } }

        /// <summary>
        /// Instantiates a new <see cref="Bus"/>. This is the single-phase/balanced three-phase
        /// constructor. Use this only if your model will be used for balanced
        /// network analysis.
        /// </summary>
        /// <param name="ID">The ID of the bus. Should be unique among buses, but
        /// does not need to be unique amongst all network elements.</param>
        /// <param name="Voltage">The single-phase absolute voltage of the bus
        /// (in Volts).</param>
        /// <param name="BaseVoltage">The single-phase base voltage of the bus
        /// (in Volts).</param>
        /// <param name="Location">The XY coordinates of the bus. Used for
        /// graphing the network.</param>
        public Bus(String ID, Complex Voltage, double BaseVoltage, Point? Location)
        {
            this.VoltagePhased = new PhasedValues<Complex>();
            this.Voltage = Voltage;
            this.ID = ID;
            this.BaseVoltage = BaseVoltage;
            this.Location = Location;
            OnDeserialization(null);
        }

        /// <summary>
        /// Instantiates a new <see cref="Bus"/>. This constructor supports
        /// arbitrarily-phased networks, including three-phase balanced and single-phase.
        /// </summary>
        /// <param name="ID">The ID of the bus. Should be unique among buses, but
        /// does not need to be unique amongst all network elements.</param>
        /// <param name="VoltagePhased">The absolute line-neutral voltage of each
        /// phase of the bus.</param>
        /// <param name="BaseVoltage">The single-phase base voltage of the bus
        /// (in Volts).</param>
        /// <param name="Location">The XY coordinates of the bus. Used for
        /// graphing the network.</param>
        public Bus(String ID, Phased<Complex> VoltagePhased, double BaseVoltage, Point? Location)
        {
            this.VoltagePhased = VoltagePhased;
            this.ID = ID;
            this.BaseVoltage = BaseVoltage;
            this.Location = Location;
            OnDeserialization(null);
        }

        /// <summary>
        /// Reconstructs the <see cref="Bus"/> when it has just been deserialised.
        /// </summary>
        /// <param name="sender">Not used.</param>
        public void OnDeserialization(object sender)
        {
            this._VoltagePUPhased = new PhasedEvaluated<Complex,Complex>(
                from => from / this.BaseVoltage, //get
                to => to * this.BaseVoltage, //set
                VoltagePhased
                );
        }
    }
}
