using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElecNetKit.NetworkModelling;
using ElecNetKit.Simulator;
using ElecNetKit.Engines;

using Tests.Helper;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Numerics;

namespace Tests
{
    [TestClass]
    [DeploymentItem("TestNetworks", "TestNetworks")]
    public class OpenDSSTests
    {
        [TestMethod]
        public void TestLoadBalanced()
        {
            var network = GetNetwork("Ex9_5");
            AssertComplexEqual(new Complex(50000, 30990), network.Buses["b1"].ConnectedTo.OfType<Load>().Single().ActualKVA, 1);
            AssertComplexEqual(new Complex(170000, 105350), network.Buses["b2"].ConnectedTo.OfType<Load>().Single().ActualKVA, 1);
            AssertComplexEqual(new Complex(200000, 123940), network.Buses["b3"].ConnectedTo.OfType<Load>().Single().ActualKVA, 1);
            AssertComplexEqual(new Complex(150000, 49580), network.Buses["b4"].ConnectedTo.OfType<Load>().Single().ActualKVA, 1);
        }

        [TestMethod]
        public void TestBusesBalanced()
        {
            var network = GetNetwork("Ex9_5");
            AssertComplexEqual(pr(132790,0), network.Buses["b1"].Voltage, 100);
            AssertComplexEqual(pr(1, 0), network.Buses["b1"].VoltagePU, 0.001);
            AssertComplexEqual(pr(122970,-1.6), network.Buses["b2"].Voltage, 100);
            AssertComplexEqual(pr(0.92604, -1.6), network.Buses["b2"].VoltagePU, 0.001);
            AssertComplexEqual(pr(123820,-2.3), network.Buses["b3"].Voltage, 100);
            AssertComplexEqual(pr(0.93242, -2.3), network.Buses["b3"].VoltagePU, 0.001);
            AssertComplexEqual(pr(122890,0.9), network.Buses["b4"].Voltage, 100);
            AssertComplexEqual(pr(0.92547, 0.9), network.Buses["b4"].VoltagePU, 0.001);
        }

        [TestMethod]
        public void TestBusesUnbalanced()
        {
            var network = GetNetwork("Ex9_5_unbal");
            AssertComplexEqual(pr(134400, -0.3), network.Buses["b1"].VoltagePhased[1], 100);
            AssertComplexEqual(pr(132450, -119.1), network.Buses["b1"].VoltagePhased[2], 100);
            AssertComplexEqual(pr(130920, 119.6), network.Buses["b1"].VoltagePhased[3], 100);
            AssertComplexEqual(pr(1.0121, -0.3), network.Buses["b1"].VoltagePUPhased[1], 0.001);

            AssertComplexEqual(pr(84344, 97.6), network.Buses["b2"].VoltagePhased[1], 100);
            AssertComplexEqual(pr(87555, -21.4), network.Buses["b2"].VoltagePhased[2], 100);
            AssertComplexEqual(pr(84683, -140), network.Buses["b2"].VoltagePhased[3], 100);
            AssertComplexEqual(pr(0.63772, -140), network.Buses["b2"].VoltagePUPhased[3], 0.001);
        }

        private Complex pr(double mag, double angle)
        {
            return Complex.FromPolarCoordinates(mag, angle * Math.PI / 180);
        }


        [TestMethod]
        public void TestLoadUnbalanced()
        {
            var network = GetNetwork("Ex9_5_unbal");
            var load = network.Buses["b1"].ConnectedTo.OfType<Load>().Single();
            AssertComplexEqual(new Complex(16963, 10361), load.ActualKVAPhased[1],    100);
            AssertComplexEqual(new Complex(16491, 10571), load.ActualKVAPhased[2],    100);
            AssertComplexEqual(new Complex(16545, 10058), load.ActualKVAPhased[3],    100);

            load = network.Buses["b2"].ConnectedTo.OfType<Load>().Single();
            AssertComplexEqual(new Complex(85005, 52675), load.ActualKVAPhased[1],    100);
            AssertComplexEqual(new Complex(84998, 52680), load.ActualKVAPhased[2],    100);
            Assert.IsFalse(load.ActualKVAPhased.ContainsKey(3), "Contains Phase 3");
            Assert.IsTrue(load.ConnectionExists(1,network.Buses["b2"],2));
            Assert.IsTrue(load.ConnectionExists(2,network.Buses["b2"],3));
        }

