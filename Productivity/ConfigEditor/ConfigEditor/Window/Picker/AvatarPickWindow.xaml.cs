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
    public partial class AvatarPickWindow : Window
    {
        public String PickedSpineName;
        public String PickedSpineZip;

        public int PickedAvatarID;
        public DataRowView PickedAvatarData;

        public AvatarPickWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataView dataView = ModelManager.Instance.AvatarXlsData.DataTable.AsDataView();
            // dataView.Sort = "ID";
            lbAvatars.ItemsSource = dataView;
        }

        private void onBtn_Confirm(object sender, RoutedEventArgs e)
        {
            if (lbAvatars.SelectedItem != null)
            {
                //PickedSpineName = lbSpines.SelectedItem.ToString();
                //String resDir = ConfigManager.Instance.Get("ResDir");
                //PickedSpineZip = System.IO.Path.Combine(resDir, "spine/" + PickedSpineName + ".zip");

                PickedAvatarData = lbAvatars.SelectedItem as DataRowView;
                PickedAvatarID = int.Parse(PickedAvatarData["ID"].ToString());

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
