using System;
using System.Collections.Generic;
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
using System.IO;
using System.Data;

namespace ConfigEditor
{
    /// <summary>
    /// Interaction logic for SpinePickWindow.xaml
    /// </summary>
    public partial class WarriorPickWindow : Window
    {
        public String PickedWarriorName;
        public int PickedWarriorID;
        public DataRowView PickedWarriorData;

        public WarriorPickWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataView dataView = ModelManager.Instance.WarriorXlsData.DataTable.AsDataView();
            // dataView.Sort = "ID";
            lbWarriors.ItemsSource = dataView;
        }

        private void onBtn_Confirm(object sender, RoutedEventArgs e)
        {
            if (lbWarriors.SelectedItem != null)
            {
                PickedWarriorData = lbWarriors.SelectedItem as DataRowView;
                PickedWarriorID = int.Parse(PickedWarriorData["ID"].ToString());

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
