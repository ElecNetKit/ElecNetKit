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
        public void TestLoadUnbalanced()
        {
            var network = GetNetwork("Ex9_5_unbal");
            var load = network.Buses["b1"].ConnectedTo.OfType<Load>().Single();
            AssertComplexEqual(new Complex(16381, 10280), load.ActualKVAPhased[1], 10);
            AssertComplexEqual(new Complex(16852, 10108), load.ActualKVAPhased[2], 10);
            AssertComplexEqual(new Complex(16766, 10602), load.ActualKVAPhased[3], 10);

            load = network.Buses["b2"].ConnectedTo.OfType<Load>().Single();
            AssertComplexEqual(new Complex(85000, 52674), load.ActualKVAPhased[1], 10);
            AssertComplexEqual(new Complex(85001, 52673), load.ActualKVAPhased[2], 10);
            Assert.IsFalse(load.ActualKVAPhased.ContainsKey(3), "Contains Phase 3");
            Assert.IsTrue(load.ConnectionExists(1,network.Buses["b2"],2));
            Assert.IsTrue(load.ConnectionExists(2,network.Buses["b2"],3));
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
