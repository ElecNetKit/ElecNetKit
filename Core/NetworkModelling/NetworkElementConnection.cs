using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.NetworkModelling
{
    [Serializable]
    public class NetworkElementConnection
    {
        public NetworkElement Element {set; get;}
        public int Phase { set; get; }

        public NetworkElementConnection(NetworkElement Element, int Phase)
        {
            this.Element = Element;
            this.Phase = Phase;
        }
    }
}
