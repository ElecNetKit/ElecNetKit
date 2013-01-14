using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ElecNetKit.Simulator;
using ElecNetKit.NetworkModelling;
using System.Numerics;
using System.Windows;
using System.Collections.ObjectModel;

namespace Tests
{
       class BogusSimulator : ISimulator
       {

           public void RunCommand(string command)
           {
               //don't do anything.
           }

           public void PrepareNetwork(string filename)
           {
               //don't do anything.
           }

           /// <summary>
           /// Synthesises a fake network with really well defined characteristics. Only useful for unit testing.
           /// </summary>
           /// <returns>A standard network.</returns>
           public ElecNetKit.NetworkModelling.NetworkModel GetNetworkModel()
           {
               Dictionary<String, Bus> buses = GetBuses();
               return new NetworkModel(buses, GetLines(buses), GetLoads(buses),
                   new Collection<Generator>(), new Complex(110.544,322.316), buses["RG60"]);
           }

           private static Collection<Load> GetLoads(Dictionary<String, Bus> buses)
           {
               Collection<Load> loads = new Collection<Load>();
               var load = new Load("611", c(170, 80));
               loads.Add(load);
               NetworkElement.Connect(load, buses["611"], 3);
               load = new Load("645", c(170, 125));
               loads.Add(load);
               NetworkElement.Connect(load, buses["645"], 2);
               load = new Load("646", c(230, 132));
               loads.Add(load);
               NetworkElement.Connect(load, buses["646"], 2,3);
               load = new Load("652", c(128, 86));
               loads.Add(load);
               NetworkElement.Connect(load, buses["652"], 1);
               load = new Load("670A", c(17, 10));
               loads.Add(load);
               NetworkElement.Connect(load, buses["670"], 1);
               load = new Load("670B", c(66, 38));
               loads.Add(load);
               NetworkElement.Connect(load, buses["670"], 2);
               load = new Load("670C", c(117, 68));
               loads.Add(load);
               NetworkElement.Connect(load, buses["670"], 3);
               load = new Load("671", c(1155, 660));
               loads.Add(load);
               NetworkElement.Connect(load, buses["671"], 1,2,3);
               load = new Load("675A", c(485, 190));
               loads.Add(load);
               NetworkElement.Connect(load, buses["675"], 1);
               load = new Load("675B", c(68, 60));
               loads.Add(load);
               NetworkElement.Connect(load, buses["675"], 2);
               load = new Load("675C", c(290, 212));
               loads.Add(load);
               NetworkElement.Connect(load, buses["675"], 3);
               load = new Load("692", c(170, 151));
               loads.Add(load);
               NetworkElement.Connect(load, buses["692"], 3,1);
               return loads;
           }

           private static Complex c(double r, double j)
           {
               return new Complex(r, j);
           }

           private static Collection<Line> GetLines(Dictionary<String, Bus> buses)
           {
               //Lines
               Collection<Line> lines = new Collection<Line>();
               var line = new Line("632633", 500);
               NetworkElement.Connect(line, buses["632"], 1, 2, 3);
               NetworkElement.Connect(line, buses["633"], 1, 2, 3);
               lines.Add(line);
               line = new Line("632645", 500);
               NetworkElement.Connect(line, buses["632"], 3, 2);
               NetworkElement.Connect(line, buses["645"], 3, 2);
               lines.Add(line);
               line = new Line("632670", 667);
               NetworkElement.Connect(line, buses["632"], 1, 2, 3);
               NetworkElement.Connect(line, buses["670"], 1, 2, 3);
               lines.Add(line);
               line = new Line("645646", 300);
               NetworkElement.Connect(line, buses["645"], 3, 2);
               NetworkElement.Connect(line, buses["646"], 3, 2);
               lines.Add(line);
               line = new Line("650632", 2000);
               NetworkElement.Connect(line, buses["RG60"], 1, 2, 3);
               NetworkElement.Connect(line, buses["632"], 1, 2, 3);
               lines.Add(line);
               line = new Line("670671", 1333);
               NetworkElement.Connect(line, buses["670"], 1, 2, 3);
               NetworkElement.Connect(line, buses["671"], 1, 2, 3);
               lines.Add(line);
               line = new Line("671680", 1000);
               NetworkElement.Connect(line, buses["680"], 1, 2, 3);
               NetworkElement.Connect(line, buses["671"], 1, 2, 3);
               lines.Add(line);
               line = new Line("671684", 1333);
               NetworkElement.Connect(line, buses["684"], 1, 3);
               NetworkElement.Connect(line, buses["671"], 1, 3);
               lines.Add(line);
               line = new Line("671692", 0.001);
               NetworkElement.Connect(line, buses["692"], 1, 2, 3);
               NetworkElement.Connect(line, buses["671"], 1, 2, 3);
               lines.Add(line);
               line = new Line("684611", 300);
               NetworkElement.Connect(line, buses["684"], 3);
               NetworkElement.Connect(line, buses["611"], 3);
               lines.Add(line);
               line = new Line("684652", 800);
               NetworkElement.Connect(line, buses["684"], 1);
               NetworkElement.Connect(line, buses["652"], 1);
               lines.Add(line);
               line = new Line("692675", 500);
               NetworkElement.Connect(line, buses["692"], 1, 2, 3);
               NetworkElement.Connect(line, buses["675"], 1, 2, 3);
               lines.Add(line);
               return lines;
           }

