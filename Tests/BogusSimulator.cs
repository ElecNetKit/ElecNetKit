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
using ElecNetKit.NetworkModelling.Phasing;

namespace Tests
{
    class BogusSimulator : ISimulator
    {
        NetworkModel model;
        public void RunCommand(string command)
        {
            //don't do anything.
        }

        public void PrepareNetwork(string filename)
        {
            if (filename.ToLower() == "balanced")
                PrepBalanced();
            else
                PrepUnbalanced();
        }

        public void PrepBalanced()
        {
            Dictionary<String, Bus> buses = new Dictionary<string, Bus>();
            buses["b1"] = new Bus("b1", pr(132.79,0), 132.79, new Point(0, 0));
            buses["b2"] = new Bus("b2", pr(133.73,-0.1), 132.79, new Point(0, 0));
            buses["b3"] = new Bus("b3", pr(133.55,-0.1), 132.79, new Point(0, 0));
            buses["b4"] = new Bus("b4", pr(133.99,-0.1), 132.79, new Point(0, 0));

            var lb1b2 = new Line("b1b2", 1);
            lb1b2.Connect(buses["b1"], buses["b2"]);
            var lb1b3 = new Line("b1b3", 2);
            lb1b3.Connect(buses["b1"], buses["b3"]);
            var lb2b4 = new Line("b2b4", 3);
            lb2b4.Connect(buses["b2"], buses["b4"]);
            var lb3b4 = new Line("b3b4", 4);
            lb3b4.Connect(buses["b3"], buses["b4"]);

            Collection<Line> lines = new Collection<Line>();
            lines.Add(lb1b2);
            lines.Add(lb1b3);
            lines.Add(lb2b4);
            lines.Add(lb3b4);

            Generator g = new Generator("g4", new Complex(318000, 0));

            g.Connect(buses["b4"]);
            Collection<Generator> generators = new Collection<Generator>();
            generators.Add(g);

            Collection<Load> loads = new Collection<Load>();
            var l1 = new Load("l1", new Complex(50000,30990));
            l1.Connect(buses["b1"]);
            var l2 = new Load("l2", new Complex(170000, 105350));
            l2.Connect(buses["b2"]);
            var l3 = new Load("l3", new Complex(200000,12940));
            l3.Connect(buses["b3"]);
            var l4 = new Load("l4", new Complex(80000, 49580));
            l4.Connect(buses["b4"]);
            loads.Add(l1);
            loads.Add(l2);
            loads.Add(l3);
            loads.Add(l4);

            model = new NetworkModel(buses, lines, loads, generators, new Complex(19580, -17810600), buses["b1"]);
        }
        public void PrepUnbalanced()
        {
        }

        /// <summary>
        /// Synthesises a fake network with really well defined characteristics. Only useful for unit testing.
        /// </summary>
        /// <returns>A standard network.</returns>
        public ElecNetKit.NetworkModelling.NetworkModel GetNetworkModel()
        {
            return model;
        }

        private Complex pr(double mag, double angle)
        {
            return Complex.FromPolarCoordinates(mag, angle * Math.PI / 180);
        }

    }
}
