using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using ElecNetKit.Util;

using System.Runtime.Serialization;

using System.Numerics;


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
    public class Bus : NetworkElement
    {
        /// <summary>
        /// The single-phase voltage (in complex phasor notation) of the bus.
        /// </summary>
        public Complex Voltage { set; get; }

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
        /// The voltage of the bus in p.u. terms, defined as
        /// <see cref="Voltage"/>/<see cref="BaseVoltage"/>.
        /// </summary>
        public Complex VoltagePU
        {
            get
            {
                return Voltage / BaseVoltage;
            }
        }

        /// <summary>
        /// Instantiates a new <see cref="Bus"/>.
        /// </summary>
        /// <param name="ID">the ID of the bus. Should be unique among buses, but
        /// does not need to be unique amongst all network elements.</param>
        /// <param name="Voltage">The single-phase absolute voltage of the bus
        /// (in Volts).</param>
        /// <param name="BaseVoltage">The single-phase base voltage of the bus
        /// (in Volts).</param>
        /// <param name="Location">The XY coordinates of the bus. Used for
        /// graphing the network.</param>
        public Bus(String ID, Complex Voltage, double BaseVoltage, Point? Location)
        {
            this.ID = ID;
            this.Voltage = Voltage;
            this.BaseVoltage = BaseVoltage;
            this.Location = Location;
        }
    }
}
