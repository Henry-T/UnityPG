using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ConfigEditor
{
    public class Json2GradeConditionConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || String.IsNullOrEmpty(value.ToString()))
                return new NatureRequirement();

            string gradeConditionStr = value.ToString();

            NatureRequirement gradeData = NatureRequirement.CreateFromJson(gradeConditionStr);

            return gradeData;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
