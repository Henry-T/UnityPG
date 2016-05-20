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
    public class GradeConditions2PropSumConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (AppUtil.IsInDesignMode)
                return "0";

            try
            {
                NatureRequirement natureRequirement = values[0] as NatureRequirement;

                if (natureRequirement == null)
                {
                    return "nature_nil";
                }

                int grade = 1;
                int.TryParse(values[1].ToString(), out grade);

                if (grade < 0)
                {
                    return "grade<0";
                }
                else if (grade == 0)
                {
                    return "0";
                }

                EAddition addType = (EAddition)values[2];

                int sum = 0;

                for (int i = 1; i <= grade; i++ )
                {
                    int index = i - 1;
                    if (index >= natureRequirement.Count)
                        break;

                    NatureList natureList = natureRequirement[index];
                    foreach(NatureEnumWrap wrap in natureList)
                    {
                        NatureEntry entry = ModelManager.Instance.NatureXlsData[wrap.Value];                        
                        Addition add = entry.Additions[addType];
                        sum += add.Value;
                    }
                }

                return sum.ToString();
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
