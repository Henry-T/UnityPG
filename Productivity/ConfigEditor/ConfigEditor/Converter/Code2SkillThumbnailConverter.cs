using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ConfigEditor
{
    public class Code2SkillThumbnailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (AppUtil.IsInDesignMode)
                return "";

            string imgPath = Path.Combine(UserConfigManager.Instance.Config.ResDir, "skills/"+value+"_buff.jpg");
            if (File.Exists(imgPath))
                return imgPath;
            else
                return "/Resource/skillDefaultThumb.jpg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
