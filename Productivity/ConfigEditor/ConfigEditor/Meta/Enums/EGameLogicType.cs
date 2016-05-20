using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor
{
    // 逻辑数据类型, 每种逻辑类型必然同时是一种基础类型，而他们的关系是已知且确定的
    // 例如: Int和Percent内部类型都是int；BuffType逻辑上表示Buff的Type值，内部也是int
    public enum ELogicType
    {
        Ivalid,         // 非法值
        Int,            // 整数
        Percent,        // 百分比
        String,         // 字符串
        BuffType,       // Buff的Type值
        Element,        // 元素
    }
}
