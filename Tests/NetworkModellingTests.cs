using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ElecNetKit.NetworkModelling;
using ElecNetKit.Simulator;
using System.IO;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class NetworkModellingTests
    {
        public NetworkModel GetNetwork()
        {
            NetworkController sim = new NetworkController(new BogusSimulator());
            sim.Execute();
            return sim.Network;
        }

        [TestMethod]
        public void TestModelIntegritySerialisationLoop()
        {
            var oldModel = GetNetwork();
            var oldStream = new MemoryStream();
            oldModel.Serialise(oldStream);
            oldStream.Seek(0, SeekOrigin.Begin);
            NetworkModel model = (NetworkModel)QuickSerialisers.Deserialise(oldStream);
            Assert.Fail();

        }
    }
}
