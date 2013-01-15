using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.NetworkModelling
{
    [Serializable]
    public struct NetworkElementConnection
    {
        private NetworkElement _Element;
        public NetworkElement Element { get { return _Element; } }
        private int _Phase;
        public int Phase { get { return _Phase; } }

        public NetworkElementConnection(NetworkElement Element, int Phase)
        {
            _Element = Element;
            _Phase = Phase;
        }

        public bool Equals(NetworkElementConnection obj)
        {
            return  obj.Element == this.Element && 
                    obj.Phase   == this.Phase;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(NetworkElementConnection))
                return false;

            return this.Equals((NetworkElementConnection)obj);
        }

        public static bool operator ==(NetworkElementConnection a, NetworkElementConnection b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(NetworkElementConnection a, NetworkElementConnection b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return String.Format("Conn: {0}:{1} Phase {2}", Element.ElementType, Element.ID, Phase);
        }

        /// <inheritdoc />
        /// <remarks>This function is based on the method proposed by <see href="http://www.cse.yorku.ca/~oz/hash.html">Dan Bernstein</see>
        /// and used extensively on <see href="http://stackoverflow.com/a/263416/996592">StackOverflow</see>.</remarks>
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
