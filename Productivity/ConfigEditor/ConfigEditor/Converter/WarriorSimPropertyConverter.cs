using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ConfigEditor
{
    public class WarriorSimPropertyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (AppUtil.IsInDesignMode)
                return "0";

            try
            {
                var intValues = values.Cast<String>().ToArray();

                int modValue = 0;
                int baseValue = 0;
                int level = 1;
                int grade = 1;
                int lvGrowth = 0;
                int gradeGrowth = 0;

                int.TryParse(intValues[0], out modValue);
                int.TryParse(intValues[1], out baseValue);
                int.TryParse(intValues[2], out level);
                int.TryParse(intValues[3], out grade);

                if (level <= 0)
                {
                    return "lv<1";
                }
                else
                {
                    int levelRowIndex = ModelManager.Instance.ExpSetXlsData.DataTable.DefaultView.Find(level);
                    if(levelRowIndex == -1)
                    {
                        LogManager.Instance.Warn("武将成长计算 ExpSet中未配置此等级: " + level);
                        return "no_lv";
                    }
                    DataRowView levelRowView = ModelManager.Instance.ExpSetXlsData.DataTable.DefaultView[levelRowIndex];
                    lvGrowth = (levelRowView == null) ? 0 : int.Parse((string)levelRowView["LvGrowth"]);
                }

                if (grade < 0)
                {
                    return "grade<0";
                }
                else if (grade == 0)
                {
                    gradeGrowth = 0;
                }
                else
                {
                    int gradeRowIndex = ModelManager.Instance.ExpSetXlsData.DataTable.DefaultView.Find(grade);
                    if (gradeRowIndex == -1)
                    {
                        LogManager.Instance.Warn("武将成长计算 ExpSet中未配置此等级: " + grade);
                        return "no_grade";
                    }
                    DataRowView gradeRowView = ModelManager.Instance.ExpSetXlsData.DataTable.DefaultView[gradeRowIndex];
                    gradeGrowth = (gradeRowView == null) ? 0 : int.Parse((string)gradeRowView["GradeGrowth"]);
                }

                var result = Math.Round(modValue + baseValue * (1 + lvGrowth / 100f + gradeGrowth / 100f));

                return result.ToString();
            }
            catch (Exception e)
            {
                LogManager.Instance.Error(e.Message);
                LogManager.Instance.Error(e.Source);
                LogManager.Instance.Error(e.StackTrace);
                return "100";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
