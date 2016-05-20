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

namespace ConfigEditor.PG
{
    /// <summary>
    /// Interaction logic for TwoWayIntList.xaml
    /// </summary>
    public partial class TwoWayIntList : Window
    {
        public TwoWayIntList()
        {
            InitializeComponent();

            List<IntWrap> data = new List<IntWrap>(new IntWrap[] { new IntWrap(1), new IntWrap(1), new IntWrap(1), new IntWrap(1), new IntWrap(1), new IntWrap(1), new IntWrap(1) });

            grid.ItemsSource = data;
        }
    }

    public class IntWrap
    {
        public int Value{get; set;}

        public IntWrap(int v)
        {
            Value = v;
        }
    }
}
