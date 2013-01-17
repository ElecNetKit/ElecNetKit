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
using ElecNetKit.NetworkModelling.Phasing;

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
                var bus = circuit.Buses[busIdx];
                // voltages are stored in OpenDSS as a double[2n], where n is the number of phases.
                // the numbers are stored in complex pairs per phase.
                var rawVoltage = bus.Voltages;

                String busID = bus.Name;
                var phases = (int[])bus.Nodes;
                PhasedValues<Complex> voltages = new PhasedValues<Complex>();
                for (int i = 0; i < phases.Length; i++)
                {
                    voltages[phases[i]] = new Complex(rawVoltage[2 * i], rawVoltage[2 * i + 1]);
                }

                //NOTE NEGATIVE FOR THE BUS Y AXIS. The Y coordinate used
                // by openDSS is opposite to that used in .net. So flip
                // it here.
                results.Add(busID, new Bus(busID,
                                            voltages,
                                            bus.kVBase * 1000,
                                            bus.Coorddefined ? (Point?)new Point(bus.x, -bus.y) : null
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

                var bus1 = ResolveOpenDSSBusString(lines.Bus1, lines.Phases);
                var bus2 = ResolveOpenDSSBusString(lines.Bus2, lines.Phases);
                
                if (Buses.ContainsKey(bus1.Item1) && Buses.ContainsKey(bus2.Item1))
                {
                    line.Connect(Buses[bus1.Item1], bus1.Item2, Buses[bus2.Item1], bus2.Item2);
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

            //more OpenDSS quirk handling. if there are no loads, OpenDSS returns
            // an array of length 1 with the item "NONE". go figure.
            if (loads[0] == "NONE")
            {
                return results;
            }

            //loop through the loads, connect the appropriate buses and add them.
            foreach (String loadName in loads)
            {
                var dssLoad = dss.ActiveCircuit.CktElements["load." + loadName];
                if (!dssLoad.Enabled)
                    continue;
                var rawPowers = (double[])dssLoad.Powers;
                var powers = new PhasedValues<Complex>();
                for (int i = 0; i < dssLoad.NumPhases; i++)
                {
                    powers[i + 1] = new Complex(rawPowers[2*i], rawPowers[2*i + 1]);
                }

                Load load = new Load(loadName, powers);

                var busConnectionInfo = ResolveOpenDSSBusString((String)dssLoad.BusNames[0], dssLoad.NumPhases);
                if (Buses.ContainsKey(busConnectionInfo.Item1))
                {
                    load.ConnectWye(Buses[busConnectionInfo.Item1],powers.Keys,busConnectionInfo.Item2);
                }
                results.Add(load);
            }
            return results;
        }

        /// <summary>
        /// Translates an OpenDSS bus connection string into a bus ID and a set of phases.
        /// </summary>
        /// <param name="busConnString">The bus connection string from OpenDSS.</param>
        /// <param name="numPhases"></param>
        /// <returns></returns>
        protected static Tuple<String, List<int>> ResolveOpenDSSBusString(String busConnString, int numPhases)
        {
            var parts = busConnString.Split('.');
            List<int> phases;
            //parts[0] is always the actual bus name, the rest is phasing.
            if (parts.Length == 0)
                throw new Exception();

            phases = parts.Skip(1).Take(numPhases).Select(phaseStr => int.Parse(phaseStr)).ToList();

            while (phases.Count < numPhases)
            {
                var autoFillPhase = (phases.Count > 0) ? phases[phases.Count - 1] + 1 : 1;
                
                if (autoFillPhase > 3)
                    autoFillPhase = 1;
                
                phases.Add(autoFillPhase);
            }
            return new Tuple<String, List<int>>(parts[0], phases);
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
                var dssGenerator = dss.ActiveCircuit.CktElements["generator." + generatorName];
                if (!dssGenerator.Enabled)
                    continue;
                var rawPowers = (double[])dssGenerator.Powers;
                var powers = new PhasedValues<Complex>();
                for (int i = 0; i < dssGenerator.NumPhases; i++)
                {
                    powers[i + 1] = new Complex(-rawPowers[2 * i], -rawPowers[2 * i + 1]);
                }
                Generator gen = new Generator(generatorName, powers);


                var busConnectionInfo = ResolveOpenDSSBusString((String)dssGenerator.BusNames[0], dssGenerator.NumPhases);
                if (Buses.ContainsKey(busConnectionInfo.Item1))
                {
                    gen.ConnectWye(Buses[busConnectionInfo.Item1], powers.Keys, busConnectionInfo.Item2);
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
