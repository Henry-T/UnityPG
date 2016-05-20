using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ConfigEditor
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        public EditorWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbExcelDir.Text = UserConfigManager.Instance.Config.ExcelDir;
            tbResDir.Text = UserConfigManager.Instance.Config.ResDir;
        }

        private void btnPickExcelDir_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                String path = folderBrowserDialog.SelectedPath;
                UserConfigManager.Instance.Config.ExcelDir = path;
                UserConfigManager.Instance.Save();
                tbExcelDir.Text = path;
            }
        }

        private void btnPickResDir_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                String path = folderBrowserDialog.SelectedPath;
                UserConfigManager.Instance.Config.ResDir = path;
                UserConfigManager.Instance.Save();
                tbResDir.Text = path;
            }
        }

        private void btnWarriorEditor_Click(object sender, RoutedEventArgs e)
        {
            WarriorEditWindow warriorEditWindow = new WarriorEditWindow();
            App.Current.MainWindow = warriorEditWindow;
            this.Close();
            warriorEditWindow.Show();
        }
    }

}
