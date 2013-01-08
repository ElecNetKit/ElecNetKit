using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;

using ElecNetKit.NetworkModelling;
using ElecNetKit.Util;
using ElecNetKit.Simulator;
using System.Numerics;

namespace ElecNetKit.Engines
{
    /// <summary>
    /// A simulator backend that connects ElecNetKit to the OpenDSS Engine.
    /// Should rarely be used directly, only instantiated and passed to the
    /// <see cref="ElecNetKit.Simulator.NetworkController"/> constructor.
    /// If you're looking to get information, obtain results etc, use a
    /// <see cref="NetworkController"/> instead.
    /// </summary>
    public class OpenDSSSimulator : ISimulator
    {
        /// <summary>
        /// The OpenDSS interface.
        /// </summary>
        private OpenDSSengine.DSS dss;

        /// <summary>
        /// The required accuracy of convergence (in pu voltage) of the results.
        /// Defaults to 1e-7. Increase this for easier and faster convergence. This
        /// level of accuracy is required for sensitivity applications.
        /// </summary>
        public Double ResultAccuracy { set; get; }

        /// <summary>
        /// Instantiates a new <see cref="OpenDSSSimulator"/>.
        /// </summary>
        public OpenDSSSimulator()
        {
            ResultAccuracy = 1e-7;

            //Start DSS engine, throw exception if it didn't work.
            dss = new OpenDSSengine.DSS();
            if (!dss.Start(0))
            {
                throw new Exception("Could not start OpenDSS Engine.");
            }
        }

        /// <summary>
        /// Run a single OpenDSS command through the OpenDSS Text interface.
        /// </summary>
        /// <param name="command">The OpenDSS command to run.</param>
        public void RunCommand(String command)
        {
            dss.Text.Command = command;

            if (dss.Error.Number != 0)
            {
                throw new Exception("OpenDSS Error: " + dss.Error.Description);
            }
        }

        /// <summary>
        /// Obtains a model of the network from the DSS Engine.
        /// </summary>
        /// <returns>A network model for the OpenDSS Active Circuit.</returns>
        public NetworkModel GetNetworkModel()
        {
            //Solve the network, to get the parameter info out.
            RunCommand("solve mode=snap");
            Dictionary<String, Bus> Buses = GetBuses();
            return new NetworkModel(Buses, GetLines(Buses), GetLoads(Buses), GetGenerators(Buses), GetLosseskVA(), GetSourceBus(Buses));
        }

        private Complex GetLosseskVA()
        {
            var losses = dss.ActiveCircuit.Losses;
            return new Complex(losses[0], losses[1]) / 1000;
        }

        /// <summary>
        /// Obtains a list of all buses from OpenDSS.
        /// </summary>
        /// <returns>A Dictionary of Buses, indexed by ID String.
        /// This is required in order to link the other network elements.
        /// </returns>
        private Dictionary<String, Bus> GetBuses()
        {
            Dictionary<String, Bus> results = new Dictionary<String, Bus>();
            OpenDSSengine.Circuit circuit = dss.ActiveCircuit;
            for (int busIdx = 0; busIdx < circuit.NumBuses; busIdx++)
            {
                //voltages are stored in OpenDSS as a double[6].
                // the numbers are stored in complex pairs, for positive,
                // negative and zero sequence.
                var voltage = circuit.Buses[busIdx].Voltages;

                String busID = circuit.Buses[busIdx].Name;

                //NOTE NEGATIVE FOR THE BUS Y AXIS. The Y coordinate used
                // by openDSS is opposite to that used in .net. So flip
                // it here.
                results.Add(busID, new Bus(busID,
                                            new Complex(voltage[0], voltage[1]),
                                            circuit.Buses[busIdx].kVBase * 1000,
                                            circuit.Buses[busIdx].Coorddefined ? (Point?)new Point(circuit.Buses[busIdx].x, -circuit.Buses[busIdx].y) : null
                                          )
                            );
            }
            return results;
        }

        private Bus GetSourceBus(Dictionary<String, Bus> Buses)
        {
            String busID = dss.ActiveCircuit.Buses[0].Name;
            return Buses[busID];
        }

        /// <summary>
        /// Obtain a collection of lines.
        /// </summary>
        /// <param name="Buses">A Dictionary of buses indexed by ID.
        /// Used for determining which buses the lines link to.</param>
        /// <returns>All the lines in the Active Circuit.</returns>
        private Collection<Line> GetLines(Dictionary<String, Bus> Buses)
        {
            Collection<Line> results = new Collection<Line>();
            OpenDSSengine.Lines lines = dss.ActiveCircuit.Lines;

            //due to the bizarre COM setup for the Lines in the system, we need to access Readonly property
            // 'First' to start the iteration process.
            int test = lines.First;
            //now loop through the rest of the lines and add them to the collection.
            do
            {
                Line line = new Line(lines.Name, lines.Length);
                String bus;
                bus = lines.Bus1;
                if (bus.Contains('.'))
                    bus = bus.Substring(0, bus.IndexOf('.'));
                if (Buses.ContainsKey(bus))
                {
                    line.Connect(Buses[bus]);
                }
                bus = lines.Bus2;
                if (bus.Contains('.'))
                    bus = bus.Substring(0, bus.IndexOf('.'));
                if (Buses.ContainsKey(bus))
                {
                    line.Connect(Buses[bus]);
                }
                results.Add(line);
            } while (lines.Next != 0);
            return results;
        }

