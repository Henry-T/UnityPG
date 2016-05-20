using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor
{
    public class AssemblyUtil
    {
        public static String GetPropertyLogicTypeAsString(PropertyInfo propInfo)
        {
            return GetPropertyLogicType(propInfo).ToString();
        }

        public static ELogicType GetPropertyLogicType(PropertyInfo propInfo)
        {
            // NOTE thy 动态类始终无法获取到属性的CustomAttribute，好在CustomAttributeData也勉强能用
            var findAttrDatas = propInfo.CustomAttributes.Where(ad => ad.AttributeType == typeof(LogicTypeAttribute));
            if (findAttrDatas.Count() == 0)
            {
                return ELogicType.Ivalid;
            }
            return (ELogicType)findAttrDatas.First().ConstructorArguments[0].Value;
        }
    }
}
