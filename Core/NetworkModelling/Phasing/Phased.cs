using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace ElecNetKit.NetworkModelling.Phasing
{
    public interface Phased<T> : IDictionary<int,T>
    {
    }

    [Serializable]
    public class PhasedValues<T> : Dictionary<int,T>, Phased<T>
    {
    }
}
