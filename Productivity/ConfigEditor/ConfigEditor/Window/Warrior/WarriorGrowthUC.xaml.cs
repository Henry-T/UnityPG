using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for WarriorGrowthUC.xaml
    /// </summary>
    public partial class WarriorGrowthUC : UserControl
    {
        public WarriorGrowthUC()
        {
            InitializeComponent();
        }

        private static bool CheckTextBoxNumber(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void PreviewNumberInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !CheckTextBoxNumber(e.Text);
        }


    }
}
