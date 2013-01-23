using ElecNetKit.Experimentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.Sensitivities
{
    /// <summary>
    /// Runs experiment commands obtained by applying <see cref="String.Format(String,Object[])"/>
    /// with values specified in <see cref="ExperimentValues"/> to the commands
    /// specified in <see cref="ExperimentCommands"/>.
    /// </summary>
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
