using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ElecNetKit.Sensitivities;
using ElecNetKit.Engines;
using System.Collections.Generic;
using System.Linq;
using ElecNetKit.NetworkModelling;
using System.Numerics;

namespace Tests.Sensitivities
{
    [TestClass]
    public class SensitivityGeneratorTests
    {
        [TestMethod]
        [DeploymentItem("TestNetworks", "TestNetworks")]
        public void TestSensitivityGenerator()
        {
            PerturbAndObserveRunner<Complex> runner = new PerturbAndObserveRunner<Complex>(new OpenDSSSimulator());
            runner.NetworkMasterFile = AppDomain.CurrentDomain.BaseDirectory + @"\TestNetworks\Ex9_5_nogen.dss";
            runner.PerturbCommands = new String[] { "new Generator.{0} bus1={1} phases=3 model=1 status=fixed kV={2} Vminpu=0.9 Vmaxpu=1.1 kW={3} kvAR=0" };
            runner.PerturbElementQuery = network => network.Buses.Values;
            runner.PerturbElementValuesQuery = elem => { Bus b = (Bus)elem; return new Object[] { "gg-" + b.ID, b.ID, b.BaseVoltage * Math.Sqrt(3) / 1000, 3260 }; };
            runner.PerturbValuesToRecord = x => x[3];
            runner.ObserveElementQuery = network => network.Buses.Values;
            runner.ObserveElementValuesQuery = elem => ((Bus)elem).Voltage;
            runner.RunPerturbAndObserve();

            SensitivityGenerator<Complex> generator = new SensitivityGenerator<Complex>();
            generator.RecordedPerturbationSelector = x => x;
            generator.ResultSelector = x => x.Magnitude;
            var sensitivities = generator.GenerateSensitivities(runner);

            AssertEqualByPercent(0.018510852, sensitivities.MapX["b2"]["b2"], 0.1);
            AssertEqualByPercent(0.007367939, sensitivities.MapX["b2"]["b3"], 0.1);
            AssertEqualByPercent(0.015043472, sensitivities.MapX["b2"]["b4"], 0.1);
            AssertEqualByPercent(0.006543352, sensitivities.MapX["b3"]["b2"], 0.1);
            AssertEqualByPercent(0.013438371, sensitivities.MapX["b3"]["b3"], 0.1);
            AssertEqualByPercent(0.009204037, sensitivities.MapX["b3"]["b4"], 0.1);
            AssertEqualByPercent(0.018171524, sensitivities.MapX["b4"]["b2"], 0.1);
            AssertEqualByPercent(0.012616745, sensitivities.MapX["b4"]["b3"], 0.1);
            AssertEqualByPercent(0.025445765, sensitivities.MapX["b4"]["b4"], 0.1);
        }

        void AssertEqualByPercent(double expected, double actual, double percentDifferenceAllowable)
        {
            Assert.AreEqual(expected, actual, expected * percentDifferenceAllowable);
        }
    }
}
