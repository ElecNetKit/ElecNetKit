using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.Composition
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterisedListAttribute : Attribute
    {
        public Type Enumeration { protected set; get; }

        public ParameterisedListAttribute(Type Enumeration)
        {
            if (!typeof(Enum).IsAssignableFrom(Enumeration))
                throw new ArgumentException("Enumeration must be an Enum type.");

            this.Enumeration = Enumeration;
        }

        public ParameterisedListAttribute() { }
    }
}
