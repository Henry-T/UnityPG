using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ConfigEditor
{
    class ID2LevelNameConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string str = value.ToString();

            if (String.IsNullOrEmpty(str))
            {
                return "绑定值空";
            }

            int id = int.Parse(str);

            Level_Type lvlType = ModelManager.Instance.LevelXlsData.GetByID(id);

            if (lvlType == null)
                return "未指定";
            else
                return lvlType.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
