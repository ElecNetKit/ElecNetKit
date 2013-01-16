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
            ModelIntegrityAssertionsBalanced(network);
        }

        [TestMethod]
        public void TestModelConstructionIntegrityUnbalanced()
        {
            var network = GetNetwork("unbalanced");
            ModelIntegrityAssertionsUnbalanced(network);
        }

        [TestMethod]
        public void TestModelSerialisationIntegrityBalanced()
        {
            var oldModel = GetNetwork("balanced");
            var oldStream = new MemoryStream();
            oldModel.Serialise(oldStream);
            oldStream.Seek(0, SeekOrigin.Begin);
            NetworkModel model = (NetworkModel)QuickSerialisers.Deserialise(oldStream);
            ModelIntegrityAssertionsBalanced(model);
        }

        public void ModelIntegrityAssertionsUnbalanced(NetworkModel network)
        {
            Assert.AreEqual(4, network.Buses.Count);
            Assert.AreEqual(4, network.Loads.Count);
            Assert.AreEqual(4, network.Lines.Count);
            Assert.AreEqual(1, network.Generators.Count);
            Assert.AreEqual(3, network.Buses["b1"].ConnectedToAnyPhase.Count());
            Assert.AreEqual(4, network.Buses["b1"].ConnectedToPhased.Count);
            //3 connections per line, 6 for the load (3Phase+3Neutral).
            Assert.AreEqual(3+3+6, network.Buses["b1"].ConnectedToPhased.Values.SelectMany(x=>x).Count());
            //3 connections for one line, 2 for another, 2 for the load (1 load L-L).
            Assert.AreEqual(3 + 2 + 2, network.Buses["b3"].ConnectedToPhased.Values.SelectMany(x => x).Count());
            foreach(var i in new[] {1,2,3,0})
                Assert.IsTrue(NetworkElement.ConnectionExists(network.Buses["b4"], 1, network.Loads[3], 1));
            

        }

        public void ModelIntegrityAssertionsBalanced(NetworkModel network)
        {
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
    }
}
