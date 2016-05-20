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
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using System.Data;
using Path = System.IO.Path;
using SimpleJSON;
using System.ComponentModel;

namespace ConfigEditor
{
    /// <summary>
    /// Interaction logic for WarriorEditWindow.xaml
    /// </summary>
    public partial class WarriorEditWindow : Window
    {
        public List<string> ProfessionList;
        private DataView warriorDataView;

        public WarriorEditWindow()
        {
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // NOTE 
            ModelManager.Instance.Load();

            RenderAppManager.Instance.Initialize();
            RenderAppManager.Instance.DoEmbed(this, rtRenderAppHolder);
            //RenderAppManager.Instance.LoadSpine("spine001", @"E:\proj\ProjectS\ProjectS\client\trunk\projects\res\spine\spine001.zip");
            //RenderAppManager.Instance.PlayAnim("monster001_wait");


            warriorDataView = ModelManager.Instance.WarriorXlsData.DataTable.AsDataView();
            warriorDataView.RowFilter = "ID < " + WarriorConfig.NatureStartID;
            warriorDataView.Sort = "ID";
            dgWarrior.DataContext = warriorDataView;

            lstGradePresets.ItemsSource = ModelManager.Instance.GradeSetupXlsData.NatureRequirePresets;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ModelManager.Instance.WarriorXlsData.Save();
            MessageBox.Show("Heor.Xls写入成功");
        }

        private void onBtn_EditDescription(object sender, RoutedEventArgs e)
        {
            tcDetail.SelectedIndex = 1;
        }

        private void onBtn_EditProperty(object sender, RoutedEventArgs e)
        {
            tcDetail.SelectedIndex = 0;
            // tbDescription.DataContext = dataTable; // NOTE 这样竟然可以拿到第一行 
        }
        private void onBtn_EditGrade(object sender, RoutedEventArgs e)
        {
            tcDetail.SelectedIndex = 3;
        }
        private void onBtn_EditSkill(object sender, RoutedEventArgs e)
        {
            tcDetail.SelectedIndex = 2;
        }
        private void onBtn_Clone(object sender, RoutedEventArgs e)
        {
            cloneWarrior(dgWarrior.SelectedIndex);
        }

        private void OnWarriorGridSelChanged(object sender, SelectionChangedEventArgs e)
        {
            // 切换Spine动画
            DataRowView dataRowView = dgWarrior.SelectedItem as DataRowView;

            // NOTE thy 卷动时ComboBox会引发另一个SelectionChanged事件，因此要检查事件源
            // https://wpf.codeplex.com/workitem/11077
            if (e.OriginalSource == dgWarrior && dataRowView != null)
            {
                previewAvatar(dataRowView["AvatarType"].ToString());
            }
        }

        private void onBtn_PickSpine(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = dgWarrior.SelectedItem as DataRowView;

            if (dataRowView != null)
            {
                AvatarPickWindow spinePickWindow = new AvatarPickWindow();
                if(spinePickWindow.ShowDialog() == true)
                {
                    (dgWarrior.SelectedItem as DataRowView)["AvatarType"] = spinePickWindow.PickedAvatarID;
                    previewAvatar(spinePickWindow.PickedAvatarID.ToString());
                }
            }
        }

        public void previewAvatar(string avatarTypeStr)
        {
            DataRowView avatarRowView = ModelManager.Instance.AvatarXlsData.GetRowViewByKey(avatarTypeStr);

            if (avatarRowView == null)
                return;

            // TODO thy 容错
            string project = avatarRowView["Spine"].ToString();
            string code = avatarRowView["Avatar"].ToString();

            // TODO thy spine zip 从设置路径加载
            RenderAppManager.Instance.LoadSpine(project, Path.Combine(UserConfigManager.Instance.Config.ResDir, "spine/" + project + ".zip"));
            RenderAppManager.Instance.PlayAnim(code + "_wait");
        }

