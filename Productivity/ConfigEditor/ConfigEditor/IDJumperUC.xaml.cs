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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConfigEditor
{
    /// <summary>
    /// Interaction logic for IDJumperUC.xaml
    /// </summary>
    public partial class IDJumperUC : UserControl
    {
        public delegate void OnJumpEventHandler(int id);
        public event OnJumpEventHandler OnJump;

        public IDJumperUC()
        {
            InitializeComponent();
        }

        private void onBtn_Jump(object sender, RoutedEventArgs e)
        {
            int id = 0;
            if(int.TryParse(tbID.Text, out id))
            {
                if (OnJump != null)
                    OnJump(id);
            }
        }
    }
}
