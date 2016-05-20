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
    /// Interaction logic for EditorSwitchUC.xaml
    /// </summary>
    public partial class EditorSwitchUC : UserControl
    {
        public EditorSwitchUC()
        {
            InitializeComponent();
        }

        private void onBtn_EditWarrior(object sender, RoutedEventArgs e)
        {
            tryShowWindow<WarriorEditWindow>();
        }

        private void onBtn_EditSkill(object sender, RoutedEventArgs e)
        {
            tryShowWindow<SkillEditWindow>();
        }

        private void onBtn_EditLevel(object sender, RoutedEventArgs e)
        {
            tryShowWindow<LevelEditWindow>();
        }

        private void tryShowWindow<T>() where T : Window, new()
        {
            if (!(Application.Current.MainWindow is T))
            {
                Window oldWindow = Application.Current.MainWindow;
                Application.Current.MainWindow = new T();
                Application.Current.MainWindow.Left = oldWindow.Left;
                Application.Current.MainWindow.Top = oldWindow.Top;
                oldWindow.Close();
                Application.Current.MainWindow.Show();
            }
        }
    }
}
