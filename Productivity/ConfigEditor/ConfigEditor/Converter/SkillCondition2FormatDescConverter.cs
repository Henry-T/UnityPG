using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ConfigEditor
{
    // 将SkillCondition转化成其格式化描述
    class SkillCondition2FormatDescConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Type type = value.GetType();
            var das = type.GetCustomAttributes<DescriptionAttribute>();

            if (das.Count() == 0)
                return type.Name;

            var da = das.First();



            List<String> formatValues = new List<String>();

            foreach (PropertyInfo pInfo in type.GetProperties())
            {
                ELogicType gameType = AssemblyUtil.GetPropertyLogicType(pInfo);
                if (gameType == ELogicType.Ivalid)
                {
                    LogManager.Instance.Warn("找不到属性对应的逻辑类型: " + pInfo.Name);
                    continue;
                }

                if (gameType == ELogicType.Int || gameType == ELogicType.String)
                {
                    formatValues.Add(pInfo.GetValue(value).ToString());
                }
                else if(gameType == ELogicType.Percent)
                {
                    formatValues.Add(pInfo.GetValue(value).ToString() + "%");
                }
                else if(gameType == ELogicType.BuffType)
                {
                    formatValues.Add("Buff:"+ID2BuffNameConverter.ConvertTypeToBuffName((int)pInfo.GetValue(value)));
                }
                else if(gameType == ELogicType.Element)
                {
                    formatValues.Add(((EElement)pInfo.GetValue(value)).ToString());
                }
                else
                {
                    LogManager.Instance.Warn("SkillCondition2FormatDescConverter未定义属性格式化的方法：" + gameType + " 默认格式化为字符串");
                    formatValues.Add(pInfo.GetValue(value).ToString());
                }
            }

            return string.Format(da.Description, formatValues.ToArray());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