        private void onBtn_PreviewSkill(object sender, RoutedEventArgs e)
        {
            if (ucAvatar.DataContext == null)
                return;

            Object tag = (sender as Button).Tag;
            int skillIndex = int.Parse(tag.ToString());

            DataRowView data = ucAvatar.DataContext as DataRowView;
            RenderAppManager.Instance.PlayAnim(data["Avatar"].ToString() + "_skill" + skillIndex);
        }

        private void onBtn_ChangeSkill1(object sender, RoutedEventArgs e)
        {
            if (dgWarrior.SelectedItem != null)
            {
                SkillPickWindow skillPickWindow = new SkillPickWindow();
                if (skillPickWindow.ShowDialog() == true)
                {
                    (dgWarrior.SelectedItem as DataRowView)["SkillA"] = "[" + skillPickWindow.PickedSkillType + "]";
                }
            }
        }
        private void onBtn_ChangeSkill2(object sender, RoutedEventArgs e)
        {
            if (dgWarrior.SelectedItem != null)
            {
                SkillPickWindow skillPickWindow = new SkillPickWindow();
                if (skillPickWindow.ShowDialog() == true)
                {
                    (dgWarrior.SelectedItem as DataRowView)["SkillB"] = "[" + skillPickWindow.PickedSkillType + "]";
                }
            }
        }
        private void onBtn_ChangeSkill3(object sender, RoutedEventArgs e)
        {
            if (dgWarrior.SelectedItem != null)
            {
                SkillPickWindow skillPickWindow = new SkillPickWindow();
                if (skillPickWindow.ShowDialog() == true)
                {
                    (dgWarrior.SelectedItem as DataRowView)["SkillC"] = "[" + skillPickWindow.PickedSkillType + "]";
                }
            }
        }
        private void onBtn_ChangeSkill4(object sender, RoutedEventArgs e)
        {
            if (dgWarrior.SelectedItem != null)
            {
                SkillPickWindow skillPickWindow = new SkillPickWindow();
                if (skillPickWindow.ShowDialog() == true)
                {
                    (dgWarrior.SelectedItem as DataRowView)["SkillD"] = "[" + skillPickWindow.PickedSkillType + "]";
                }
            }
        }

        private void onBtn_ClearSkill1(object sender, RoutedEventArgs e)
        {
            if (dgWarrior.SelectedItem != null)
            {
                (dgWarrior.SelectedItem as DataRowView)["SkillA"] = "[0]";
            }
        }
        private void onBtn_ClearSkill2(object sender, RoutedEventArgs e)
        {
            if (dgWarrior.SelectedItem != null)
            {
                (dgWarrior.SelectedItem as DataRowView)["SkillB"] = "[0]";
            }
        }
        private void onBtn_ClearSkill3(object sender, RoutedEventArgs e)
        {
            if (dgWarrior.SelectedItem != null)
            {
                (dgWarrior.SelectedItem as DataRowView)["SkillC"] = "[0]";
            }
        }
        private void onBtn_ClearSkill4(object sender, RoutedEventArgs e)
        {
            if (dgWarrior.SelectedItem != null)
            {
                (dgWarrior.SelectedItem as DataRowView)["SkillD"] = "[0]";
            }
        }

        private void onBtn_Delete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("删除'武将'可能造成引用丢失，请慎重操作。", "是否继续?", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                (dgWarrior.SelectedItem as DataRowView).Delete();
            }
        }

        private void btnNewWarrior_Click(object sender, RoutedEventArgs e)
        {
            cloneWarrior(dgWarrior.Items.Count - 1);
        }

