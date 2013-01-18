using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ElecNetKit;
using ElecNetKit.Simulator;
using ElecNetKit.Convenience;

using System.IO;
using System.Linq;

using ElecNetKit.Engines;
using ElecNetKit.NetworkModelling;
using ElecNetKit.Util;

namespace Tests
{
    [TestClass]
    public class ElecNetKitTests
    {
        public NetworkModel GetNetwork()
        {
            NetworkController sim = new NetworkController(null);
            sim.Execute();
            return sim.Network;
        }

        [TestMethod]
        public void TestWithSimpleNetwork()
        {
            var model = GetNetwork();

            Assert.AreEqual(4, model.Buses.Count, "Num Buses");
            Assert.AreEqual(4, model.Loads.Count, "Num Loads");
            Assert.AreEqual(4, model.Lines.Count, "Num Lines");
            Assert.AreEqual(0.88408, model.Buses["b4"].VoltagePU.Magnitude, 0.00001, "Lowest Voltage");
            Assert.AreEqual(model.LosseskVA.Real, 19511.1, 0.1, "Losses");
        }

        [TestMethod]
        public void TestSerialisation()
        {
            var oldModel = GetNetwork();
            var oldStream = new MemoryStream();
            oldModel.Serialise(oldStream);
            oldStream.Seek(0, SeekOrigin.Begin);
            NetworkModel model = (NetworkModel) QuickSerialisers.Deserialise(oldStream);
            var newStream = new MemoryStream();
            model.Serialise(newStream);
            Assert.IsTrue(newStream.ToArray().SequenceEqual(oldStream.ToArray()));
        }

        public void TestNetworkElementConnectivity()
        {
        }
    }
}
