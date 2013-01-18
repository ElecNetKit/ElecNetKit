using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ElecNetKit.NetworkModelling;
using System.Numerics;

using System.Linq;

namespace Tests
{
    [TestClass]
    public class PowerConversionElementTests
    {
        [TestMethod]
        public void Connect()
        {
            FakePCElement pcElem = new FakePCElement();
            Bus b = new Bus("Test", Complex.Zero, 0, null);
            pcElem.Connect(1, b, 1);
            PairConnectionAssertions(pcElem, 1, b, 1);
        }

        [TestMethod]
        public void ConnectWye()
        {
            FakePCElement pcElem = new FakePCElement();
            Bus b = new Bus("Test", Complex.Zero, 0, null);
            pcElem.ConnectWye(b, 1, 2, 3);
            PairConnectionAssertions(pcElem, 1, b, 1);
            PairConnectionAssertions(pcElem, 2, b, 2);
            PairConnectionAssertions(pcElem, 3, b, 3);
        }

        public void PairConnectionAssertions(NetworkElement elem1, int phase1, NetworkElement elem2, int phase2)
        {
            Assert.IsTrue(elem1.ConnectionExists(phase1, elem2, phase2));
            Assert.IsTrue(elem2.ConnectionExists(phase2, elem1, phase1));
        }
    }
}
