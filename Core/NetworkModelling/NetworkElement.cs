using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ElecNetKit.NetworkModelling.Phasing;

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
            if (!_ConnectedToPhased.ContainsKey(1))
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
                return _ConnectedToPhased.Values.SelectMany(phaseVals => phaseVals.Select(conn => conn.Element)).Distinct();
            }
        }

        public IEnumerable<NetworkElement> ConnectedOnAllActivePhases
        {
            get
            {
                return _ConnectedToPhased.Where(kvp => kvp.Key != 0).Select(kvp => kvp.Value).Aggregate<IEnumerable<NetworkElementConnection>, IEnumerable<NetworkElement>>(null, (seed, elem) => seed == null ? elem.Select(conn => conn.Element) : seed.Intersect(elem.Select(conn => conn.Element)));
            }
        }

        /// <summary>
        /// Disconnects two <see cref="NetworkElement"/> from each other.
        /// </summary>
        /// <param name="elem1">The first element to disconnect.</param>
        /// <param name="elem2">The second element to disconnect.</param>
        protected static void Disconnect(NetworkElement elem1, NetworkElement elem2)
        {
            foreach (var connections in elem1._ConnectedToPhased.Values)
            {
                foreach (var deleteCon in connections.Where(conn => conn.Element == elem2))
                {
                    connections.Remove(deleteCon);
                }
            }
            foreach (var connections in elem2._ConnectedToPhased.Values)
            {
                foreach (var deleteCon in connections.Where(conn => conn.Element == elem1))
                {
                    connections.Remove(deleteCon);
                }
            }
        }

        protected static void Disconnect(NetworkElement elem1, int phase1, NetworkElement elem2, int phase2)
        {
            if (!ConnectionExists(elem1, phase1, elem2, phase2))
                return;

            elem1._ConnectedToPhased[phase1].Remove(new NetworkElementConnection(elem2,phase2));
            elem2._ConnectedToPhased[phase2].Remove(new NetworkElementConnection(elem1, phase1));
        }

        /// <summary>
        /// Disconnect this network element from another network element.
        /// </summary>
        /// <param name="elem">The <see cref="NetworkElement"/> to disconnect from.</param>
        protected void Disconnect(NetworkElement elem)
        {
            Disconnect(this, elem);
        }

        protected static void ConnectBetween(NetworkElement thisElem, int thisElemPhase, NetworkElement elem1, int elem1Phase, NetworkElement elem2, int elem2Phase)
        {
            Connect(thisElem, thisElemPhase, elem1, elem1Phase);
            Connect(thisElem, thisElemPhase, elem2, elem2Phase);
        }

        protected void ConnectBetween(int thisElemPhase, NetworkElement elem1, int elem1Phase, NetworkElement elem2, int elem2Phase)
        {
            Connect(this, thisElemPhase, elem1, elem1Phase);
            Connect(this, thisElemPhase, elem2, elem2Phase);
        }

        private static void Connect(NetworkElement elem1, int phase1, NetworkElement elem2, int phase2)
        {
            if (ConnectionExists(elem1,phase1,elem2,phase2))
                return;

            if (!elem1._ConnectedToPhased.ContainsKey(phase1))
                elem1._ConnectedToPhased[phase1] = new Collection<NetworkElementConnection>();

            elem1._ConnectedToPhased[phase1].Add(new NetworkElementConnection(elem2, phase2));

            if (!elem2._ConnectedToPhased.ContainsKey(phase2))
                elem2._ConnectedToPhased[phase2] = new Collection<NetworkElementConnection>();

            elem2._ConnectedToPhased[phase2].Add(new NetworkElementConnection(elem1, phase1));
        }

        public static bool ConnectionExists(NetworkElement elem1, int phase1, NetworkElement elem2, int phase2)
        {
            return elem1._ConnectedToPhased.ContainsKey(phase1) &&
                    elem1._ConnectedToPhased[phase1].Any(conn => conn.Element == elem2 && conn.Phase == phase2);
        }

        public bool ConnectionExists(int thisElemPhase, NetworkElement otherElem, int otherElemPhase)
        {
            return ConnectionExists(this, thisElemPhase, otherElem, otherElemPhase);
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
