using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace ConfigEditor
{
    // SkillConditions json字符串转列表
    class Json2SkillConditionsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string jsonStr = value.ToString();

            ObservableCollection<SkillCondition> skillConditionList = new ObservableCollection<SkillCondition>();

            // NOTE thy 方案一 使用JsonConverter转换内部派生实例
            // skillConditionList = JsonConvert.DeserializeObject<ObservableCollection<SkillCondition>>(jsonStr);

            // NOTE thy 方案二 自己遍历每个元素，单独调用反序列化
            JArray jArray = JArray.Parse(jsonStr);
            foreach(JObject jo in jArray)
            {
                String className = jo["ClassName"].ToString();
                jo.Remove("ClassName");

                if (!AssemblyManager.Instance.SkillConditionTypes.ContainsKey(className))
                {
                    LogManager.Instance.Warn("找不到对应的动态类: " + className + ", 此数据已直接丢弃。");
                    LogManager.Instance.Warn("请确保动态类型配置中已定义了此类型，然后重启编辑器。");
                    continue;
                }

                Type type = AssemblyManager.Instance.SkillConditionTypes[className];
                SkillCondition skillCondition = JsonConvert.DeserializeObject(jo.ToString(), type) as SkillCondition;

                skillConditionList.Add(skillCondition);
            }

            return skillConditionList;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // NOTE thy 为什么ObservableCollection无法ConvertBack
            // http://stackoverflow.com/questions/2816163/when-will-the-valueconverters-convert-method-be-called-in-wpf
            return ConvertListToStr(value as ObservableCollection<SkillCondition>);
        }

        public static string ConvertListToStr(ObservableCollection<SkillCondition> list)
        {
            ObservableCollection<SkillCondition> skillConditionList = list;
            
            JArray jArray = new JArray();

            foreach(SkillCondition condition in skillConditionList)
            {
                Type type = condition.GetType();

                JObject jCond = JObject.Parse(JsonConvert.SerializeObject(condition));
                jCond["ClassName"] = type.Name.Replace("SkillCondition_", "");
                jArray.Add(jCond);
            }

            return jArray.ToString(Formatting.None);
        }
    }
}
