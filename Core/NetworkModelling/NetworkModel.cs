using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using System.Windows;

using ElecNetKit.NetworkModelling;
using ElecNetKit.Util;
using System.Runtime.Serialization;
using System.Numerics;


namespace ElecNetKit.NetworkModelling
{
   /// <summary>
    /// Defines the entire network model, in terms of buses, lines, loads, generators
    /// and the connections between them.
    /// </summary>
    [Serializable]
    public class NetworkModel
    {
        /// <summary>
        /// A dictionary of buses in the network, indexed by Bus ID so as to be
        /// easier to find and connect to other network elements.
        /// </summary>
        public Dictionary<String, Bus> Buses { protected set; get; }

        /// <summary>
        /// Every line in the network.
        /// </summary>
        public Collection<Line> Lines { protected set; get; }

        /// <summary>
        /// Every load in the network.
        /// </summary>
        public Collection<Load> Loads { protected set; get; }

        /// <summary>
        /// Every generator in the network.
        /// </summary>
        public Collection<Generator> Generators { protected set; get; }

        /// <summary>
        /// A rectangle indicating the outer limits of the XY coordinates of the
        /// network buses.
        /// </summary>
        protected Rect networkBounds;

        /// <summary>
        /// The bounding box of all network elements in network coordinates.
        /// Calculated on network construction, or by calling <see cref="UpdateNetworkBounds"/>.
        /// </summary>
        public Rect NetworkBounds
        {
            get
            {
                return networkBounds;
            }
        }

        /// <summary>
        /// The total losses on the network, across all phases, in kVA.
        /// </summary>
        public Complex LosseskVA { get; set; }

        /// <summary>
        /// The source (stiff, fixed) bus of the network.
        /// </summary>
        public Bus SourceBus { get; set; }

        /// <summary>
        /// Instantiates a new <see cref="NetworkModel"/>.
        /// </summary>
        /// <param name="Buses">All the buses in the network.</param>
        /// <param name="Lines">All the lines in the network.</param>
        /// <param name="Loads">All the loads in the network.</param>
        /// <param name="Generators">All the generators in the network.</param>
        /// <param name="Losses">The losses (in kVA) across the whole network.</param>
        /// <param name="SourceBus">The source bus (fixed bus) of the network.</param>
        public NetworkModel(Dictionary<String, Bus> Buses, Collection<Line> Lines, Collection<Load> Loads, Collection<Generator> Generators, Complex Losses, Bus SourceBus)
        {
            this.Buses = Buses;
            this.Lines = Lines;
            this.Loads = Loads;
            this.Generators = Generators;
            this.LosseskVA = Losses;
            this.SourceBus = SourceBus;

            networkBounds = FindNetworkBounds();
        }

        /// <summary>
        /// Finds a bounding box for the network elements.
        /// </summary>
        /// <returns>A bounding box (in network coords) that contains all the objects in the network.</returns>
        protected Rect FindNetworkBounds()
        {
            Limits X = new Limits();
            Limits Y = new Limits();
            foreach (var bus in this.Buses.Values)
            {
                if (!bus.Location.HasValue)
                    continue;
                X.ProcessData(bus.Location.Value.X);
                Y.ProcessData(bus.Location.Value.Y);
            }
            return new Rect(X.AutoMin, Y.AutoMin, X.AutoMax - X.AutoMin, Y.AutoMax - Y.AutoMin);
        }

        /// <summary>
        /// Updates the network bounds based upon the XY coordinates of the
        /// network buses.
        /// </summary>
        public void UpdateNetworkBounds()
        {
            networkBounds = FindNetworkBounds();
        }
    }
}
