using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Usings
using ElecNetKit.Simulator; //Contains NetworkController
using ElecNetKit.Transform; //Contains DifferenceTransform
using ElecNetKit.Engines;   //Contains OpenDSSSimulator
#endregion
namespace MinimalWorkingExample
{
    class Program
    {
        #region Main
        static void Main(string[] args)
        {
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
            
        }
        #endregion
    }
}
