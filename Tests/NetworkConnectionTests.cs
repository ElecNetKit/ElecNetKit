using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ElecNetKit.NetworkModelling;
using System.Numerics;

namespace Tests
{
    [TestClass]
    public class NetworkConnectionTests
    {
        [TestMethod]
        public void TestSameTypeEquality()
        {
            Load l = new Load("ID", Complex.Zero);
            var conn1 = new NetworkElementConnection(l, 1);
            var conn2 = new NetworkElementConnection(l, 1);
            Assert.IsTrue(conn1.Equals(conn2) && conn2.Equals(conn1));
        }

        [TestMethod]
        public void TestOtherTypeEquality()
        {
            Load l = new Load("ID", Complex.Zero);
            var conn1 = new NetworkElementConnection(l, 1);
            var conn2 = new NetworkElementConnection(l, 1);
            Assert.IsTrue(NetworkElementConnection.Equals(conn1, conn2));
            Assert.IsTrue(conn1.Equals((Object)conn2) && conn2.Equals((Object)conn1));
        }

        [TestMethod]
        public void TestSameTypeInequality()
        {
            Load l = new Load("ID", Complex.Zero);
            var conn1 = new NetworkElementConnection(l, 1);
            var conn2 = new NetworkElementConnection(l, 2);
            var conn3 = new NetworkElementConnection(new Load("ID",0), 1);
            Assert.IsFalse(conn1.Equals(conn2));
            Assert.IsFalse(conn2.Equals(conn1));
            Assert.IsFalse(conn1.Equals(conn3));
        }

        [TestMethod]
        public void TestEqualityWithNull()
        {
            Load l = new Load("ID", Complex.Zero);
            var conn1 = new NetworkElementConnection(l, 1);
            Assert.IsFalse(conn1.Equals(null));
            Assert.IsFalse(NetworkElementConnection.Equals(conn1, null));
        }

        public void TestOperatorOverloadEquality()
        {
            Load l = new Load("ID", Complex.Zero);
            var conn1 = new NetworkElementConnection(l, 1);
            var conn2 = new NetworkElementConnection(l, 1);
            Assert.IsTrue(conn1 == conn2);
            Assert.IsFalse(conn1 != conn2);
        }
    }
}
