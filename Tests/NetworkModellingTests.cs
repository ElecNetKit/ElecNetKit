using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ElecNetKit.NetworkModelling;
using ElecNetKit.Simulator;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class NetworkModellingTests
    {
        public NetworkModel GetNetwork(String networkName)
        {
            NetworkController sim = new NetworkController(new BogusSimulator());
            sim.NetworkFilename = networkName;
            sim.Execute();
            return sim.Network;
        }

        [TestMethod]
        public void TestModelConstructionIntegrityBalanced()
        {
            var network = GetNetwork("balanced");

            Assert.AreEqual(4, network.Buses.Count);
            Assert.AreEqual(4, network.Loads.Count);
            Assert.AreEqual(4, network.Lines.Count);
            Assert.AreEqual(1, network.Generators.Count);
            Assert.IsTrue(ConnectedToElemWithID(network.Buses["b1"], (IEnumerable<NetworkElement>)network.Lines, "b1b2"));
            Assert.IsTrue(ConnectedToElemWithID(network.Buses["b1"], (IEnumerable<NetworkElement>)network.Lines, "b1b3"));
            Assert.IsFalse(ConnectedToElemWithID(network.Buses["b1"], (IEnumerable<NetworkElement>)network.Lines, "b3b4"));
            Assert.AreEqual(network.Buses["b1"], network.Loads[0].ConnectedTo.Single());
        }

        public bool ConnectedToElemWithID(NetworkElement thisElem, IEnumerable<NetworkElement> elements, String id)
        {
            return thisElem.ConnectedTo.Contains(elements.Single(x => x.ID == id));
        }

        [TestMethod]
        public void TestModelIntegritySerialisationLoop()
        {
            var oldModel = GetNetwork("balanced");
            var oldStream = new MemoryStream();
            oldModel.Serialise(oldStream);
            oldStream.Seek(0, SeekOrigin.Begin);
            NetworkModel model = (NetworkModel)QuickSerialisers.Deserialise(oldStream);
            Assert.Fail();

        }
    }
}
