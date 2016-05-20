using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor
{
    public class MinValueAttribute : Attribute
    {
        public int Value;

        public MinValueAttribute(int value)
        {
            Value = value;
        }
    }

}
