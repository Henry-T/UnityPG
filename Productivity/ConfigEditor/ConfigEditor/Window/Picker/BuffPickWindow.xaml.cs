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
    public partial class BuffPickWindow : Window
    {
        public String PickedBuffName;
        public int PickedBuffType;

        public BuffPickWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgBuffs.ItemsSource = ModelManager.Instance.BuffXlsData.DataTable.DefaultView;
        }
        
        private void onBtn_Confirm(object sender, RoutedEventArgs e)
        {
            if (dgBuffs.SelectedItem != null)
            {
                PickedBuffName = dgBuffs.SelectedItem.ToString();
                PickedBuffType = int.Parse((dgBuffs.SelectedItem as DataRowView)["ID"].ToString());
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