           private Dictionary<String, Bus> GetBuses()
           {
               //Buses
               List<Bus> buses = new List<Bus>();
               buses.Add(bus("RG60", pr(2.5514, 0.0), pr(2.5216, -120.0), pr(2.5664, 120.0), 4.160, null));
               buses.Add(bus("650", pr(2.4016, 0.0), pr(2.4017, -120.0), pr(2.4016, 120.0), 4.160, null));
               buses.Add(bus("633", pr(2.4444, -2.6), pr(2.4977, -121.8), pr(2.4375, 117.8), 4.160, null));
               buses.Add(bus("671", pr(2.3762, -5.3), pr(2.5297, -122.4), pr(2.3512, 116.1), 4.160, null));
               buses.Add(bus("645", 2, pr(2.4802, -121.9), 3, pr(2.439, 117.8), 4.160, null));
               buses.Add(bus("646", 2, pr(2.476, -122.0), 3, pr(2.4341, 117.9), 4.160, null));
               buses.Add(bus("692", pr(2.3762, -5.3), pr(2.5297, -122.4), pr(2.3512, 116.1), 4.160, null));
               buses.Add(bus("675", pr(2.3606, -5.6), pr(2.5354, -122.5), pr(2.3467, 116.1), 4.160, null));
               buses.Add(bus("611", 3, pr(2.3416, 115.8), 4.160, null));
               buses.Add(bus("652", 1, pr(2.3582, -5.3), 4.160, null));
               buses.Add(bus("670", pr(2.427, -3.4), pr(2.5094, -122.0), pr(2.4098, 117.2), 4.160, null));
               buses.Add(bus("632", pr(2.4517, -2.5), pr(2.5022, -121.7), pr(2.4438, 117.8), 4.160, null));
               buses.Add(bus("680", pr(2.3762, -5.3), pr(2.5297, -122.4), pr(2.3512, 116.1), 4.160, null));
               buses.Add(bus("684", 1, pr(2.3716, -5.3), 3, pr(2.3464, 116.0), 4.160, null));
               return buses.ToDictionary(b => b.ID, b => b);
           }

           private Complex pr(double mag, double angle)
           {
               return Complex.FromPolarCoordinates(mag, angle * Math.PI / 180);
           }

           private Bus bus(string ID, int a1, Complex p1, int a2, Complex p2, double BaseKV, Point? coords)
           {
               var volts = new PhasedValues<Complex>();
               volts[a1] = p1 * 1000;
               volts[a2] = p2 * 1000;
               return new Bus(ID, volts, BaseKV * 1000, coords);
           }

           private Bus bus(string ID, int a1, Complex p1, double BaseKV, Point? coords)
           {
               var volts = new PhasedValues<Complex>();
               volts[a1] = p1 * 1000;
               return new Bus(ID, volts, BaseKV * 1000, coords);
           }

           private Bus bus(string ID, Complex p1, Complex p2, Complex p3, double BaseKV, Point? coords)
           {
               var volts = new PhasedValues<Complex>();
               volts[1] = p1 * 1000;
               volts[2] = p2 * 1000;
               volts[3] = p3 * 1000;
               return new Bus(ID, volts, BaseKV * 1000, coords);
           }
     }
}
