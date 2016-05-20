using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ConfigEditor
{
    // 动态数据定义
    public class DynamicDefineData
    {
        public List<SkillConditionDefine> SkillConditions;

        public List<SkillFunctionDefine> SkillFunctions;
    }
}
