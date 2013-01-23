using ElecNetKit.NetworkModelling;
using ElecNetKit.Simulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ElecNetKit.Sensitivities
{    
    using IDTypePair = Tuple<String, Type>;

    /// <summary>
    /// Runs perturb-and-observe experiments on electrical network models.
    /// </summary>
    /// <typeparam name="TObserve">The type of value to observe during experiments.</typeparam>
    public class PerturbAndObserveRunner<TObserve>
    {
        /// <summary>
        /// The full path to the master network file to run the perturb-and-observe experiment on.
        /// </summary>
        public String NetworkFilename { set; get; }

        //the network controller for running experiments.
        private NetworkController controller;

        /// <summary>
        /// Instantiates a new <see cref="PerturbAndObserveRunner{T}"/>.
        /// </summary>
        protected PerturbAndObserveRunner(){}

        /// <summary>
        /// Instantiates a new <see cref="PerturbAndObserveRunner{T}"/>.
        /// </summary>
        /// <param name="Simulator">The simulator to use for the perturb-and-observe experiment.</param>
        public PerturbAndObserveRunner(ISimulator Simulator)
        {
            controller = new NetworkController(Simulator);
            PerturbValuesToRecord = x => x;
        }

        /// <summary>
        /// A function that, given a <see cref="NetworkModel"/>, returns a set of <see cref="NetworkElement"/>s that should
        /// have a specific value observed.
        /// </summary>
        public Func<NetworkModel, IEnumerable<NetworkElement>> ObserveElementSelector { set; get; }

        /// <summary>
        /// A function that, given a <see cref="NetworkElement"/>, returns a value that should be recorded, both
        /// before and after the experiment is run.
        /// </summary>
        public Func<NetworkElement, TObserve> ObserveElementValuesSelector { set; get; }

        /// <summary>
        /// A function that, given a <see cref="NetworkModel"/>, returns a set of <see cref="NetworkElement"/>s
        /// that should be candidates for perturbation.
        /// </summary>
        public Func<NetworkModel, IEnumerable<NetworkElement>> PerturbElementSelector { set; get; }

        /// <summary>
        /// A function that, given a <see cref="NetworkElement"/> that is to be perturbed, should select a set
        /// of values that will be used as the parameters to the format strings given by <see cref="PerturbCommands"/>.
        /// </summary>
        public Func<NetworkElement, Object[]> PerturbElementValuesSelector { set; get; }

        /// <summary>
        /// A function that, given the set of values specified by <see cref="PerturbElementValuesSelector"/>, should
        /// return a value to be recorded as the perturbation.
        /// </summary>
        /// <value>
        /// Defaults to <c>x => x</c>, that is, to return the complete set of values specified by <see cref="PerturbElementValuesSelector"/>.</value>
        public Func<Object[], Object> PerturbValuesToRecord { set; get; }

        /// <summary>
        /// A set of commands that should be run on the network. These commands will be used as a parameter to
        /// <see cref="String.Format(String,Object[])"/>, and as such, should be in format-string notation. The
        /// corresponding values to fill in the parameters in these commands will be supplied by <see cref="PerturbElementValuesSelector"/>.
        /// </summary>
        public IEnumerable<String> PerturbCommands { set; get; }

        /// <summary>
        /// The set of observed values before any perturbation occurs.
        /// </summary>
        /// <value>
        /// The keys of the dictionary provided by this property are <see cref="Tuple{T1,T2}"/>s, where
        /// <see cref="Tuple{T1,T2}.Item1"/> is the ID of the observed network element, and <see cref="Tuple{T1,T2}.Item2"/>
        /// is the type of the observed network element.</value>
        public Dictionary<IDTypePair, TObserve> BeforeValues { private set; get; }

        /// <summary>
        /// A dictionary specifying the observed effect of all perturbations.
        /// </summary>
        /// <value>
        /// The keys of the outer dictionary provided by this property are <see cref="Tuple{T1,T2}"/>s, where
        /// <see cref="Tuple{T1,T2}.Item1"/> is a <see cref="Tuple{T1,T2}"/> specifying the ID and type of the perturbing network element, and <see cref="Tuple{T1,T2}.Item2"/>
        /// is the set of perturbation values, as specified by <see cref="PerturbValuesToRecord"/>.
        /// The keys of the inner dictionary are <see cref="Tuple{T1,T2}"/>s specifying the ID and type of the observed network element.
        /// The values of the inner dictionary are the values required to be observed by <see cref="ObserveElementValuesSelector"/>.</value>
        public Dictionary<Tuple<IDTypePair, Object>, Dictionary<IDTypePair, TObserve>> AfterValues { private set; get; }

        /// <summary>
        /// Runs the perturb-and-observe experiment.
        /// </summary>
        public void RunPerturbAndObserve()
        {
            controller.NetworkFilename = NetworkFilename;
            controller.ClearNetworkCache();
            controller.CacheNetwork = true;
            controller.ExperimentDriver = null;
            //hook this in here so we can capture the no-change state.
            var observeTransform = new ObserveResultsTransform<TObserve>(ObserveElementSelector, ObserveElementValuesSelector);
            controller.ResultsTransformer = observeTransform;
            controller.Execute();

            BeforeValues = observeTransform.WatchedElements;
            var afterValues = new Dictionary<Tuple<IDTypePair, Object>, Dictionary<IDTypePair, TObserve>>();

            var experimentor = new StringFormatExperimentor();
            controller.ExperimentDriver = experimentor;
            experimentor.ExperimentCommands = PerturbCommands;

            foreach (var elem in PerturbElementSelector(controller.Network))
            {
                var perturbValues = PerturbElementValuesSelector(elem);
                experimentor.ExperimentValues = perturbValues;
                controller.Execute();
                afterValues[Tuple.Create(new IDTypePair(elem.ID,elem.GetType()),PerturbValuesToRecord(perturbValues))]
                = observeTransform.WatchedElements;
            }
            AfterValues = afterValues;
        }
    }
}
