using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ElecNetKit.NetworkModelling;
using System.Numerics;
using System.Linq;
using ElecNetKit.NetworkModelling.Phasing;

namespace Tests
{
    [TestClass]
    public class EvaluatedPhasedTests
    {
        PhasedValues<Complex> values;
        PhasedEvaluated<Complex, Complex> eval;
        public EvaluatedPhasedTests()
        {
            values = new PhasedValues<Complex>();
            values.Add(1, new Complex(10, 5));
            eval = new PhasedEvaluated<Complex, Complex>(c => c / 5, c => c * 5, values);
        }

        [TestMethod]
        public void TestSetup()
        {
            Assert.AreEqual(1, values.Count);
            Assert.AreEqual(1, values.Keys.First());
            Assert.AreEqual(new Complex(2,1), eval.Values.First());
        }

         [TestMethod]
        public void TestAdd()
        {
            eval.Add(2, new Complex(3, 10));
            Assert.AreEqual(values[2], new Complex(15, 50));
        }
    }
}
