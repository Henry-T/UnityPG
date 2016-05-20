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
    public partial class LevelPickWindow : Window
    {
        public String PickedLevelName;
        public int PickedLevelType;

        public LevelPickWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lbLevels.ItemsSource = ModelManager.Instance.LevelXlsData.DataList;
        }
        
        private void onBtn_Confirm(object sender, RoutedEventArgs e)
        {
            if (lbLevels.SelectedItem != null)
            {
                PickedLevelName = (lbLevels.SelectedItem as Level_Type).Name;
                PickedLevelType = int.Parse((lbLevels.SelectedItem as Level_Type).ID.ToString());
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