        private DataRowView cloneWarrior(int cloneSrcIndex)
        {
            DataRowView srcRow = warriorDataView[cloneSrcIndex];
            DataRowView lastRow = warriorDataView[warriorDataView.Count - 1];
            DataRowView newRow = warriorDataView.AddNew();

            DataView searchView = ModelManager.Instance.WarriorXlsData.DataTable.AsDataView();
            searchView.RowFilter = "ID < " + WarriorConfig.NatureStartID;
            searchView.Sort = "ID";

            String keyName = ModelManager.Instance.WarriorXlsData.KeyName;
            DataRow[] maxIdResult =  ModelManager.Instance.WarriorXlsData.DataTable.Select(String.Format("{0} = MAX({0})", keyName));
            // int lastKey = int.Parse(maxIdResult[0].Field<string>(keyName));

            int lastKey = int.Parse(searchView[searchView.Count - 1]["ID"].ToString());

            foreach (var pair in ModelManager.Instance.WarriorXlsData.HeadMap)
            {
                if (pair.Key == keyName)
                { // ID 要单独设置
                    newRow[keyName] = (lastKey + 1).ToString();
                }
                else
                {
                    var srcVal = srcRow[pair.Key];
                    newRow[pair.Key] = srcVal;
                }
            }

            dgWarrior.SelectedItem = newRow;
            dgWarrior.ScrollIntoView(newRow);
            
            return newRow;
                        

            // TODO thy 移除
            //DataTable table = ModelManager.Instance.WarriorXlsData.DataTable;
            //String keyName = ModelManager.Instance.WarriorXlsData.KeyName;

            //DataRow srcRow = table.Rows[cloneSrcIndex];
            //DataRow lastRow = table.Rows[table.Rows.Count - 1];
            //DataRow newRow = table.NewRow();

            //foreach (var pair in ModelManager.Instance.WarriorXlsData.HeadMap)
            //{
            //    var srcVal = srcRow.Field<String>(pair.Key);
            //    newRow.SetField<String>(pair.Key, srcVal);
            //}

            //int lastKey = int.Parse(lastRow.Field<String>(keyName));
            //newRow.SetField<String>(keyName, (lastKey + 1).ToString());

            //table.Rows.Add(newRow);
            //return newRow;
        }

        private void btnSaveGrade_Click(object sender, RoutedEventArgs e)
        {
            if (dgWarrior.SelectedItem == null)
                return;

            NatureRequirement natureRequire = dgGrade.ItemsSource as NatureRequirement;

            if (natureRequire == null)
                return;

            string gradeConditionStr = natureRequire.ConvertToJson();

            (dgWarrior.SelectedItem as DataRowView)["GradeCondition"] = gradeConditionStr;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void onFrom_Changed(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (sender as ComboBox);

            DataRowView rowView = comboBox.DataContext as DataRowView;
            Console.WriteLine(comboBox.Text);

        }

        private void onBtn_UpgradeType(object sender, RoutedEventArgs e)
        {
            DataRowView rowView = dgWarrior.SelectedItem as DataRowView;
            // int from 
            int group = int.Parse(rowView["Group"].ToString());
            int from = int.Parse(rowView["From"].ToString());

            int newType = 10000 + from * 1000 + group * 10;
            try
            {
                rowView["ID"] = newType;
            }
            catch(Exception ex)
            {
                if (ex is ConstraintException)
                {
                    MessageBox.Show("计算 Type 与现有值冲突，无法保留结果");
                }
            }
        }

        private void onBtn_DeleteGrade(object sender, RoutedEventArgs e)
        {
            NatureList natureList = dgGrade.SelectedItem as NatureList;
            if (natureList == null)
                return;

            NatureRequirement x = dgGrade.ItemsSource as NatureRequirement;
            x.Remove(natureList);
        }

        private void onBtn_LoadGradePreset(object sender, RoutedEventArgs e)
        {
            NatureRequirePreset preset = lstGradePresets.SelectedItem as NatureRequirePreset;
            DataRowView warriorRowView = dgWarrior.SelectedItem as DataRowView;
            if(preset != null && warriorRowView != null)
            {
                warriorRowView["GradeCondition"] = preset.Requirement.ConvertToJson();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            RenderAppManager.Instance.Detach();
        }

        private void IDJumperUC_OnJump(int id)
        {
            int rowIndex = warriorDataView.Find(id);

            if (rowIndex != -1)
            {
                DataRowView rowView = warriorDataView[rowIndex];

                dgWarrior.ScrollIntoView(rowView);
                dgWarrior.SelectedItem = rowView;
            }
        }
    }
}
