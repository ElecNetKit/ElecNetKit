using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.NetworkModelling.Phasing
{
    /// <summary>
    /// An implementation of <see cref="PhasedReadOnlyEvaluated{TFrom,T}"/> that caches
    /// results and provides the same object multiple times if requested.
    /// </summary>
    /// <example>
    /// Use when <typeparamref name="T"/> is a type that provides new views into 
    /// the same data, such as when <typeparamref name="TFrom"/> is a
    /// <see cref="System.Collections.ObjectModel.Collection{T}"/>  and <typeparamref name="T"/> is a
    /// <see cref="System.Collections.ObjectModel.ReadOnlyCollection{T}"/>.</example>
    /// <typeparam name="TFrom">The type of the backing <see cref="Phased{T}"/>.</typeparam>
    /// <typeparam name="T">The type of the elements of this <see cref="Phased{T}"/>.</typeparam>
    public class CachedPhasedReadOnlyEvaluated<TFrom,T> : PhasedReadOnlyEvaluated<TFrom,T>
    {
        Dictionary<TFrom, T> buffer;
        Func<TFrom, T> transform;
        
        /// <summary>
        /// Instantiates a new <see cref="CachedPhasedReadOnlyEvaluated{TFrom,T}"/>
        /// </summary>
        /// <param name="transform">The transform to apply to the elements of the backing <see cref="Phased{T}"/>.</param>
        /// <param name="theBase">The backing <see cref="Phased{T}"/>.</param>
        public CachedPhasedReadOnlyEvaluated(Func<TFrom, T> transform, Phased<TFrom> theBase)
        {
            this.basePhased = theBase;
            this.transform = transform;
            this.getTransform = CacheTransform;
            buffer = new Dictionary<TFrom, T>();
        }

        /// <summary>
        /// Provides caching.
        /// </summary>
        /// <param name="arg">The element from the backing <see cref="Phased{T}"/>.</param>
        /// <returns>A converted and possibly cached transformed value.</returns>
        protected T CacheTransform(TFrom arg)
        {
            if (!buffer.ContainsKey(arg))
                buffer[arg] = transform(arg);
            return buffer[arg];
        }
    }
}
