using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ConfigEditor
{
    class Type2AvatarConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string str = value.ToString();

            if (String.IsNullOrEmpty(str))
            {
                return new Object();
            }

            var find = ModelManager.Instance.AvatarXlsData.GetRowViewByKey(str);
            return find;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Data.DataRowView row = (System.Data.DataRowView)value;
            return row[ModelManager.Instance.AvatarXlsData.KeyName];
        }
    }
}
