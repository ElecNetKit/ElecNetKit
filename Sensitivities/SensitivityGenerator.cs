using ElecNetKit.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.Sensitivities
{
    public class SensitivityGenerator<T>
    {
        public Func<T, Object> ResultSelector {set; get;}
        public Func<Object, Object> RecordedPerturbationSelector { set; get; }

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
