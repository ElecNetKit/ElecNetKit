using ElecNetKit.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.Sensitivities
{
    /// <summary>
    /// Generates sensitivity information from the values obtained from
    /// applying a <see cref="PerturbAndObserveRunner{T}"/> to an electrical network
    /// model.
    /// </summary>
    /// <remarks>The <see cref="SensitivityGenerator{T}"/> class uses perturb-and-observe
    /// measurements to obtain the rate of change of a given parameter relative to another parameter.
    /// This is done by applying a <see cref="RecordedPerturbationSelector"/> to obtain a numerical value
    /// for the perturbation, and a <see cref="ResultSelector"/> to obtain a numerical value for 
    /// the parameter in question both before and after experimentation. The sensitivity is then determined
    /// by the formula
    /// <code>
    /// Sensitivity = (ValueAfterExperiment - ValueBeforeExperiment) / PerturbationAmount;</code>
    /// Both <see cref="RecordedPerturbationSelector"/> and <see cref="ResultSelector"/> should return
    /// values that are convertible to <see cref="double"/>.</remarks>
    /// <typeparam name="T">The generic type parameter of the <see cref="PerturbAndObserveRunner{T}"/>
    /// to determine sensitivities from.</typeparam>
    public class SensitivityGenerator<T>
    {
        /// <summary>
        /// Maps the results stored in <see cref="PerturbAndObserveRunner{T}.BeforeValues"/> and
        /// the values of <see cref="PerturbAndObserveRunner{T}.AfterValues"/> to a single object that is
        /// convertible to <see cref="double"/>.
        /// </summary>
        public Func<T, Object> ResultSelector {set; get;}

        /// <summary>
        /// Maps the perturbations stored in the keys of <see cref="PerturbAndObserveRunner{T}.AfterValues"/>
        /// to single objects that are convertible to <see cref="double"/>.
        /// </summary>
        public Func<Object, Object> RecordedPerturbationSelector { set; get; }

        /// <summary>
        /// Generate sensitivities from the results of <see cref="PerturbAndObserveRunner{T}.RunPerturbAndObserve"/>,
        /// and using the selection functions <see cref="ResultSelector"/> and <see cref="RecordedPerturbationSelector"/>.
        /// </summary>
        /// <param name="perturbAndObserveRunner">The <see cref="PerturbAndObserveRunner{T}"/> to determine sensitivities
        /// from.</param>
        /// <returns>A twin-key dictionary with each value representing the sensitivity of the specified value at the Y-element
        /// to changes in the perturbed value at the X-element.</returns>
        public TwinKeyDictionary<String, String, double> GenerateSensitivities(PerturbAndObserveRunner<T> perturbAndObserveRunner)
        {
            var results = new TwinKeyDictionary<String, String, double>();
            foreach (var perturbation in perturbAndObserveRunner.AfterValues)
            {
                foreach (var observation in perturbation.Value)
                {
                    T oldResult;
                    if (!perturbAndObserveRunner.BeforeValues.TryGetValue(observation.Key, out oldResult))
                        continue;
                    var sensitivity = (Convert.ToDouble(ResultSelector(observation.Value)) - Convert.ToDouble(ResultSelector(oldResult)))/Convert.ToDouble(RecordedPerturbationSelector(perturbation.Key.Item2));
                    results.Add(perturbation.Key.Item1.Item1, observation.Key.Item1, sensitivity);
                }
            }
            return results;
        }
    }
}
