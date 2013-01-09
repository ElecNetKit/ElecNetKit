using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region UsingsMWE
using ElecNetKit.Simulator; //Contains NetworkController
using ElecNetKit.Transform; //Contains DifferenceTransform
using ElecNetKit.Engines;   //Contains OpenDSSSimulator
#endregion
#region UsingsGraphing
using ElecNetKit.Graphing.Graphs;   //Contains ValueTransformableTreeGraph
using ElecNetKit.Graphing.Controls; //Contains GraphHostWindow.
#endregion

namespace MinimalWorkingExample
{
    using ElecNetKit.NetworkModelling;
    class Program
    {
        static void Main(string[] args)
        {
#region MainMWE
            //Setup a network controller and choose a network definition file
            // appropriate to the simulator in use
            NetworkController controller = 
                new NetworkController(new OpenDSSSimulator());
            //Set this to the absolute path to the IEEE13mod.dss file, e.g.
            controller.NetworkFilename = @"C:\temp\IEEE13mod.dss";

            //Add our new experiment, with +10% load scaling.
            controller.ExperimentDriver = new LoadScalingExperimentor(1.1);

            //Use a DifferenceTransform so that the controller returns the
            // differences between pre- and post- experiment voltages.
            controller.ResultsTransformer = new DifferenceTransform();

            //Run the simulation
            controller.Execute();

            //Output the change in voltage at every network bus.
            foreach (var bus in controller.Network.Buses.Values)
            {
                System.Diagnostics.Debug.WriteLine(
                "Bus " + bus.ID + " has changed in voltage (pu) by "
                + bus.VoltagePU.Magnitude);
            }
#endregion
#region MainGraphing
            var graph = new ValueTransformableTreeGraph();  // make a new graph.
            graph.Network = controller.Network;             // assign the output of the experiment to the graph.
            GraphHostWindow.StartGraphHostWindow(graph);    // put the graph in a window and display it.
#endregion
            otherGraph(controller.Network);
        }

        static void otherGraph(NetworkModel network)
        {
            #region ValueTransform
            var graph = new ValueTransformableTreeGraph(); //new graph
            graph.BusSizeMin = 2;
            graph.BusSizeMax = 10;
            graph.BusSizeTransform = bus => bus.VoltagePU.Magnitude;
            graph.BusColorTransform = bus => bus.VoltagePU.Magnitude;
            graph.RingEnabledTransform = bus => bus.ConnectedTo.OfType<Generator>().Any();
            graph.RingDistanceTransform = bus => 2; //constant
            graph.Network = network;
            #endregion
            GraphHostWindow.StartGraphHostWindow(graph);
        }
    }
}
