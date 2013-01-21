using ElecNetKit.Experimentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.Sensitivities
{
    class StringFormatExperimentor : IExperimentor
    {
        public IEnumerable<String> ExperimentCommands { set; get; }

        public Object[] ExperimentValues { set; get; }

        public List<string> Experiment(NetworkModelling.NetworkModel Network)
        {
            List<string> commandsFormatted = new List<string>();
            foreach (String str in ExperimentCommands)
            {
                commandsFormatted.Add(String.Format(str, ExperimentValues));
            }
            return commandsFormatted;
        }
    }
}
