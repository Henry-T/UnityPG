using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ConfigEditor
{
    class Json2SkillFunctionsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string jsonStr = value.ToString();

            ObservableCollection<SkillFunction> skillConditionList = new ObservableCollection<SkillFunction>();

            JArray jArray = JArray.Parse(jsonStr);
            foreach (JObject jo in jArray)
            {
                String className = jo["ClassName"].ToString();
                jo.Remove("ClassName");

                if (!AssemblyManager.Instance.SkillFunctionTypes.ContainsKey(className))
                {
                    LogManager.Instance.Warn("找不到对应的动态类: " + className + ", 此数据已直接丢弃。");
                    LogManager.Instance.Warn("请确保动态类型配置中已定义了此类型，然后重启编辑器。");
                    continue;
                }

                Type type = AssemblyManager.Instance.SkillFunctionTypes[className];
                SkillFunction skillCondition = JsonConvert.DeserializeObject(jo.ToString(), type) as SkillFunction;

                skillConditionList.Add(skillCondition);
            }

            return skillConditionList;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // NOTE thy 为什么ObservableCollection无法ConvertBack
            // http://stackoverflow.com/questions/2816163/when-will-the-valueconverters-convert-method-be-called-in-wpf
            return ConvertListToStr(value as ObservableCollection<SkillFunction>);

        }

        public static string ConvertListToStr(ObservableCollection<SkillFunction> list)
        {
            ObservableCollection<SkillFunction> skillFunctionList = list;

            JArray jArray = new JArray();

            foreach (SkillFunction condition in skillFunctionList)
            {
                Type type = condition.GetType();

                JObject jCond = JObject.Parse(JsonConvert.SerializeObject(condition));
                jCond["ClassName"] = type.Name.Replace("SkillFunction_", "");
                jArray.Add(jCond);
            }

            return jArray.ToString(Formatting.None);
        }
    }
}
