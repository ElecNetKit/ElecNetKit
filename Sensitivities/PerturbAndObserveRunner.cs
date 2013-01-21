using ElecNetKit.NetworkModelling;
using ElecNetKit.Simulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.Sensitivities
{
    using System.Collections;
    using IDTypePair = Tuple<String, Type>;
    public class PerturbAndObserveRunner<TObserve>
    {
        public String NetworkMasterFile { set; get; }

        private NetworkController controller;

        protected PerturbAndObserveRunner(){}

        public PerturbAndObserveRunner(ISimulator Simulator)
        {
            controller = new NetworkController(Simulator);
            PerturbValuesToRecord = x => x;
        }

        public Func<NetworkModel, IEnumerable<NetworkElement>> ObserveElementQuery { set; get; }

        public Func<NetworkElement, TObserve> ObserveElementValuesQuery { set; get; }

        public Func<NetworkModel, IEnumerable<NetworkElement>> PerturbElementQuery { set; get; }

        public Func<NetworkElement, Object[]> PerturbElementValuesQuery { set; get; }

        public Func<Object[], Object> PerturbValuesToRecord { set; get; }

        public IEnumerable<String> PerturbCommands { set; get; }

        public Dictionary<IDTypePair, TObserve> BeforeValues { private set; get; }
        public Dictionary<Tuple<IDTypePair, Object>, Dictionary<IDTypePair, TObserve>> AfterValues { private set; get; }

        public void RunPerturbAndObserve()
        {
            controller.NetworkFilename = NetworkMasterFile;
            controller.CacheNetwork = true;

            //hook this in here so we can capture the no-change state.
            var observeTransform = new ObserveResultsTransform<TObserve>(ObserveElementQuery, ObserveElementValuesQuery);
            controller.ResultsTransformer = observeTransform;
            controller.Execute();

            BeforeValues = observeTransform.WatchedElements;
            var afterValues = new Dictionary<Tuple<IDTypePair, Object>, Dictionary<IDTypePair, TObserve>>();

            var experimentor = new StringFormatExperimentor();
            controller.ExperimentDriver = experimentor;
            experimentor.ExperimentCommands = PerturbCommands;

            foreach (var elem in PerturbElementQuery(controller.Network))
            {
                var perturbValues = PerturbElementValuesQuery(elem);
                experimentor.ExperimentValues = perturbValues;
                controller.Execute();
                afterValues[Tuple.Create(new IDTypePair(elem.ID,elem.GetType()),PerturbValuesToRecord(perturbValues))]
                = observeTransform.WatchedElements;
            }
            AfterValues = afterValues;
        }

        public static Object[] ToObjArray(Object obj)
        {
            var x = obj as IEnumerable<Object>;

            if (x == null)
                return new[] { x };

            return x.ToArray();
        }
    }
}
