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

namespace ConfigEditor
{
    /// <summary>
    /// Interaction logic for SkillEditWindow.xaml
    /// </summary>
    public partial class SkillEditWindow : Window
    {
        public ObservableCollection<SkillType> SkillTypeList;

        public SkillEditWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModelManager.Instance.Load();

            SkillTypeList = ModelManager.Instance.SkillXlsData.DataList;
            dgSkills.DataContext = SkillTypeList;

            AssemblyManager.Instance.Load();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ModelManager.Instance.SkillXlsData.Save();
            MessageBox.Show("Skill.Xls写入成功");
        }

        private void btnNewSkill_Click(object sender, RoutedEventArgs e)
        {
            cloneSkill(dgSkills.Items.Count - 1);
        }

        private SkillType cloneSkill(int cloneSrcIndex)
        {
            SkillType source = SkillTypeList[cloneSrcIndex];
            SkillType target = source.ShallowCopy();

            SkillTypeList.Add(target);
            dgSkills.SelectedItem = target;
            dgSkills.ScrollIntoView(target);

            return target;
        }

        private void onBtn_EditSkillFunction(object sender, RoutedEventArgs e)
        {
            tcDetail.SelectedIndex = 0;
        }

        private void onBtn_EditCondition(object sender, RoutedEventArgs e)
        {
            tcDetail.SelectedIndex = 1;
        }

        private void onBtn_EditDesc(object sender, RoutedEventArgs e)
        {
            tcDetail.SelectedIndex = 2;
        }

        private void onBtn_PickSkillIcon(object sender, RoutedEventArgs e)
        {
            if (dgSkills.SelectedItem == null)
                return;

            SkillIconPickWindow skillIconPickWindow = new SkillIconPickWindow();
            if (skillIconPickWindow.ShowDialog() == true)
            {
                SkillType skillType = dgSkills.SelectedItem as SkillType;
                skillType.Code = skillIconPickWindow.PickedCode;
            }
        }

        private void onBtn_EditWarrior(object sender, RoutedEventArgs e)
        {
            WarriorEditWindow warriorEditWindow = new WarriorEditWindow();
            warriorEditWindow.Left = this.Left;
            warriorEditWindow.Top = this.Top;
            warriorEditWindow.Show();
            this.Close();
        }

        private TextBox tt;
        private void lsbConditions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            spConditionParams.Children.Clear();

            if (lsbConditions.SelectedItem == null)
                return;

            SkillCondition skillCondition = lsbConditions.SelectedItem as SkillCondition;

