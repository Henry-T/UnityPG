using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;

namespace ConfigEditor
{
    class LogicTypeAttribute : Attribute
    {
        public ELogicType LogicType;

        public LogicTypeAttribute(ELogicType logicType)
        {
            LogicType = logicType;
        }
    }
}