        [TestMethod]
        public void TestGeneratorsBalanced()
        {
            var network = GetNetwork("Ex9_5");
            var gen = network.Buses["b4"].ConnectedTo.OfType<Generator>().Single();
            AssertComplexEqual(new Complex(318000/3, 0), gen.GenerationPhased[1],    100);
            AssertComplexEqual(new Complex(318000/3, 0), gen.GenerationPhased[2],    100);
            AssertComplexEqual(new Complex(318000/3, 0), gen.GenerationPhased[2],    100);
            Assert.IsTrue(gen.ConnectionExists(1, network.Buses["b4"], 1));
            Assert.IsTrue(gen.ConnectionExists(2, network.Buses["b4"], 2));
            Assert.IsTrue(gen.ConnectionExists(3, network.Buses["b4"], 3));
            Assert.AreEqual(3, gen.ConnectedToPhased.Values.SelectMany(x => x).Count());
        }

        [TestMethod]
        public void TestGeneratorsUnbalanced()
        {
            var network = GetNetwork("Ex9_5_unbal");
            var gen = network.Buses["b4"].ConnectedTo.OfType<Generator>().Single();
            AssertComplexEqual(new Complex(159011, 1.15428), gen.GenerationPhased[1],    100);
            AssertComplexEqual(new Complex(158995, 10.4573), gen.GenerationPhased[2],    100);
            Assert.IsFalse(gen.GenerationPhased.ContainsKey(3), "Contains Phase 3");
            Assert.IsTrue(gen.ConnectionExists(1, network.Buses["b4"], 2));
            Assert.IsTrue(gen.ConnectionExists(2, network.Buses["b4"], 3));
        }

        public void AssertComplexEqual(Complex expected, Complex actual, double precision)
        {
            Assert.AreEqual(expected.Real, actual.Real, precision, "Real parts are not equal.");
            Assert.AreEqual(expected.Imaginary, actual.Imaginary, precision, "Imaginary parts are not equal.");
        }

        //Ex9_5_unbal
        //Ex9_5
        public NetworkModel GetNetwork(String NetworkName)
        {
            NetworkController sim = new NetworkController(new OpenDSSSimulator());
            string path = AppDomain.CurrentDomain.BaseDirectory;
            sim.NetworkFilename = path + @"\TestNetworks\"+NetworkName+".dss";
            sim.Execute();
            return sim.Network;
        }

        [TestMethod]
        public void TestResolveOpenDSSString_A()
        {
            var retVal = PrivateMemberAccessor.RunStaticMethod(typeof(OpenDSSSimulator), "ResolveOpenDSSBusString", "BusA.2.3.1",3);
            ResolveOpenDSSStringExpect(retVal, "BusA", new[] {2,3,1});
        }

        [TestMethod]
        public void TestResolveOpenDSSString_B()
        {
            var retVal = PrivateMemberAccessor.RunStaticMethod(typeof(OpenDSSSimulator), "ResolveOpenDSSBusString", "BusA.1.3.2", 2);
            ResolveOpenDSSStringExpect(retVal, "BusA", new[] { 1, 3});
        }

        [TestMethod]
        public void TestResolveOpenDSSString_C()
        {
            var retVal = PrivateMemberAccessor.RunStaticMethod(typeof(OpenDSSSimulator), "ResolveOpenDSSBusString", "BusA", 3);
            ResolveOpenDSSStringExpect(retVal, "BusA", new[] { 1, 2,3 });
        }

        [TestMethod]
        public void TestResolveOpenDSSString_D()
        {
            var retVal = PrivateMemberAccessor.RunStaticMethod(typeof(OpenDSSSimulator), "ResolveOpenDSSBusString", "BusA.2", 3);
            ResolveOpenDSSStringExpect(retVal, "BusA", new[] { 2,3,1 });
        }

        public void ResolveOpenDSSStringExpect(Object retVal, String busName, ICollection phases)
        {
            var vals = (Tuple<String, List<int>>)retVal;
            Assert.AreEqual(busName, vals.Item1);
            CollectionAssert.AreEqual(phases, vals.Item2);
        }
    }
}
