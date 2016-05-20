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
    public partial class SkillPickWindow : Window
    {
        public String PickedSkillName;
        public int PickedSkillType;

        public SkillPickWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<String> skillNameList = new List<string>();
            //foreach(DataRowView rowView in ModelManager.Instance.SkillXlsData.DataTable.Rows)
            //{
            //    skillNameList.Add(rowView["Name"].ToString());
            //}

            // lbSkills.DataContext = skillNameList;
            lbSkills.ItemsSource = ModelManager.Instance.SkillXlsData.DataList;
            // lbSkills.DataContext = ModelManager.Instance.SkillXlsData.DataTable.DefaultView;
        }
        
        private void onBtn_Confirm(object sender, RoutedEventArgs e)
        {
            if (lbSkills.SelectedItem != null)
            {
                PickedSkillName = lbSkills.SelectedItem.ToString();
                PickedSkillType = int.Parse((lbSkills.SelectedItem as DataRowView)["ID"].ToString());
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
