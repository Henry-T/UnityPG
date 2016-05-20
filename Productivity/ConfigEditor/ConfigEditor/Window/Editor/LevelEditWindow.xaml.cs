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
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.ComponentModel;
using Label = System.Windows.Controls.Label;
using PropertyChanged;

namespace ConfigEditor
{
    /// <summary>
    /// Interaction logic for SkillEditWindow.xaml
    /// </summary>
    [ImplementPropertyChanged]
    public partial class LevelEditWindow : Window
    {
        public ObservableCollection<Level_Type> LevelTypeList;

        public int PickedWave { get; set; }

        public LevelEditWindow()
        {
            PickedWave = 1;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModelManager.Instance.Load();

            LevelTypeList = ModelManager.Instance.LevelXlsData.DataList;
            dgLevels.DataContext = LevelTypeList;

            AssemblyManager.Instance.Load();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ModelManager.Instance.LevelXlsData.Save();
            ModelManager.Instance.LevelNpcXlsData.Save();
            MessageBox.Show("Level.Xls 和 LevelSet.xls写入成功");
        }

        private void btnNewLevel_Click(object sender, RoutedEventArgs e)
        {
            cloneLevel(dgLevels.Items.Count - 1);
        }

        private Level_Type cloneLevel(int cloneSrcIndex)
        {
            Level_Type source = LevelTypeList[cloneSrcIndex];
            Level_Type target = (Level_Type)source.ShallowCopy();

            LevelTypeList.Add(target);
            dgLevels.SelectedItem = target;
            dgLevels.ScrollIntoView(target);

            return target;
        }

        private void onBtn_EditWarrior(object sender, RoutedEventArgs e)
        {
            WarriorEditWindow warriorEditWindow = new WarriorEditWindow();
            warriorEditWindow.Left = this.Left;
            warriorEditWindow.Top = this.Top;
            warriorEditWindow.Show();
            this.Close();
        }

        private void onBtn_SaveConditions(object sender, RoutedEventArgs e)
        {
            //if (dgLevels.SelectedItem != null)
            //{
            //    var list = lsbConditions.ItemsSource as ObservableCollection<SkillCondition>;
            //    string conditionStr = Json2SkillConditionsConverter.ConvertListToStr(list);
            //    (dgLevels.SelectedItem as Level_Type).Conditions = conditionStr;
            //}
        }

        private void onBtn_SaveNPCs(object sender, RoutedEventArgs e)
        {
            //if (dgLevels.SelectedItem != null)
            //{
            //    var list = lsbFunctions.ItemsSource as ObservableCollection<SkillFunction>;
            //    string functionStr = Json2SkillFunctionsConverter.ConvertListToStr(list);
            //    (dgLevels.SelectedItem as Level_Type).Functions = functionStr;
            //}
        }

        private void onBtn_DeleteNPC(object sender, RoutedEventArgs e)
        {
            if (dgNPCs.ItemsSource == null)
                return;

            var list = (ObservableCollection<LevelNPCType>)dgNPCs.ItemsSource;
            LevelNPCType npcToDelete = (LevelNPCType)dgNPCs.SelectedItem;
            list.Remove(npcToDelete);
            ModelManager.Instance.LevelNpcXlsData.DataList.Remove(npcToDelete);
        }

        private void onBtn_DeleteAwards(object sender, RoutedEventArgs e)
        {
            if (lsbAwards.ItemsSource == null)
                return;

            var list = (ObservableCollection<SkillCondition>)lsbAwards.ItemsSource;
            list.Remove((SkillCondition)lsbAwards.SelectedItem);
        }

        private void onBtn_AddNPC(object sender, RoutedEventArgs e)
        {
            if (dgLevels.SelectedItem == null || dgNPCs.ItemsSource == null)
                return;

            Level_Type level = (Level_Type)dgLevels.SelectedItem;

            var newItem = ModelManager.Instance.LevelNpcXlsData.CreateNew();
            newItem.Level = level.ID;
            newItem.Wave = PickedWave;

            var list = (ObservableCollection<LevelNPCType>)dgNPCs.ItemsSource;
            list.Add(newItem);
        }

        private void onBtn_AddAwards(object sender, RoutedEventArgs e)
        {
            if (lsbAwards.ItemsSource == null)
                return;

            SkillConditionPickWindow window = new SkillConditionPickWindow();
            if (window.ShowDialog() == true)
            {
                SkillCondition newCondition = (SkillCondition)Activator.CreateInstance(window.PickedType);
                var list = (ObservableCollection<SkillCondition>)lsbAwards.ItemsSource;
                list.Add(newCondition);
            }

        }

        private void onBtn_ChangeLastLevel(object sender, RoutedEventArgs e)
        {
            if (dgLevels.SelectedItem == null)
                return;

            LevelPickWindow window = new LevelPickWindow();
            if (window.ShowDialog() == true)
            {
                Level_Type Level_Type = dgLevels.SelectedItem as Level_Type;
                Level_Type.LastLevel = window.PickedLevelType;
            }
        }

        private void onBtn_DeleteLevel(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("删除'关卡'可能造成引用丢失，请慎重操作。", "是否继续?", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var lvl = (Level_Type)dgLevels.SelectedItem;
                ModelManager.Instance.LevelXlsData.DataList.Remove(lvl);
            }
        }


        private void onBtn_ChangeBattleBg(object sender, RoutedEventArgs e)
        {
            if (dgLevels.SelectedItem == null)
                return;

            LevelBgPickWindow window = new LevelBgPickWindow();
            if (window.ShowDialog() == true)
            {
                Level_Type Level_Type = dgLevels.SelectedItem as Level_Type;
                Level_Type.BattleBackground = window.PickedLevelBgType;
            }
        }

        #region tag switch

        private void onBtn_EditDesc(object sender, RoutedEventArgs e)
        {
            tcDetail.SelectedIndex = 0;
        }

        private void onBtn_EditNPC(object sender, RoutedEventArgs e)
        {
            tcDetail.SelectedIndex = 1;
        }

        private void onBtn_EditShowAward(object sender, RoutedEventArgs e)
        {
            tcDetail.SelectedIndex = 2;
        }

        private void onBtn_EditAwards(object sender, RoutedEventArgs e)
        {
            tcDetail.SelectedIndex = 3;
        }

        private void onBtn_EditExtraAwards(object sender, RoutedEventArgs e)
        {
            tcDetail.SelectedIndex = 4;
        }
        #endregion

        private void lsbAwards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void onBtn_ChangeNPCOrigin(object sender, RoutedEventArgs e)
        {
            if (dgNPCs.SelectedItem == null)
                return;

            WarriorPickWindow window = new WarriorPickWindow();
            if (window.ShowDialog() == true)
            {
                LevelNPCType npcType = dgNPCs.SelectedItem as LevelNPCType;
                npcType.Type = window.PickedWarriorID;
            }
        }

        private void IDJumperUC_OnJump(int id)
        {
            IEnumerable<Level_Type> searchResult = LevelTypeList.Where(st => st.ID == id);

            if (searchResult.Count() > 0)
            {
                Level_Type findType = searchResult.First();

                dgLevels.ScrollIntoView(findType);
                dgLevels.SelectedItem = findType;
            }
        }
    }
}
