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
        private Collection<NetworkElement> _ConnectedTo;

        /// <summary>
        /// A set of other elements that the <see cref="NetworkElement"/> is connected to.
        /// </summary>
        public IReadOnlyCollection<NetworkElement> ConnectedTo { get { return _ConnectedTo; } }

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
            _ConnectedTo = new Collection<NetworkElement>();
        }

        /// <summary>
        /// Connects two <see cref="NetworkElement"/> together.
        /// </summary>
        /// <param name="elem1">The first <see cref="NetworkElement"/>.</param>
        /// <param name="elem2">The second <see cref="NetworkElement"/>.</param>
        public static void Connect(NetworkElement elem1, NetworkElement elem2)
        {
            if (!elem1.ConnectedTo.Contains(elem2))
            {
                elem1._ConnectedTo.Add(elem2);
            }
            if (!elem2.ConnectedTo.Contains(elem1))
            {
                elem2._ConnectedTo.Add(elem1);
            }
        }

        /// <summary>
        /// Disconnects two <see cref="NetworkElement"/> from each other.
        /// </summary>
        /// <param name="elem1">The first element to disconnect.</param>
        /// <param name="elem2">The second element to disconnect.</param>
        public static void Disconnect(NetworkElement elem1, NetworkElement elem2)
        {
            elem1._ConnectedTo.Remove(elem2);
            elem2._ConnectedTo.Remove(elem1);
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
