using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.Composition
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class AliasAttribute : Attribute
    {
        public String Alias { private set; get; }

        public AliasAttribute(String Alias)
        {
            this.Alias = Alias;
        }
    }
}