        /// <summary>
        /// Obtains all loads in the Active Circuit.
        /// </summary>
        /// <param name="Buses">A Dictionary of buses indexed by ID.
        /// Used for determining which buses the loads are on.</param>
        /// <returns>All loads in the Active Circuit.</returns>
        private Collection<Load> GetLoads(Dictionary<String, Bus> Buses)
        {
            Collection<Load> results = new Collection<Load>();
            String[] loads = dss.ActiveCircuit.Loads.AllNames;

            //more OpenDSS quirk handling. if there are no generators, OpenDSS returns
            // an array of length 1 with the item "NONE". go figure.
            if (loads[0] == "NONE")
            {
                return results;
            }

            //loop through the loads, connect the appropriate buses and add them.
            foreach (String loadName in loads)
            {
                var dssLoad = dss.ActiveCircuit.CktElements["load." + loadName];
                String busID = (String)dssLoad.Properties["bus1"].Val;
                double kW = double.Parse(dssLoad.Properties["kW"].Val);
                double kvar = double.Parse(dssLoad.Properties["kvar"].Val);

                Load load = new Load(loadName, new Complex(kW, kvar));
                if (Buses.ContainsKey(busID))
                {
                    load.Connect(Buses[busID]);
                }
                results.Add(load);
            }
            return results;
        }

        /// <summary>
        /// Obtains all generators in the Active Circuit.
        /// </summary>
        /// <param name="Buses">A Dictionary of buses indexed by ID.
        /// Used for determining which buses the loads are on.</param>
        /// <returns>All generators in the Active Circuit.</returns>
        private Collection<Generator> GetGenerators(Dictionary<String, Bus> Buses)
        {
            Collection<Generator> results = new Collection<Generator>();
            String[] generators = dss.ActiveCircuit.Generators.AllNames;
            //more OpenDSS quirk handling. if there are no generators, OpenDSS returns
            // an array of length 1 with the item "NONE". go figure.
            if (generators[0] == "NONE")
            {
                return results;
            }
            //loop through the generators, connect the appropriate buses and add them.
            foreach (String generatorName in generators)
            {


                OpenDSSengine.CktElement dss_generator_reference = dss.ActiveCircuit.CktElements["generator." + generatorName];
                String busID = (String)dss_generator_reference.Properties["bus1"].Val;
                double[] powers = dss_generator_reference.Powers;
                Complex dss_gen_amount = new Complex(-(powers[0] + powers[2] + powers[4]), -(powers[1] + powers[3] + powers[5]));

                Generator gen = new Generator(generatorName, dss_gen_amount);
                if (Buses.ContainsKey(busID))
                {
                    gen.Connect(Buses[busID]);
                }
                results.Add(gen);
            }
            return results;
        }

        /// <summary>
        /// Provides a list of all properties for an item.
        /// </summary>
        /// <param name="fullID">The ID in terms of object type and object
        /// name the information is to be obtained for,
        /// e.g. generator.gen-1</param>
        /// <returns>A Dictionary of all properties for the specified item.
        /// the Key is the property name, the String in the Tuple is the
        /// property description and the Object in the Tuple is value.</returns>
        private Dictionary<String, Object> GetProperties(String fullID)
        {
            Dictionary<String, Object> results = new Dictionary<string, Object>();
            if (fullID.Substring(0, fullID.IndexOf('.')).ToLower() == "bus")
            {
                String busID = fullID.Substring(fullID.IndexOf('.') + 1);
                PopulateBusPropertyDictionary(results, busID);
            }
            else
            {
                PopulateElementPropertyDictionary(results, fullID);
            }
            return results;
        }

        private void PopulateBusPropertyDictionary(Dictionary<String, Object> properties, String busID)
        {
            var bus = dss.ActiveCircuit.Buses[busID];
            properties["kVBase"] = bus.kVBase;
            properties["Name"] = bus.Name;
            if (bus.Coorddefined)
            {
                properties["x"] = bus.x;
                properties["y"] = bus.y;
            }
        }

        private void PopulateElementPropertyDictionary(Dictionary<String, Object> properties, String fullID)
        {
            int elemID = dss.ActiveCircuit.SetActiveElement(fullID);
            if (elemID == -1)
                throw new Exception("Couldn't get the element details.");
            OpenDSSengine.CktElement dssElem = dss.ActiveCircuit.ActiveCktElement;
            foreach (String prop in dssElem.AllPropertyNames)
            {
                properties.Add(prop, dssElem.Properties[prop].Val);
            }
        }

        /// <summary>
        /// Loads a network from <paramref name="filename"/>, but does not solve.
        /// </summary>
        /// <param name="filename">The filename to load the network from.</param>
        public void PrepareNetwork(string filename)
        {
            //Compile the specified network master file.
            RunCommand("clear");
            RunCommand("compile \"" + filename + "\"");
            RunCommand("Set Tolerance=" + this.ResultAccuracy);
        }
    }
}
