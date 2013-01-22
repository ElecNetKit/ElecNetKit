using ElecNetKit.NetworkModelling;
using ElecNetKit.Simulator;
using ElecNetKit.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.Sensitivities
{
    /// <summary>
    /// Generates Voltage Sensitivities to changes in P and Q.
    /// </summary>
    public class VoltageSensitivityToComplexPowerGenerator
    {
        /// <summary>
        /// Generates Voltage Sensitivities to changes in P and Q at each load bus on
        /// the network.
        /// </summary>
        /// <remarks>
        /// This is a convenience class that uses the functionality supplied by
        /// <see cref="SensitivityGenerator{T}"/> and <see cref="PerturbAndObserveRunner{T}"/>.
        /// For other kinds of sensitivities, you may wish to use these classes directly.</remarks>
        /// <param name="Simulator">The simulator to use for the generation of sensitivities.</param>
        /// <param name="NetworkMasterFile">The file path of the Network to calculate sensitivities for.</param>
        /// <param name="CommandString">A command for issuing perturbations. The command should be
        /// compatible with <see cref="String.Format(String,Object[])"/>-style format strings, and should
        /// use <c>{0}</c> to represent a random ID, <c>{1}</c> to represent the bus that perturbation should occur
        /// on, <c>{2}</c> to represent a kW quantity to perturb by and <c>{3}</c> to represent a kVAr quantity to
        /// perturb by.
        /// <example>
        /// As an example, the following string specifies a new generator for perturbation in OpenDSS syntax:
        /// <code>
        /// "new Generator.{0} bus1={1} phases=3 model=1 status=fixed kV=11 Vminpu=0.9 Vmaxpu=1.1 kW={2} kvAR={3}"</code></example></param>
        /// <param name="PerturbationFrac">The fraction of average load size to perturb by.</param>
        /// <returns>A 2-axis dictionary, in which the X-axis represents the source bus, the Y-axis represents the affected bus,
        /// and the values are an index of sensitivity information.</returns>
        public static TwinKeyDictionary<String, String, VoltageSensitivityToPQDataSet> GetVoltageSensitivityToComplexPower(ISimulator Simulator, String NetworkMasterFile, String CommandString, double PerturbationFrac)
        {
            PerturbAndObserveRunner<Complex> perturbAndObserve = new PerturbAndObserveRunner<Complex>(Simulator);
            NetworkController controller = new NetworkController(Simulator);
            controller.NetworkFilename = NetworkMasterFile;
            controller.Execute();
            var avgLoad = controller.Network.Loads.Select(load=>load.ActualKVA).Aggregate((seed,elem) => seed+elem);
            avgLoad /= controller.Network.Loads.Count;
            perturbAndObserve.NetworkMasterFile = NetworkMasterFile;
            perturbAndObserve.ObserveElementQuery = network => network.Buses.Values.Where(bus => bus.ConnectedTo.OfType<Load>().Any());
            perturbAndObserve.PerturbElementQuery = perturbAndObserve.ObserveElementQuery;
            perturbAndObserve.PerturbCommands = new []{CommandString};
            perturbAndObserve.ObserveElementValuesQuery = elem => ((Bus)elem).Voltage;

            SensitivityGenerator<Complex> generator = new SensitivityGenerator<Complex>();

            //real
            perturbAndObserve.PerturbElementValuesQuery = bus => new Object[] {"inject-"+bus.ID,bus.ID,PerturbationFrac * avgLoad.Real, 0};
            perturbAndObserve.PerturbValuesToRecord = vars => vars[2];
            perturbAndObserve.RunPerturbAndObserve();

            generator.RecordedPerturbationSelector = x => x;
            generator.ResultSelector = x => x.Magnitude;

            var MagnitudeDictionaryReal = generator.GenerateSensitivities(perturbAndObserve);

            generator.ResultSelector = x => x.Phase;
            var PhaseDictionaryReal = generator.GenerateSensitivities(perturbAndObserve);

            //imaginary
            perturbAndObserve.PerturbElementValuesQuery = bus => new Object[] { "inject-" + bus.ID, bus.ID, 0, PerturbationFrac * avgLoad.Imaginary };
            perturbAndObserve.PerturbValuesToRecord = vars => vars[3];
            perturbAndObserve.RunPerturbAndObserve();

            generator.RecordedPerturbationSelector = x => x;
            generator.ResultSelector = x => x.Magnitude;

            var MagnitudeDictionaryImag = generator.GenerateSensitivities(perturbAndObserve);

            generator.ResultSelector = x => x.Phase;
            var PhaseDictionaryImag = generator.GenerateSensitivities(perturbAndObserve);

            // now merge all the dictionaries.
            return TwinKeyDictionaryMerge(MagnitudeDictionaryReal, MagnitudeDictionaryImag, PhaseDictionaryReal, PhaseDictionaryImag);
        }

        // Merges the dictionaries obtained by GetVoltageSensitivityToComplexPower.
        private static TwinKeyDictionary<String, String, VoltageSensitivityToPQDataSet> TwinKeyDictionaryMerge(TwinKeyDictionary<String,String,double> MagReal, TwinKeyDictionary<String,String,double> MagImag, 
TwinKeyDictionary<String,String,double> PhaseReal, 
TwinKeyDictionary<String,String,double> PhaseImag
)
        {
            var newDictionary = new TwinKeyDictionary<String, String, VoltageSensitivityToPQDataSet>();
            foreach(var X in MagReal.MapX)
                foreach (var Y in X.Value)
                {
                    var val = new VoltageSensitivityToPQDataSet();
                    val.dV_dP = Y.Value;
                    val.dV_dQ = MagImag.MapX[X.Key][Y.Key];
                    val.dd_dP = PhaseReal.MapX[X.Key][Y.Key];
                    val.dd_dQ = PhaseImag.MapX[X.Key][Y.Key];
                    newDictionary.Add(X.Key, Y.Key, val);
                }
            return newDictionary;
        }
    }
}
