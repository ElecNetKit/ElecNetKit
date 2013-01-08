using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ElecNetKit.NetworkModelling;
using ElecNetKit.Experimentation;
using ElecNetKit.Transform;

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
        /// Instantiates a <see cref="NetworkController"/> using the provided simulator.
        /// </summary>
        /// <param name="sim">The simulator object to use for simulations.</param>
        public NetworkController(ISimulator sim)
        {
            simulator = sim;
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
