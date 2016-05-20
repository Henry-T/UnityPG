using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor
{
    public class DefaultValueAttribute : Attribute
    {
        public string Value;

        public DefaultValueAttribute(string value)
        {
            Value = value;
        }
    }
}
