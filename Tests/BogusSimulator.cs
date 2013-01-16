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
            buses["b1"] = new Bus("b1", pr(132.79, 0), 132.79, new Point(0, 0));
            buses["b2"] = new Bus("b2", pr(133.73, -0.1), 132.79, new Point(0, 0));
            buses["b3"] = new Bus("b3", pr(133.55, -0.1), 132.79, new Point(0, 0));
            buses["b4"] = new Bus("b4", pr(133.99, -0.1), 132.79, new Point(0, 0));

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
            var l1 = new Load("l1", new Complex(50000, 30990));
            l1.Connect(buses["b1"]);
            var l2 = new Load("l2", new Complex(170000, 105350));
            l2.Connect(buses["b2"]);
            var l3 = new Load("l3", new Complex(200000, 12940));
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
            Dictionary<String, Bus> buses = new Dictionary<string, Bus>();
            buses["b1"] = new Bus("b1", pc3pr(130770,0.4,132860,-120.8,134130,120.6), 132.79, new Point(0, 0));
            buses["b2"] = new Bus("b2", pc3pr(866380,99.9,83445,-21.2,86499,-142.5), 132.79, new Point(0, 0));
            buses["b3"] = new Bus("b3", pc3pr(94825,14.5,96205,-107.4,98249,134.2), 132.79, new Point(0, 0));
            buses["b4"] = new Bus("b4", pc3pr(67374,68.6,65606,-54,69131,-174), 132.79, new Point(0, 0));

            var lb1b2 = new Line("b1b2", 1);
            lb1b2.Connect(buses["b1"], new []{1,2,3}, buses["b2"],new[] {2,3,1});
            var lb1b3 = new Line("b1b3", 2);
            lb1b3.Connect3Phase(buses["b1"], buses["b3"]);
            var lb2b4 = new Line("b2b4", 3);
            lb2b4.Connect3Phase(buses["b2"], buses["b4"]);
            var lb3b4 = new Line("b3b4", 4);
            lb3b4.Connect(buses["b3"], buses["b4"], new[] {1,2});

            Collection<Line> lines = new Collection<Line>();
            lines.Add(lb1b2);
            lines.Add(lb1b3);
            lines.Add(lb2b4);
            lines.Add(lb3b4);

            Generator g = new Generator("g4", pc3(106000,0.9797,106000,-1.3930,106000,0.4265));

            g.ConnectWye(buses["b4"]);
            Collection<Generator> generators = new Collection<Generator>();
            generators.Add(g);

            Collection<Load> loads = new Collection<Load>();
            var l1 = new Load("l1", pc3(16382,10280,16852,10108,16766,10601));
            l1.ConnectWye(buses["b1"],1,2,3);
            var l2 = new Load("l2", pcV(2,85000,52674,3,85000,52675));
            l2.ConnectWye(buses["b2"],2,3);
            var l3 = new Load("l3", pcV(1,200000, 12940));
            l3.Connect(1,buses["b3"],1,2);
            var l4 = new Load("l4", pc3(26666,16527,26666,16527,26666,16527));
            l4.ConnectWye(buses["b4"],1,2,3);
            loads.Add(l1);
            loads.Add(l2);
            loads.Add(l3);
            loads.Add(l4);

            model = new NetworkModel(buses, lines, loads, generators, new Complex(140424, 692575), buses["b1"]);
        }

        public Phased<Complex> pc3pr(double M1, double A1, double M2, double A2, double M3, double A3)
        {
            var p = new PhasedValues<Complex>();
            p[1] = pr(M1, A1);
            p[2] = pr(M2, A2);
            p[3] = pr(M3, A3);
            return p;
        }

        public Phased<Complex> pcV(params double[] p)
        {
            var retVal = new PhasedValues<Complex>();
            for (int i = 0; i < p.Length; i += 3)
            {
                retVal[(int)p[i]] = new Complex(p[i + 1], p[i + 2]);
            }
            return retVal;
        }

        public Phased<Complex> pc3(double R1, double I1, double R2, double I2, double R3, double I3)
        {
            var p = new PhasedValues<Complex>();
            p[1] = new Complex(R1, I1);
            p[2] = new Complex(R2, I2);
            p[3] = new Complex(R3, I3);
            return p;
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
