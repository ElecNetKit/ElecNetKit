using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.Composition
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequiresSimulatorAttribute : Attribute
    {
        public String SimulatorName { private set; get; }

        public RequiresSimulatorAttribute(String SimulatorName)
        {
            this.SimulatorName = SimulatorName;
        }
    }
}
