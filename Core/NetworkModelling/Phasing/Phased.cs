using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace ElecNetKit.NetworkModelling.Phasing
{
    /// <summary>
    /// An interface that defines a set of values of type <typeparamref name="T"/> and
    /// ordered by phase.
    /// </summary>
    /// <typeparam name="T">The type that should have a unique value per phase.</typeparam>
    /// <remarks>The <see cref="Phased{T}"/> interface is intended to be used directly by all
    /// client code. Implementors of <see cref="NetworkElement"/>s should consider using the
    /// types <see cref="PhasedValues{T}"/> and <see cref="PhasedEvaluated{TFrom, TTo}"/>, which implement
    /// the <see cref="Phased{T}"/> interface with a set of values and an evaluator function
    /// respectively. For example usage, see the implementation of <see cref="ElecNetKit.NetworkModelling.Bus"/>.
    /// </remarks>
    public interface Phased<T> : IDictionary<int,T>
    {
    }

    /// <summary>
    /// A value-based type that implements the <see cref="Phased{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The type that should have a unique value per phase.</typeparam>
    [Serializable]
    public class PhasedValues<T> : Dictionary<int,T>, Phased<T>
    {
    }
}
