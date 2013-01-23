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
    public abstract class NetworkElement : IDeserializationCallback
    {
        [NonSerialized]
        Phased<ReadOnlyCollection<NetworkElementConnection>> _ConnectedToPhasedReadOnly;

        private Phased<Collection<NetworkElementConnection>> _ConnectedToPhased;


        /// <summary>
        /// A set of other elements that the <see cref="NetworkElement"/> is connected to, arranged by phase.
        /// </summary>
        public Phased<ReadOnlyCollection<NetworkElementConnection>> ConnectedToPhased { get { return _ConnectedToPhasedReadOnly; } }

        /// <summary>
        /// A set of other elements that the <see cref="NetworkElement"/> is connected to. Incorporates
        /// connections on any phase. This property can be safely used for analysis of balanced three-phase or single-phase networks.
        /// </summary>
        public IEnumerable<NetworkElement> ConnectedTo
        {
            get
            {
                return ConnectedToAnyPhase;
            }
        }

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
            OnDeserialization(null);
        }

        /// <summary>
        /// All connected elements, across any phase.
        /// </summary>
        public IEnumerable<NetworkElement> ConnectedToAnyPhase
        {
            get
            {
                return _ConnectedToPhased.Values.SelectMany(phaseVals => phaseVals.Select(conn => conn.Element)).Distinct();
            }
        }

        /// <summary>
        /// All elements that are connected to this element across all active phases (and possibly across neutral, phase 0).
        /// </summary>
        public IEnumerable<NetworkElement> ConnectedOnAllActivePhases
        {
            get
            {
                return _ConnectedToPhased.Values.Aggregate<IEnumerable<NetworkElementConnection>, IEnumerable<NetworkElement>>(null, (seed, elem) => seed == null ? elem.Select(conn => conn.Element) : seed.Intersect(elem.Select(conn => conn.Element)));
            }
        }

        /// <summary>
        /// Disconnects two <see cref="NetworkElement"/> from each other across all
        /// phases.
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

        /// <summary>
        /// Disconnects a specific phased connection between network elements.
        /// </summary>
        /// <param name="elem1">The first element to disconnect.</param>
        /// <param name="phase1">The phase that the connection is currently on, for <paramref name="elem1"/>.</param>
        /// <param name="elem2">The second element to disconnect.</param>
        /// <param name="phase2">The phase that the connection is currently on, for <paramref name="elem2"/>.</param>
        protected static void Disconnect(NetworkElement elem1, int phase1, NetworkElement elem2, int phase2)
        {
            if (!ConnectionExists(elem1, phase1, elem2, phase2))
                return;

            elem1._ConnectedToPhased[phase1].Remove(new NetworkElementConnection(elem2, phase2));
            elem2._ConnectedToPhased[phase2].Remove(new NetworkElementConnection(elem1, phase1));
        }

        /// <summary>
        /// Disconnect this network element from another network element across all phases.
        /// </summary>
        /// <param name="elem">The <see cref="NetworkElement"/> to disconnect from.</param>
        protected void Disconnect(NetworkElement elem)
        {
            Disconnect(this, elem);
        }

        /// <summary>
        /// Connect a <see cref="NetworkElement"/> between two other <see cref="NetworkElement"/>s, with a specific phasing.
        /// </summary>
        /// <param name="thisElem">The element to connect in between the other two elements.</param>
        /// <param name="thisElemPhase">The phase of <paramref name="thisElem"/> that should be connected.</param>
        /// <param name="elem1">The element to connect to <paramref name="thisElem"/> on one side.</param>
        /// <param name="elem1Phase">The phase of <paramref name="elem1"/> that should be connected.</param>
        /// <param name="elem2">The element to connect to <paramref name="thisElem"/> on the other side.</param>
        /// <param name="elem2Phase">The phase of <paramref name="elem2"/> that should be connected.</param>
        protected static void ConnectBetween(NetworkElement thisElem, int thisElemPhase, NetworkElement elem1, int elem1Phase, NetworkElement elem2, int elem2Phase)
        {
            Connect(thisElem, thisElemPhase, elem1, elem1Phase);
            Connect(thisElem, thisElemPhase, elem2, elem2Phase);
        }

        /// <summary>
        /// Connects the <see cref="NetworkElement"/> between two other <see cref="NetworkElement"/>s, with a specific phasing.
        /// </summary>
        /// <param name="thisElemPhase">The phase that should be connected.</param>
        /// <param name="elem1">The element to connect to this element on one side.</param>
        /// <param name="elem1Phase">The phase of <paramref name="elem1"/> that should be connected.</param>
        /// <param name="elem2">The element to connect to this element on the other side.</param>
        /// <param name="elem2Phase">The phase of <paramref name="elem2"/> that should be connected.</param>
        protected void ConnectBetween(int thisElemPhase, NetworkElement elem1, int elem1Phase, NetworkElement elem2, int elem2Phase)
        {
            Connect(this, thisElemPhase, elem1, elem1Phase);
            Connect(this, thisElemPhase, elem2, elem2Phase);
        }

        /// <summary>
        /// Connects a <see cref="NetworkElement"/> to a single other <see cref="NetworkElement"/>, with a specified phasing.
        /// </summary>
        /// <param name="elem1">The first element to connect.</param>
        /// <param name="phase1">The phase of the first element to connect.</param>
        /// <param name="elem2">The second element to connect.</param>
        /// <param name="phase2">The phase of the second element to connect.</param>
        protected static void Connect(NetworkElement elem1, int phase1, NetworkElement elem2, int phase2)
        {
            if (ConnectionExists(elem1, phase1, elem2, phase2))
                return;

            if (!elem1._ConnectedToPhased.ContainsKey(phase1))
                elem1._ConnectedToPhased[phase1] = new Collection<NetworkElementConnection>();

            elem1._ConnectedToPhased[phase1].Add(new NetworkElementConnection(elem2, phase2));

            if (!elem2._ConnectedToPhased.ContainsKey(phase2))
                elem2._ConnectedToPhased[phase2] = new Collection<NetworkElementConnection>();

            elem2._ConnectedToPhased[phase2].Add(new NetworkElementConnection(elem1, phase1));
        }

        /// <summary>
        /// Connects this <see cref="NetworkElement"/> to a single other <see cref="NetworkElement"/>, with a specified phasing.
        /// </summary>
        /// <param name="thisPhase">The phase of this element to connect.</param>
        /// <param name="connectTo">The other element to connect.</param>
        /// <param name="connectToPhase">The phase of the other element to connect.</param>
        protected void Connect(int thisPhase, NetworkElement connectTo, int connectToPhase)
        {
            Connect(this, thisPhase, connectTo, connectToPhase);
        }

        /// <summary>
        /// Test if a specific connection between elements on specific phases exists.
        /// </summary>
        /// <param name="elem1">The first element to check.</param>
        /// <param name="phase1">The phase of <paramref name="elem1"/> to check.</param>
        /// <param name="elem2">The second element to check.</param>
        /// <param name="phase2">The phase of <paramref name="elem2"/> to check.</param>
        /// <returns><c>true</c> if the specifically requested connection exists.</returns>
        public static bool ConnectionExists(NetworkElement elem1, int phase1, NetworkElement elem2, int phase2)
        {
            return elem1._ConnectedToPhased.ContainsKey(phase1) &&
                    elem1._ConnectedToPhased[phase1].Any(conn => conn.Element == elem2 && conn.Phase == phase2);
        }

        /// <summary>
        /// Test if a specific connection between this element and another element on specific phases exists.
        /// </summary>
        /// <param name="thisElemPhase">The phase of this element to check.</param>
        /// <param name="otherElem">The second element to check.</param>
        /// <param name="otherElemPhase">The phase of <paramref name="otherElem"/> to check.</param>
        /// <returns><c>true</c> if the specifically requested connection exists.</returns>
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

        /// <summary>
        /// Reconstructs the <see cref="NetworkElement"/> after deserialisation.
        /// </summary>
        /// <param name="sender">This value is unused.</param>
        public virtual void OnDeserialization(object sender)
        {
            _ConnectedToPhasedReadOnly =
                new CachedPhasedReadOnlyEvaluated<Collection<NetworkElementConnection>,
                                                  ReadOnlyCollection<NetworkElementConnection>
                                                 >(x => new ReadOnlyCollection<NetworkElementConnection>(x),
                                                   _ConnectedToPhased
                                                  );
        }
    }
}
