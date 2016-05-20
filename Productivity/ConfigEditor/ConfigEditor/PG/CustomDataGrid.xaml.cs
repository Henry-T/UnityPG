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
    /// Interaction logic for CustomDataGrid.xaml
    /// </summary>
    public partial class CustomDataGrid : Window
    {
        private List<ENature[]> data;

        public CustomDataGrid()
        {
            InitializeComponent();

            grid.AutoGenerateColumns = false;

            data = new List<ENature[]>();
            ENature[] d1 = new ENature[8];
            d1[0] = ENature.毒;

            ENature[] d2 = new ENature[8];
            d2[0] = ENature.毒;
            d2[1] = ENature.毒;
            d2[2] = ENature.毒;

            ENature[] d3 = new ENature[8];
            d2[0] = ENature.毒;
            d2[1] = ENature.毒;
            d2[2] = ENature.毒;
            d2[3] = ENature.毒;

            data.Add(d1);
            data.Add(d2);
            data.Add(d3);

            //for (int i=0; i<8; i++)
            //{
            //    // var col = new DataGridTextColumn();
            //    // var binding = new Binding("[" + i + "]");
            //    // col.Binding = binding;

            //    var col = new DataGridComboBoxColumn();
            //    var binding = new Binding("[" + i + "]");
            //    col.TextBinding = binding;

            //    col.ItemsSource = Enum.GetNames(typeof(ENature));
                
            //    Style style = new Style();
            //    style.Setters.Add(new EventSetter(TextBox.TextChangedEvent, new TextChangedEventHandler(onTextChanged)));
            //    col.CellStyle = style;

            //    grid.Columns.Add(col);
            //}

            grid.ItemsSource = data;
        }
        private void onTextChanged(object sender, RoutedEventArgs e)
        {
            DataGridCell dc = sender as DataGridCell;
            TextChangedEventArgs args = e as TextChangedEventArgs;

            int[] d = grid.SelectedItem as int[];
            
            // DataGridTextColumn c = (grid.Columns[data.IndexOf(d)] as DataGridTextColumn);

            // Console.WriteLine("some");
        }
    }
}
