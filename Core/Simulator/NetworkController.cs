using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ElecNetKit.NetworkModelling;
using ElecNetKit.Experimentation;
using ElecNetKit.Transform;

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace ElecNetKit.Simulator
{
    /// <summary>
    /// A class that governs the production of a network model,
    /// experimenting on the network model and transforming the results
    /// (all as requested). Capable of working with any
    /// <see cref="ISimulator"/>, and will automatically detect and use
    /// any available simulator in the <c>/ElecNetKitExtensions</c> subfolder
    /// of the entry assembly (<see cref="System.Reflection.Assembly.GetEntryAssembly"/>)'s
    /// location.
    /// </summary>
    public class NetworkController
    {
        private CompositionContainer _container;

        /// <summary>
        /// The filename corresponding to a stored network model. The network model should
        /// define the network but not (necessarily) solve anything.
        /// </summary>
        public String NetworkFilename { set; get; }

        /// <summary>
        /// The particular experiment that will be applied to the network.
        /// </summary>
        public IExperimentor ExperimentDriver { set; get; }

        /// <summary>
        /// The <see cref="IResultsTransform"/> to be used in the experiment.
        /// </summary>
        public IResultsTransform ResultsTransformer { set; get; }

        /// <summary>
        /// The simulated network. Only avaiable once Execute() has been
        /// called.
        /// </summary>
        public NetworkModel Network { protected set; get; }

        /// <summary>
        /// The simulator to be used by the system.
        /// </summary>
        ISimulator simulator;

#pragma warning disable 0649 //disable "is never assigned to" error.
        [ImportMany]
        IEnumerable<Lazy<ISimulator, ISimulatorMetaData>> simulatorOptions;
#pragma warning restore 0649

        String _SimulatorTarget;

        /// <summary>
        /// The name of the simulator to be used by the <see cref="NetworkController"/>.
        /// </summary>
        public String SimulatorTarget { get { return _SimulatorTarget; } }

        /// <summary>
        /// A flag indicating, if set, that the <see cref="NetworkController"/> should
        /// only ever calculate the pre-experiment network once.
        /// Default value is <c>true</c>.
        /// </summary>
        public bool CacheNetwork { set; get; }

        /// <summary>
        /// Forces the pre-experiment network to be recalculated.
        /// </summary>
        public void ClearNetworkCache()
        {
            _initialNetwork = null;
        }

        private NetworkModel _initialNetwork;

        /// <summary>
        /// Instantiates a new <see cref="NetworkController"/>, using the specified simulator.
        /// </summary>
        /// <param name="SimulatorTarget">The name of the simulator to use, if available.</param>
        /// <exception cref="Exception">The simulator requested by <paramref name="SimulatorTarget"/> is not available.</exception>
        public NetworkController(String SimulatorTarget)
        {
            this._SimulatorTarget = SimulatorTarget;
            ObtainSimulator();
        }

        /// <summary>
        /// Initialises a new <see cref="NetworkController"/>. Autodetect any available simulator and use that.
        /// </summary>
        /// <exception cref="Exception">There was no, or multiple, simulators available.</exception>
        public NetworkController()
        {
            ObtainSimulator();
        }

        /// <summary>
        /// Instantiates a <see cref="NetworkController"/> using the provided simulator.
        /// </summary>
        /// <param name="sim">The simulator object to use for simulations.</param>
        public NetworkController(ISimulator sim)
        {
            simulator = sim;
        }

        private void ObtainSimulator()
        {
            var catalog = new AggregateCatalog();
            var dllPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\ElecNetKitExtensions";
            try {
                catalog.Catalogs.Add(new ApplicationCatalog());
            }
            catch { } // not working? that's ok, proceed as normal.
            try {
                catalog.Catalogs.Add(new DirectoryCatalog(dllPath, "*.dll"));
                catalog.Catalogs.Add(new DirectoryCatalog(dllPath, "*.exe"));
            }
            catch { } // not working? that's ok, proceed as normal.
            _container = new CompositionContainer(catalog);
            _container.ComposeParts(this);
            var keys = simulatorOptions.Select(x => x.Metadata.Name).Distinct();
            if (SimulatorTarget == null)
            {
                if (keys.Count() != 1)
                {
                    if (simulatorOptions.Count() < 1)
                        throw new Exception("No simulator could be detected.");
                    else
                        throw new Exception("Simulator can only be detected automatically if there is only one simulator available." +
                            " Try setting the SimulatorTarget property to the appropriate simulator type");
                }

                simulator = simulatorOptions.Single().Value;
            }
            else
            {
                foreach (var simulatorOption in simulatorOptions)
                {
                    if (simulatorOption.Metadata.Name == SimulatorTarget)
                        simulator = simulatorOption.Value;
                }
            }
            if (simulator == null)
            {
                throw new Exception("The simulator specified by SimulatorTarget could not be found.");
            }
        }

        /// <summary>
        /// Runs the simulation, and an experiment (<see cref="IExperimentor"/>, if specified), and 
        /// applies a results transform (<see cref="IResultsTransform"/>, if specified). After simulation
        /// has been run, the network can be retrieved for plotting/analysis.
        /// </summary>
        public void Execute()
        {
            if (!CacheNetwork || _initialNetwork == null)
            {
                simulator.PrepareNetwork(NetworkFilename);
                _initialNetwork = simulator.GetNetworkModel();
                Network = _initialNetwork;
            }

            //if we're supposed to be running an Experiment,
            if (ExperimentDriver != null)
            {
                //Notify the Results Transformer of preliminary (pre-experiment)
                // results.
                if (ResultsTransformer != null)
                {
                    ResultsTransformer.PreExperimentHook(_initialNetwork);
                }
                //Run the Experiment commands.
                foreach (String str in ExperimentDriver.Experiment(_initialNetwork))
                {
                    simulator.RunCommand(str);
                }

                //Solve the network again.
                Network = simulator.GetNetworkModel();
            }
            //If we're using a Results Transformer, use it to finalise results.
            if (ResultsTransformer != null)
            {
                ResultsTransformer.PostExperimentHook(Network);
            }
        }
    }
}
