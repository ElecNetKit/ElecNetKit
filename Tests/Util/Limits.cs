using ElecNetKit.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class LimitsTests
    {
        [TestMethod]
        public void LimitsTestResetBatch()
        {
            var lim = new Limits();
            var ds1 = new double[] { 1, 5, 3, 9 };
            var ds2 = new double[] { 15, -40, 25, 6 };
            lim.ProcessData(ds2);
            lim.AutoDataReset();
            lim.ProcessData(ds1);
            Assert.AreEqual(4, lim.Count);
            Assert.AreEqual(1, lim.AutoMin);
            Assert.AreEqual(9, lim.AutoMax);
        }

        [TestMethod]
        public void LimitsTestMinBatch()
        {
            var lim = new Limits();
            var ds1 = new double[] { 1, 5, 3, 9 };
            lim.ProcessData(ds1);
            Assert.AreEqual(1, lim.AutoMin);
        }

        [TestMethod]
        public void LimitsTestMaxBatch()
        {
            var lim = new Limits();
            var ds1 = new double[] { 1, 5, 3, 9 };
            lim.ProcessData(ds1);
            Assert.AreEqual(9, lim.AutoMax);
        }

        [TestMethod]
        public void LimitsTestCountBatch()
        {
            var lim = new Limits();
            var ds1 = new double[] { 1, 5, 3, 9 };
            lim.ProcessData(ds1);
            Assert.AreEqual(4, lim.Count);
        }

        [TestMethod]
        public void LimitsTestCount()
        {
            var lim = new Limits();
            var ds1 = new double[] { 1, 5, 3, 9 };
            foreach (double d in ds1)
                lim.ProcessData(d);
            Assert.AreEqual(4, lim.Count);
        }

        [TestMethod]
        public void LimitsTestMin()
        {
            var lim = new Limits();
            var ds1 = new double[] { 1, 5, 3, 9 };
            foreach (double d in ds1)
                lim.ProcessData(d);
            Assert.AreEqual(1, lim.AutoMin);
        }

        [TestMethod]
        public void LimitsTestMax()
        {
            var lim = new Limits();
            var ds1 = new double[] { 1, 5, 3, 9 };
            foreach (double d in ds1)
                lim.ProcessData(d);
            Assert.AreEqual(9, lim.AutoMax);
        }


    }
}
