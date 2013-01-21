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
            runner.PerturbCommands = new String[] { "new Generator.{0} bus1={1} phases=3 model=1 status=fixed kV={2} Vminpu=0.9 Vmaxpu=1.1 kW={3}" };
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

            Assert.Fail();
        }
    }
}
