using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ElecNetKit.NetworkModelling
{
    /// <summary>
    /// The <see cref="NetworkElement"/> defines the basic interconnection
    /// model between elements of the electrical network.
    /// </summary>
    [Serializable]
    public abstract class NetworkElement
    {
        private Phased<Collection<NetworkElementConnection>> _ConnectedToPhased;

        /// <summary>
        /// A set of other elements that the <see cref="NetworkElement"/> is connected to.
        /// </summary>
        public Phased<IReadOnlyCollection<NetworkElementConnection>> ConnectedToPhased { get { return (Phased<IReadOnlyCollection<NetworkElementConnection>>)_ConnectedToPhased; } }

        public IEnumerable<NetworkElement> ConnectedTo { get {
            if (!_ConnectedToPhased.PhaseExists(1))
                _ConnectedToPhased[1] = new Collection<NetworkElementConnection>();
            return _ConnectedToPhased[1].Select(conn => conn.Element); } }

        /// <summary>
        /// The ID of this specific element.
        /// </summary>
        public String ID { protected set; get; }

        /// <summary>
        /// Instantiate a new <see cref="NetworkElement"/>. Initialises the network element
        /// to be connected to nothing.
        /// </summary>
        public NetworkElement()
        {
            _ConnectedToPhased = new PhasedValues<Collection<NetworkElementConnection>>();
        }

        public IEnumerable<NetworkElement> ConnectedToAnyPhase
        {
            get
            {
                return _ConnectedToPhased.Phases.SelectMany(phase => _ConnectedToPhased[phase].Select(conn => conn.Element)).Distinct();
            }
        }

        public IEnumerable<NetworkElement> ConnectedOnAllMatchingPhases
        {
            get
            {
                return ConnectedOnAllPhases.Where(elem => this._ConnectedToPhased.Phases.Select(phase => this._ConnectedToPhased[phase].Single(x => x.Element == elem).Phase == phase).Aggregate(true,(seed, val) => seed && val));
            }
        }

        public IEnumerable<NetworkElement> ConnectedOnAllPhases
        {
            get
            {
                return _ConnectedToPhased.Phases.Aggregate<int,IEnumerable<NetworkElement>>(null, (seed, elem) => seed == null ? _ConnectedToPhased[elem].Select(conn=>conn.Element) : seed.Intersect(_ConnectedToPhased[elem].Select(conn=>conn.Element)));
            }
        }

        /// <summary>
        /// Connects two <see cref="NetworkElement"/> together. This is a single-phase operation.
        /// </summary>
        /// <param name="elem1">The first <see cref="NetworkElement"/>.</param>
        /// <param name="elem2">The second <see cref="NetworkElement"/>.</param>
        public static void Connect(NetworkElement elem1, NetworkElement elem2)
        {
            if (!elem1.ConnectedTo.Contains(elem2))
            {
                elem1._ConnectedToPhased[1].Add(new NetworkElementConnection(elem2, 1));
            }
            if (!elem2.ConnectedTo.Contains(elem1))
            {
                elem2._ConnectedToPhased[1].Add(new NetworkElementConnection(elem1, 1));
            }
        }

        /// <summary>
        /// Disconnects two <see cref="NetworkElement"/> from each other.
        /// </summary>
        /// <param name="elem1">The first element to disconnect.</param>
        /// <param name="elem2">The second element to disconnect.</param>
        public static void Disconnect(NetworkElement elem1, NetworkElement elem2)
        {
            foreach (var phase in elem1._ConnectedToPhased.Phases)
            {
                elem1._ConnectedToPhased[phase].Remove(elem1._ConnectedToPhased[phase].Single(conn => conn.Element == elem2));
            }
            foreach (var phase in elem2._ConnectedToPhased.Phases)
            {
                elem2._ConnectedToPhased[phase].Remove(elem2._ConnectedToPhased[phase].Single(conn => conn.Element == elem1));
            }
        }

        /// <summary>
        /// Connect this <see cref="NetworkElement"/> to another <see cref="NetworkElement"/>.
        /// </summary>
        /// <param name="elem">The <see cref="NetworkElement"/> to connect to.</param>
        public void Connect(NetworkElement elem)
        {
            Connect(this, elem);
        }

        /// <summary>
        /// Disconnect this network element from another network element.
        /// </summary>
        /// <param name="elem">The <see cref="NetworkElement"/> to disconnect from.</param>
        public void Disconnect(NetworkElement elem)
        {
            Disconnect(this, elem);
        }

        public void Connect(NetworkElement elem1, int phase1, NetworkElement elem2, int phase2)
        {
            if (ConnectionExists(elem1,phase1,elem2,phase2))
                return;

            if (!elem1._ConnectedToPhased.PhaseExists(phase1))
                elem1._ConnectedToPhased[phase1] = new Collection<NetworkElementConnection>();

            elem1._ConnectedToPhased[phase1].Add(new NetworkElementConnection(elem2, phase2));

            if (!elem2._ConnectedToPhased.PhaseExists(phase2))
                elem2._ConnectedToPhased[phase2] = new Collection<NetworkElementConnection>();

            elem2._ConnectedToPhased[phase2].Add(new NetworkElementConnection(elem1, phase1));
        }

        public bool ConnectionExists(NetworkElement elem1, int phase1, NetworkElement elem2, int phase2)
        {
            return elem1._ConnectedToPhased.PhaseExists(phase1) &&
                    elem1._ConnectedToPhased[phase1].Any(conn => conn.Element == elem2 && conn.Phase == phase2);
        }

        /// <summary>
        /// Gets a type-string for the element, e.g. 'Bus', 'Load', etc.
        /// </summary>
        public String ElementType
        {
            get
            {
                String typeStr = this.GetType().ToString();
                return typeStr.Substring(typeStr.LastIndexOf(".") + 1);
            }
        }
    }
}
