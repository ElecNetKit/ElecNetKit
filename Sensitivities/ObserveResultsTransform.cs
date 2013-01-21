using ElecNetKit.NetworkModelling;
using ElecNetKit.Transform;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.Sensitivities
{
    using IDTypePair = Tuple<String, Type>;

    /// <summary>
    /// A transform that observes and saves data from specific network elements.
    /// </summary>
    /// <typeparam name="T">The type of data to store about each selected element.</typeparam>
    public class ObserveResultsTransform<T> : IResultsTransform
    {
        /// <summary>
        /// Instantiates a new <see cref="ObserveResultsTransform{T}"/>, with the specified
        /// observation targets and observation values.
        /// </summary>
        /// <param name="ChooseElementsStrategy">A function defining the network elements that
        /// should be examined.</param>
        /// <param name="ObserveStrategy">A function that returns a value of type <typeparamref name="T"/>
        /// for each element specified by <see cref="ChooseElementsStrategy"/>.</param>
        public ObserveResultsTransform(Func<NetworkModel, IEnumerable<NetworkElement>> ChooseElementsStrategy, Func<NetworkElement, T> ObserveStrategy)
        {
            this.ChooseElementsStrategy = ChooseElementsStrategy;
            this.ObserveStrategy = ObserveStrategy;
        }

        Func<NetworkModel, IEnumerable<NetworkElement>> ChooseElementsStrategy;

        Func<NetworkElement, T> ObserveStrategy;

        /// <summary>
        /// The data obtained by watching the values specified by <see cref="ObserveStrategy"/>
        /// of the elements specified by <see cref="ChooseElementsStrategy"/>.
        /// </summary>
        /// <value>
        /// The keys in the <see cref="Dictionary{TKey,TValue}"/> returned by <see cref="WatchedElements"/>
        /// are <see cref="Tuple{T1,T2}"/>s, where <see cref="Tuple{T1,T2}.Item1"/> is the ID of the element,
        /// and <see cref="Tuple{T1,T2}.Item1"/> is the type of the element.</value>
        public Dictionary<IDTypePair,T> WatchedElements {private set; get;}

        /// <inheritdoc />
        public void PreExperimentHook(NetworkModel Network)
        {
        }

        /// <summary>
        /// Processes the network data after the experiment is run. Stores the requested
        /// network data in the <see cref="WatchedElements"/> property.
        /// </summary>
        /// <inheritdoc />
        public void PostExperimentHook(NetworkModel Network)
        {
            var results = new Dictionary<IDTypePair, T>();

            foreach (var elem in ChooseElementsStrategy(Network))
            {
               results[new IDTypePair(elem.ID, elem.GetType())] = ObserveStrategy(elem);
            }
            WatchedElements = results;
        }
    }
}
