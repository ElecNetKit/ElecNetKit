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

    class ObserveResultsTransform<T> : IResultsTransform
    {

        public ObserveResultsTransform(Func<NetworkModel, IEnumerable<NetworkElement>> ChooseElementsStrategy, Func<NetworkElement, T> ObserveStrategy)
        {
            this.ChooseElementsStrategy = ChooseElementsStrategy;
            this.ObserveStrategy = ObserveStrategy;
        }

        Func<NetworkModel, IEnumerable<NetworkElement>> ChooseElementsStrategy;

        Func<NetworkElement, T> ObserveStrategy;

        public Dictionary<IDTypePair,T> WatchedElements {private set; get;}

        public void PreExperimentHook(NetworkModel Network)
        {
        }

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
