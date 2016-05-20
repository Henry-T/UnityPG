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
    public class Str2EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                              System.Globalization.CultureInfo culture)
        {
            string returnValue = "";
            if (parameter is Type)
            {
                returnValue = Enum.Parse((Type)parameter, value.ToString()).ToString();
            }
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            if (parameter is Type)
            {
                Type enumType = parameter as Type;
                string valueStr = value.ToString();

                if (!String.IsNullOrEmpty(valueStr))
                {
                    Object v = Enum.Parse((Type)enumType, valueStr);
                    return v;
                }
            }
            return default(Enum);
        }
    }
}
