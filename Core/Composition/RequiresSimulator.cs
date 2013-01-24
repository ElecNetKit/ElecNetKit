using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.Composition
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequiresSimulator : Attribute
    {
        public String SimulatorName { private set; get; }

        public RequiresSimulator(String SimulatorName)
        {
            this.SimulatorName = SimulatorName;
        }
    }
}
