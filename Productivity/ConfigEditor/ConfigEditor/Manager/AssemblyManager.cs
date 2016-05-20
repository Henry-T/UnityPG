using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.ComponentModel;
using System.Reflection.Emit;
using System.Reflection;
using System.Threading;

namespace ConfigEditor
{
    public class AssemblyManager
    {
        private static AssemblyManager instance;
        public static AssemblyManager Instance 
        {
            get { 
                if (instance == null)
                {
                    instance = new AssemblyManager();
                    instance.Load();
                }
                return instance;
            }
        }

        public DynamicDefineData DynamicDefineData;

        public Dictionary<String, Type> SkillConditionTypes;
        public Dictionary<String, Type> SkillFunctionTypes;

        public AssemblyBuilder AssemblyBuilder;
        public ModuleBuilder ModuleBuilder;

        private bool loaded = false;
        public void Load()
        {
            if (loaded)
                return;
            else
                loaded = true;

            // 初始化动态程序集
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "ConfigEditorDynamic";
            AssemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder = AssemblyBuilder.DefineDynamicModule("ConfigEditor");

            // 创建动态类
            loadDynamicTypes();
        }

        private void loadDynamicTypes()
        {
            string typeExtPath = "TypeExt.json";

            try
            {
                string jsonStr = File.ReadAllText(typeExtPath);
                try
                {
                    DynamicDefineData = JsonConvert.DeserializeObject<DynamicDefineData>(jsonStr);
                }
                catch(Exception e)
                {
                    StringBuilder errStrBuilder = new StringBuilder();
                    errStrBuilder.AppendLine("动态类型配置反序列化时出错，请检查配置是否正确。");
                    errStrBuilder.AppendLine("信息如下:");
                    errStrBuilder.AppendLine(e.Message);
                    errStrBuilder.AppendLine(e.StackTrace);
                    LogManager.Instance.ShowErrorMessageBox(errStrBuilder.ToString());
                    return;
                }

                SkillConditionTypes = new Dictionary<string, Type>();
                SkillFunctionTypes = new Dictionary<string, Type>();

                if (DynamicDefineData.SkillConditions != null)
                {
                    foreach (SkillConditionDefine scDefine in DynamicDefineData.SkillConditions)
                    {
                        Type newType = scDefine.CreateClass();
                        if (newType != null)
                            SkillConditionTypes.Add(scDefine.Name, newType);
                    }
                }

                if (DynamicDefineData.SkillFunctions != null)
                {
                    foreach (SkillFunctionDefine scDefine in DynamicDefineData.SkillFunctions)
                    {
                        Type newType = scDefine.CreateClass();
                        if (newType != null)
                            SkillFunctionTypes.Add(scDefine.Name, newType);
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                string info = "找不到配置数据扩展文件，请确保它存在后重启编辑器！ " + typeExtPath;
                LogManager.Instance.ShowErrorMessageBox(info);
            }
        }
    }
}
