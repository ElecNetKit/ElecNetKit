using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.Composition
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterisedValueAttribute : ParameterisedAttribute
    {
        public double? MinValue { set; get; }
        public double? MaxValue { set; get; }
        public double? Interval { set; get; }
        public double DefaultValue { set; get; }

        public ParameterisedValueAttribute(double DefaultValue)
        {
            this.DefaultValue = DefaultValue;
        }
    }
}
