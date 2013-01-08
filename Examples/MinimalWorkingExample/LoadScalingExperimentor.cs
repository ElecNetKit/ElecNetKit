using System;
using System.Collections.Generic;
using System.Linq;
using ElecNetKit.Experimentation;

namespace MinimalWorkingExample
{
    class LoadScalingExperimentor : IExperimentor
    {
        public double LoadScalingFactor { set; get; }

        //Convenience constructor, to set scaling factor when object is initialised.
        public LoadScalingExperimentor(double factor)
        {
            LoadScalingFactor = factor;
        }

        //Implements IExperimentor.Experiment()
        public List<string> Experiment(ElecNetKit.NetworkModelling.NetworkModel Network)
        {
            return Network.Loads.Select(
                load => String.Format("edit load.{0} kW={1:F6} kVAr={2:F6}",
                                      load.ID,
                                      load.ActualKVA.Real * LoadScalingFactor,
                                      load.ActualKVA.Imaginary * LoadScalingFactor
                                      )
                    ).ToList();
        }
    }
}
