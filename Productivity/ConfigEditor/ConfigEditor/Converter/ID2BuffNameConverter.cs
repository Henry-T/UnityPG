using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Data;

namespace ConfigEditor
{
    class ID2BuffNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int buffId = int.Parse(value.ToString());
            return ConvertTypeToBuffName(buffId);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static string ConvertTypeToBuffName(int id)
        {
            DataView dataView = ModelManager.Instance.BuffXlsData.DataTable.AsDataView();
            dataView.Sort = "ID";
            int findIndex = dataView.Find(id);

            if (findIndex == -1)
                return "未指定";
            else
                return dataView[findIndex]["Name"].ToString();
        }
    }
}