            Type skillConditionType = skillCondition.GetType();
            foreach(var p in skillConditionType.GetProperties())
            {
                Grid grid = createPropEntryGrid(p);
                spConditionParams.Children.Add(grid);
            }
        }
        
        // 创建一个动态属性编辑项
        private Grid createPropEntryGrid(PropertyInfo propInfo)
        {
            Grid grid = new Grid();

            Label lbSCParamName = new Label();
            lbSCParamName.Content = propInfo.Name;
            grid.Children.Add(lbSCParamName);

            ELogicType logicType = AssemblyUtil.GetPropertyLogicType(propInfo);

            if (logicType == ELogicType.Int || logicType == ELogicType.Percent)
            {
                TextBox tbSCParamValue = new TextBox();
                Binding tbBinding = new Binding(propInfo.Name);
                tbBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                tbSCParamValue.SetBinding(TextBox.TextProperty, tbBinding);
                tbSCParamValue.Margin = new Thickness(80, 4, 0, 4);
                grid.Children.Add(tbSCParamValue);
            }
            else if (logicType == ELogicType.String)
            {
                LogManager.Instance.Error("此版本不支持String类型的动态类属性: " + propInfo.Name);
            }
            else if (logicType == ELogicType.BuffType)
            {
                Button btnBuff = new Button();
                Binding txtBinding = new Binding(propInfo.Name);
                txtBinding.Converter = (IValueConverter)Application.Current.TryFindResource("ID2BuffNameConverter");
                btnBuff.SetBinding(Button.ContentProperty, txtBinding);
                btnBuff.Margin = new Thickness(80, 4, 0, 4);
                grid.Children.Add(btnBuff);

                btnBuff.Click += (o, re) =>
                {
                    BuffPickWindow buffPickWindow = new BuffPickWindow();
                    if (buffPickWindow.ShowDialog() == true)
                    {
                        propInfo.SetValue(btnBuff.DataContext, buffPickWindow.PickedBuffType);
                    }
                };
            }
            else if (logicType == ELogicType.Element)
            {
                ComboBox cb = new ComboBox();
                cb.ItemsSource = Enum.GetNames(typeof(EElement));

                Binding binding = new Binding(propInfo.Name);
                binding.Converter = (IValueConverter)Application.Current.TryFindResource("Enum2StrConverter");
                binding.ConverterParameter = typeof(EElement);
                cb.SetBinding(ComboBox.TextProperty, binding);
                cb.Margin = new Thickness(80, 4, 0, 4);
                grid.Children.Add(cb);
            }
            else
            {
                LogManager.Instance.Error("配置了不支持的参数类型: " + propInfo.Name + " - " + logicType);
            }

            return grid;
        }

        private void onBtn_Test(object sender, RoutedEventArgs e)
        {
            // tt.Text = "8888";

            // PropertyInfo[] pInfo = typeof(Normal).GetProperties();
            // object xx = pInfo[0].GetCustomAttributes();

            dynamicAttributeTest();
        }

        private void dynamicAttributeTest()
        {

            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "ConfigEditorD";

            AssemblyBuilder newAssembly = Thread.GetDomain().DefineDynamicAssembly(
                assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder newModule = newAssembly.DefineDynamicModule("ConfigEditor");

            string innerClassName = "SkillCondtion_33";

            TypeBuilder classBuilder = newModule.DefineType(innerClassName, TypeAttributes.Public, typeof(SkillCondition));

            PropertyBuilder pBuilder = classBuilder.DefineProperty("erer",
                PropertyAttributes.HasDefault,
                typeof(int),
                null);

            CustomAttributeBuilder caBuilder = new CustomAttributeBuilder(typeof(LogicTypeAttribute).GetConstructor(new Type[] { typeof(String) }), new Object[] { "awsd" });
            pBuilder.SetCustomAttribute(caBuilder);

            FieldBuilder fBuilder = classBuilder.DefineField("_erer",
                typeof(int),
                FieldAttributes.Private);

            MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            MethodBuilder getMBuilder = classBuilder.DefineMethod("get_erer",
                getSetAttr, typeof(int), Type.EmptyTypes);
            getMBuilder.SetCustomAttribute(caBuilder);

            ILGenerator getGenerator = getMBuilder.GetILGenerator();
            getGenerator.Emit(OpCodes.Ldarg_0);
            getGenerator.Emit(OpCodes.Ldfld, fBuilder);
            getGenerator.Emit(OpCodes.Ret);

            MethodBuilder setMBuilder = classBuilder.DefineMethod("set_erer",
                getSetAttr, null, new Type[] { typeof(int) });
            setMBuilder.SetCustomAttribute(caBuilder);

            ILGenerator setGenerator = setMBuilder.GetILGenerator();
            setGenerator.Emit(OpCodes.Ldarg_0);
            setGenerator.Emit(OpCodes.Ldarg_1);
            setGenerator.Emit(OpCodes.Stfld, fBuilder);
            setGenerator.Emit(OpCodes.Ldarg_0);     // TODO thy 验证事件成功调用 未成功!
            setGenerator.Emit(OpCodes.Ldstr, "_erer");
            setGenerator.Emit(OpCodes.Call, typeof(SkillCondition).GetMethod("OnPropertyChanged"));
            setGenerator.Emit(OpCodes.Ret);

            pBuilder.SetGetMethod(getMBuilder);
            pBuilder.SetSetMethod(setMBuilder);
            Type newType = classBuilder.CreateType();

        }

        private void onBtn_SaveConditions(object sender, RoutedEventArgs e)
        {
            if (dgSkills.SelectedItem != null)
            {
                var list = lsbConditions.ItemsSource as ObservableCollection<SkillCondition>;
                string conditionStr = Json2SkillConditionsConverter.ConvertListToStr(list);
                (dgSkills.SelectedItem as SkillType).Conditions = conditionStr;
            }
        }

        private void lsbFunctions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            spFunctionParams.Children.Clear();

            if (lsbFunctions.SelectedItem == null)
                return;

            SkillFunction skillFunction = lsbFunctions.SelectedItem as SkillFunction;

            Type skillConditionType = skillFunction.GetType();
            foreach (var p in skillConditionType.GetProperties())
            {
                Grid grid = createPropEntryGrid(p);
                spFunctionParams.Children.Add(grid);
            }
        }

        private void onBtn_SaveFunctions(object sender, RoutedEventArgs e)
        {
            if (dgSkills.SelectedItem != null)
            {
                var list = lsbFunctions.ItemsSource as ObservableCollection<SkillFunction>;
                string functionStr = Json2SkillFunctionsConverter.ConvertListToStr(list);
                (dgSkills.SelectedItem as SkillType).Functions = functionStr;
            }
        }

        private void onBtn_DeleteFunction(object sender, RoutedEventArgs e)
        {
            if (lsbFunctions.ItemsSource == null)
                return;
            
            var list = (ObservableCollection<SkillFunction>)lsbFunctions.ItemsSource;
            list.Remove((SkillFunction)lsbFunctions.SelectedItem);
        }

        private void onBtn_DeleteCondition(object sender, RoutedEventArgs e)
        {
            if (lsbConditions.ItemsSource == null)
                return;

            var list = (ObservableCollection<SkillCondition>)lsbConditions.ItemsSource;
            list.Remove((SkillCondition)lsbConditions.SelectedItem);
        }

        private void onBtn_AddFunction(object sender, RoutedEventArgs e)
        {
            if (lsbFunctions.ItemsSource == null)
                return;

            SkillFunctionPickWindow window = new SkillFunctionPickWindow();
            if(window.ShowDialog() == true)
            {
                SkillFunction newFunction = (SkillFunction)Activator.CreateInstance(window.PickedType);
                var list = (ObservableCollection<SkillFunction>)lsbFunctions.ItemsSource;
                list.Add(newFunction);
            }
        }

        private void onBtn_AddCondition(object sender, RoutedEventArgs e)
        {
            if (lsbConditions.ItemsSource == null)
                return;

            SkillConditionPickWindow window = new SkillConditionPickWindow();
            if (window.ShowDialog() == true)
            {
                SkillCondition newCondition = (SkillCondition)Activator.CreateInstance(window.PickedType);
                var list = (ObservableCollection<SkillCondition>)lsbConditions.ItemsSource;
                list.Add(newCondition);
            }

        }

        private void onBtn_DeleteSkill(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("删除'技能'可能造成引用丢失，请慎重操作。", "是否继续?", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var lvl = (SkillType)dgSkills.SelectedItem;
                ModelManager.Instance.SkillXlsData.DataList.Remove(lvl);
            }
        }

        private void ucIDJumper_OnJump(int id)
        {
            IEnumerable<SkillType> searchResult = SkillTypeList.Where(st => st.ID == id);

            if (searchResult.Count() > 0)
            {
                SkillType findType = searchResult.First();

                dgSkills.ScrollIntoView(findType);
                dgSkills.SelectedItem = findType;
            }

            // ModelManager.Instance.WarriorXlsData.DataTable.Select("ID = " + idStr);
        }
    }
}
