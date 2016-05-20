using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigEditor
{
    public class DynamicClassDefine<Base>
    {
        public String Name;
        public String Desc;
        public List<DynamicPropertyDefine> Properties;

        public Type CreateClass()
        {
            try
            {
                Type baseType = typeof(Base);
                String baseTypeName = baseType.Name;

                string innerClassName = baseTypeName + "_" + Name;

                TypeBuilder classBuilder = AssemblyManager.Instance.ModuleBuilder.DefineType(innerClassName, TypeAttributes.Public, baseType);

                CustomAttributeBuilder classCaBuilder = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new Type[] { typeof(String) }), new Object[] { Desc });
                classBuilder.SetCustomAttribute(classCaBuilder);

                foreach (DynamicPropertyDefine pDefine in Properties)
                {
                    PropertyBuilder pBuilder = classBuilder.DefineProperty(pDefine.Name,
                        PropertyAttributes.HasDefault,
                        typeof(int),
                        null);

                    // TODO thy 如何添加多个Attribute?
                    // CustomAttributeBuilder caBuilder = new CustomAttributeBuilder(typeof(MaxValueAttribute).GetConstructor(new Type[]{typeof(int)}), new Object[] { 100 });
                    // pBuilder.SetCustomAttribute(caBuilder);

                    CustomAttributeBuilder caBuilder = new CustomAttributeBuilder(typeof(LogicTypeAttribute).GetConstructor(new Type[] { typeof(ELogicType) }), new Object[] { pDefine.LogicType });
                    pBuilder.SetCustomAttribute(caBuilder);

                    FieldBuilder fBuilder = classBuilder.DefineField("_" + pDefine.Name,
                        typeof(int),
                        FieldAttributes.Private);

                    MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

                    MethodBuilder getMBuilder = classBuilder.DefineMethod("get_" + pDefine.Name,
                        getSetAttr, typeof(int), Type.EmptyTypes);

                    ILGenerator getGenerator = getMBuilder.GetILGenerator();
                    getGenerator.Emit(OpCodes.Ldarg_0);
                    getGenerator.Emit(OpCodes.Ldfld, fBuilder);
                    getGenerator.Emit(OpCodes.Ret);

                    MethodBuilder setMBuilder = classBuilder.DefineMethod("set_" + pDefine.Name,
                        getSetAttr, null, new Type[] { typeof(int) });

                    ILGenerator setGenerator = setMBuilder.GetILGenerator();
                    setGenerator.Emit(OpCodes.Ldarg_0);
                    setGenerator.Emit(OpCodes.Ldarg_1);
                    setGenerator.Emit(OpCodes.Stfld, fBuilder);
                    setGenerator.Emit(OpCodes.Ldarg_0);
                    setGenerator.Emit(OpCodes.Ldstr, pDefine.Name);
                    setGenerator.Emit(OpCodes.Call, typeof(SkillCondition).GetMethod("OnPropertyChanged"));
                    setGenerator.Emit(OpCodes.Ret);

                    pBuilder.SetGetMethod(getMBuilder);
                    pBuilder.SetSetMethod(setMBuilder);
                }

                Type newType = classBuilder.CreateType();
                return newType;
            }
            catch(Exception e)
            {
                StringBuilder errorStrBuilder = new StringBuilder();
                errorStrBuilder.AppendLine("动态类型创建失败，请检查动态类型配置是否正确，或联系编辑器维护人员");
                errorStrBuilder.AppendLine("错误信息如下: ");
                errorStrBuilder.AppendLine(e.ToString());
                errorStrBuilder.AppendLine(e.Message);
                errorStrBuilder.AppendLine(e.StackTrace);
                LogManager.Instance.ShowErrorMessageBox(errorStrBuilder.ToString());
                return null;
            }
        }
    }

    // NOTE thy 定义这些空类是为了消除Json.NET反序列化泛型类的复杂性
    public class SkillFunctionDefine : DynamicClassDefine<SkillFunction>
    {

    }

    public class SkillConditionDefine : DynamicClassDefine<SkillCondition>
    {

    }
}
