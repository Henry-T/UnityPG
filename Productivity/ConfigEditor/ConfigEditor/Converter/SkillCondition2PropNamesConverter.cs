using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ConfigEditor
{
    // SkillCondition转属性名列表
    public class SkillCondition2PropNamesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Type t = value.GetType();
            List<String> propNames = new List<string>();
            foreach(PropertyInfo pInfo in t.GetProperties())
            {
                propNames.Add(pInfo.Name);
            }
            return propNames;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
