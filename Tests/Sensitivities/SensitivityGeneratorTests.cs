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
            runner.NetworkFilename = AppDomain.CurrentDomain.BaseDirectory + @"\TestNetworks\Ex9_5_nogen.dss";
            runner.PerturbCommands = new String[] { "new Generator.{0} bus1={1} phases=3 model=1 status=fixed kV={2} Vminpu=0.9 Vmaxpu=1.1 kW={3} kvAR=0" };
            runner.PerturbElementSelector = network => network.Buses.Values;
            runner.PerturbElementValuesSelector = elem => { Bus b = (Bus)elem; return new Object[] { "gg-" + b.ID, b.ID, b.BaseVoltage * Math.Sqrt(3) / 1000, 3260 }; };
            runner.PerturbValuesToRecord = x => x[3];
            runner.ObserveElementSelector = network => network.Buses.Values;
            runner.ObserveElementValuesSelector = elem => ((Bus)elem).Voltage;
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

        [TestMethod]
        [DeploymentItem("TestNetworks", "TestNetworks")]
        public void TestVoltageSensitivityToComplexPowerGenerator()
        {
            var sensitivities = VoltageSensitivityToComplexPowerGenerator.GetVoltageSensitivityToComplexPower(new OpenDSSSimulator(),
                AppDomain.CurrentDomain.BaseDirectory + @"\TestNetworks\Ex9_5_nogen.dss",
                "new Generator.{0} bus1={1} phases=3 model=1 status=fixed kV=230 Vminpu=0.8 Vmaxpu=1.2 kW={2} kVAr={3}",
                0.02);

            AssertEqualByPercent(0.018510852, sensitivities.MapX["b2"]["b2"].dV_dP, 0.1);
            AssertEqualByPercent(0.007367939, sensitivities.MapX["b2"]["b3"].dV_dP, 0.1);
            AssertEqualByPercent(0.015043472, sensitivities.MapX["b2"]["b4"].dV_dP, 0.1);
            AssertEqualByPercent(0.006543352, sensitivities.MapX["b3"]["b2"].dV_dP, 0.1);
            AssertEqualByPercent(0.013438371, sensitivities.MapX["b3"]["b3"].dV_dP, 0.1);
            AssertEqualByPercent(0.009204037, sensitivities.MapX["b3"]["b4"].dV_dP, 0.1);
            AssertEqualByPercent(0.018171524, sensitivities.MapX["b4"]["b2"].dV_dP, 0.1);
            AssertEqualByPercent(0.012616745, sensitivities.MapX["b4"]["b3"].dV_dP, 0.1);
            AssertEqualByPercent(0.025445765, sensitivities.MapX["b4"]["b4"].dV_dP, 0.1);

            AssertEqualByPercent(0.060197407, sensitivities.MapX["b2"]["b2"].dV_dQ, 0.1);
            AssertEqualByPercent(0.017790522, sensitivities.MapX["b2"]["b3"].dV_dQ, 0.1);
            AssertEqualByPercent(0.04584525, sensitivities.MapX["b2"]["b4"].dV_dQ, 0.1);
            AssertEqualByPercent(0.017635184, sensitivities.MapX["b3"]["b2"].dV_dQ, 0.1);
            AssertEqualByPercent(0.047029425, sensitivities.MapX["b3"]["b3"].dV_dQ, 0.1);
            AssertEqualByPercent(0.029357285, sensitivities.MapX["b3"]["b4"].dV_dQ, 0.1);
            AssertEqualByPercent(0.045998745, sensitivities.MapX["b4"]["b2"].dV_dQ, 0.1);
            AssertEqualByPercent(0.029708389, sensitivities.MapX["b4"]["b3"].dV_dQ, 0.1);
            AssertEqualByPercent(0.077140931, sensitivities.MapX["b4"]["b4"].dV_dQ, 0.1);
            /**/
            AssertEqualByPercent(4.38209E-07, sensitivities.MapX["b2"]["b2"].dd_dP, 0.1);
            AssertEqualByPercent(1.1185E-07, sensitivities.MapX["b2"]["b3"].dd_dP, 0.1);
            AssertEqualByPercent(3.26266E-07, sensitivities.MapX["b2"]["b4"].dd_dP, 0.1);
            AssertEqualByPercent(1.13038E-07, sensitivities.MapX["b3"]["b2"].dd_dP, 0.1);
            AssertEqualByPercent(3.43355E-07, sensitivities.MapX["b3"]["b3"].dd_dP, 0.1);
            AssertEqualByPercent(2.05534E-07, sensitivities.MapX["b3"]["b4"].dd_dP, 0.1);
            AssertEqualByPercent(3.21392E-07, sensitivities.MapX["b4"]["b2"].dd_dP, 0.1);
            AssertEqualByPercent(2.00372E-07, sensitivities.MapX["b4"]["b3"].dd_dP, 0.1);
            AssertEqualByPercent(5.84424E-07, sensitivities.MapX["b4"]["b4"].dd_dP, 0.1);

            AssertEqualByPercent(-3.45082E-08, sensitivities.MapX["b2"]["b2"].dd_dQ, 0.1);
            AssertEqualByPercent(2.31984E-09, sensitivities.MapX["b2"]["b3"].dd_dQ, 0.1);
            AssertEqualByPercent(8.64631E-09, sensitivities.MapX["b2"]["b4"].dd_dQ, 0.1);
            AssertEqualByPercent(8.20066E-09, sensitivities.MapX["b3"]["b2"].dd_dQ, 0.1);
            AssertEqualByPercent(-3.6108E-08, sensitivities.MapX["b3"]["b3"].dd_dQ, 0.1);
            AssertEqualByPercent(1.30424E-08, sensitivities.MapX["b3"]["b4"].dd_dQ, 0.1);
            AssertEqualByPercent(-1.55317E-08, sensitivities.MapX["b4"]["b2"].dd_dQ, 0.1);
            AssertEqualByPercent(-1.25475E-08, sensitivities.MapX["b4"]["b3"].dd_dQ, 0.1);
            AssertEqualByPercent(-3.33473E-08, sensitivities.MapX["b4"]["b4"].dd_dQ, 0.1);
        }


        void AssertEqualByPercent(double expected, double actual, double percentDifferenceAllowable)
        {
            Assert.AreEqual(expected, actual, Math.Abs(expected * percentDifferenceAllowable));
        }
    }
}
