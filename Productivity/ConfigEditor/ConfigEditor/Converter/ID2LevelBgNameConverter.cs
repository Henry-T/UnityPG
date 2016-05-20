using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ConfigEditor
{
    class ID2LevelBgNameConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string str = value.ToString();

            if (String.IsNullOrEmpty(str))
            {
                return "绑定值空";
            }

            int avatarTypeID = int.Parse(str);
            LevelBgType find = GetLevelBgEntry(avatarTypeID);

            if (find == null)
                return "未指定";
            else
                return find.Name;
        }

        public static LevelBgType GetLevelBgEntry(int typeId)
        {
            var findResult = ModelManager.Instance.LevelBgXlsData.DataList.Where(l => l.ID == typeId);
            if (findResult.Count() == 0)
            {
                return null;
            }
            else
            {
                return findResult.First();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
