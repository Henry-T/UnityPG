using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows;

namespace ConfigEditor
{
    public class LevelWave2NPCListConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (AppUtil.IsInDesignMode)
                return "0";

            if (values[0] == DependencyProperty.UnsetValue)
                return null;

            int levelID = (int)values[0];
            int wave = (int)values[1];

            ObservableCollection<LevelNPCType> result = new ObservableCollection<LevelNPCType>();
            var finds = ModelManager.Instance.LevelNpcXlsData.DataList.Where(n => n.Wave == wave && n.Level == levelID);

            foreach (LevelNPCType npc in finds)
            {
                result.Add(npc);
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
