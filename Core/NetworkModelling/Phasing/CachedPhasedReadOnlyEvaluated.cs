using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.NetworkModelling.Phasing
{
    public class CachedPhasedReadOnlyEvaluated<TFrom,T> : PhasedReadOnlyEvaluated<TFrom,T>
    {
        Dictionary<TFrom, T> buffer;
        Func<TFrom, T> transform;
        public CachedPhasedReadOnlyEvaluated(Func<TFrom, T> transform, Phased<TFrom> theBase)
        {
            this.basePhased = theBase;
            this.transform = transform;
            this.getTransform = CacheTransform;
            buffer = new Dictionary<TFrom, T>();
        }

        protected T CacheTransform(TFrom arg)
        {
            if (!buffer.ContainsKey(arg))
                buffer[arg] = transform(arg);
            return buffer[arg];
        }
    }
}
