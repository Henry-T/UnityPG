using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ConfigEditor
{
    // String-Enum 转换
    public class Enum2StrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                              System.Globalization.CultureInfo culture)
        {
            if (parameter is Type)
            {
                Type enumType = parameter as Type;
                string valueStr = value.ToString();

                if (!String.IsNullOrEmpty(valueStr))
                {
                    //if (Enum.IsDefined(enumType, valueStr))
                    {
                        return (Enum)Enum.Parse((Type)enumType, valueStr);
                    }
                    //else
                    //{
                    //    Debugger.Break();
                    //    LogManager.Instance.Warn("EnumConvert 失败: " + valueStr + " -> " + enumType.Name);
                    //    return Enum.GetValues(enumType).GetEnumerator().Current;
                    //}
                }
            }
            // Console.WriteLine(enumValue);
            return default(Enum);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            int returnValue = 0;
            if (parameter is Type)
            {
                returnValue = (int)Enum.Parse((Type)parameter, value.ToString());
            }
            // Console.WriteLine(returnValue);
            return returnValue;
        }
    }
}
