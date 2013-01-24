using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.Composition
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=true)]
    public class ListOptionAttribute : ParameterisedAttribute
    {
        public String OptionName { private set; get; }
        public Object OptionValue { private set; get; }

        public ListOptionAttribute(String OptionName, Object OptionValue)
        {
            this.OptionName = OptionName;
            this.OptionValue = OptionValue;
        }
    }
}
