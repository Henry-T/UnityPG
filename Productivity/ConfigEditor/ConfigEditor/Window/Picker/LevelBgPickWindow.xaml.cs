using System;
using System.Collections.Generic;
using System.Data;
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

namespace ConfigEditor
{
    /// <summary>
    /// Interaction logic for SkillPickWindow.xaml
    /// </summary>
    public partial class LevelBgPickWindow : Window
    {
        public String PickedLevelBgName;
        public int PickedLevelBgType;

        public LevelBgPickWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lbLevelBgs.ItemsSource = ModelManager.Instance.LevelBgXlsData.DataList;
        }
        
        private void onBtn_Confirm(object sender, RoutedEventArgs e)
        {
            if (lbLevelBgs.SelectedItem != null)
            {
                PickedLevelBgName = (lbLevelBgs.SelectedItem as LevelBgType).Name.ToString();
                PickedLevelBgType = int.Parse((lbLevelBgs.SelectedItem as LevelBgType).ID.ToString());
                DialogResult = true;
                this.Close();
            }
        }

        private void onBtn_Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
