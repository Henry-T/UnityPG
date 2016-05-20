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

namespace ConfigEditor
{
    /// <summary>
    /// Interaction logic for LockKillerWindow.xaml
    /// </summary>
    public partial class LockKillerWindow : Window
    {
        public LockKillerWindow()
        {
            InitializeComponent();
        }

        private void onBtn_Confirm(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void onBtn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
