using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ConfigEditor
{
    public class Json2SkillConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string str = value.ToString();

            if (String.IsNullOrEmpty(str))
            {
                return new Object();
            }

            str = str.Replace("[", "");
            str = str.Replace("]", "");

            int skillTypeId = int.Parse(str);

            // NOTE thy DataRow 无法通过Key索引值，必须转换成DataRowView

            /*
            DataView dataView = ModelManager.Instance.SkillXlsData.DataTable.AsDataView();
            dataView.RowFilter = "ID = " + skillTypeId;
            if (dataView.Count == 0)
            {
                return new Object();
            }
            else
            {
                return dataView[0];
            }
             */

            IEnumerable<SkillType> skillTypes = ModelManager.Instance.SkillXlsData.DataList.Where(s => s.ID == skillTypeId);
            if (skillTypes.Count() == 0)
                return new Object();
            else
                return skillTypes.First();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Data.DataRowView row = (System.Data.DataRowView)value;
            return "[" + row["ID"] + "]";
        }
    }
}
