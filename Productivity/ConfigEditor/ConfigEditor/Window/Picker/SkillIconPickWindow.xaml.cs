using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace ConfigEditor
{
    /// <summary>
    /// Interaction logic for SkillIconPickWindow.xaml
    /// </summary>
    public partial class SkillIconPickWindow : Window
    {
        public string PickedFile;
        public string PickedCode;

        public SkillIconPickWindow()
        {
            InitializeComponent();

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            String spineDir = Path.Combine(UserConfigManager.Instance.Config.ResDir, "skills");
            DirectoryInfo spineDirInfo = new DirectoryInfo(spineDir);
            List<SkillIconInfo> iconInfos = spineDirInfo.GetFiles("*.jpg")
                .Where(f => f.Name[f.Name.Length - 5] != 'f')
                .Select<FileInfo, SkillIconInfo>(fi => new SkillIconInfo(fi))
                .ToList();

            lsbIcons.ItemsSource = iconInfos;
        }
        
        private void onBtn_Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void onBtn_Confirm(object sender, RoutedEventArgs e)
        {
            if (lsbIcons.SelectedItem == null)
                return;

            SkillIconInfo pickedItem = lsbIcons.SelectedItem as SkillIconInfo;
            PickedFile = pickedItem.File.FullName;
            PickedCode = pickedItem.Code;
            DialogResult = true;
            this.Close();
        }
    }
    public class SkillIconInfo
    {
        public FileInfo File { get; set; }
        public String Code { get; set; }

        public SkillIconInfo(FileInfo fInfo)
        {
            File = fInfo;
            Code = fInfo.Name.Replace(".jpg", "");
        }
    }
}
