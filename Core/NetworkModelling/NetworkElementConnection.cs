using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.NetworkModelling
{
    /// <summary>
    /// A structure that defines a single connection to another network element,
    /// specifying a network element and a phase to connect on.
    /// </summary>
    [Serializable]
    public struct NetworkElementConnection
    {
        /// <summary>
        /// The <see cref="NetworkElement"/> that this structure represents a connection to.
        /// </summary>
        public NetworkElement Element { get { return _Element; } }
        private NetworkElement _Element;
        
        /// <summary>
        /// The phase of <see cref="Element"/> that this structure represents a connection to.
        /// </summary>
        public int Phase { get { return _Phase; } }
        private int _Phase;

        /// <summary>
        /// Creates a new <see cref="NetworkElementConnection"/>.
        /// </summary>
        /// <param name="Element">The <see cref="NetworkElement"/> that this structure represents a connection to.</param>
        /// <param name="Phase">The phase of <see cref="Element"/> that this structure represents a connection to.</param>
        public NetworkElementConnection(NetworkElement Element, int Phase)
        {
            _Element = Element;
            _Phase = Phase;
        }

        /// <summary>
        /// Compares between this and another <see cref="NetworkElementConnection"/> for member-wise equality.
        /// </summary>
        /// <param name="obj">The other <see cref="NetworkElementConnection"/> to compare to.</param>
        /// <returns><c>true</c> if the two elements are equal.</returns>
        public bool Equals(NetworkElementConnection obj)
        {
            return  obj.Element == this.Element && 
                    obj.Phase   == this.Phase;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(NetworkElementConnection))
                return false;

            return this.Equals((NetworkElementConnection)obj);
        }

        /// <summary>
        /// Tests for member-wise equality between <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The first connection to check.</param>
        /// <param name="b">The second connection to check.</param>
        /// <returns><c>true</c> if the two <see cref="NetworkElementConnection"/>s are equal.</returns>
        public static bool operator ==(NetworkElementConnection a, NetworkElementConnection b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Tests for inequality between <paramref name="a"/> and <paramref name="b"/>.
        /// Defined as <c>!(a == b)</c>.
        /// </summary>
        /// <param name="a">The first connection to check.</param>
        /// <param name="b">The second connection to check.</param>
        /// <returns><c>true</c> if the two <see cref="NetworkElementConnection"/>s are not equal.</returns>
        public static bool operator !=(NetworkElementConnection a, NetworkElementConnection b)
        {
            return !(a == b);
        }

        ///<inheritdoc />
        public override string ToString()
        {
            return String.Format("Conn: {0}:{1} Phase {2}", Element.ElementType, Element.ID, Phase);
        }

        /// <inheritdoc />
        /// <remarks>This function is based on the method proposed by <see href="http://www.cse.yorku.ca/~oz/hash.html">Dan Bernstein</see>
        /// and used extensively on <see href="http://stackoverflow.com/a/263416/996592">Stack Overflow</see>.</remarks>
        public override int GetHashCode()
        {
            int hash = 5381;
            unchecked
            {
                hash = hash * 33 + Element.GetHashCode();
                hash = hash * 33 + Phase.GetHashCode();
            }
            return hash;
        }
    }
}
