using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using System.ComponentModel;

namespace ConfigEditor
{
    // NOTE thy 这里不使用PropertyChanged插件，便于维护
    //[ImplementPropertyChanged]
    //public class SkillCondition
    //{
    //    public string Some { get; set; }
    //}

    public class NotifyPropChangeObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class SkillCondition : NotifyPropChangeObject
    {

    }

    //public class SkillFunction : NotifyPropChangeObject
    //{

    //}

    //public class SkillCondition : INotifyPropertyChanged
    //{
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    public virtual void OnPropertyChanged(string propertyName)
    //    {
    //        var propertyChanged = PropertyChanged;
    //        if (propertyChanged != null)
    //        {
    //            propertyChanged(this, new PropertyChangedEventArgs(propertyName));
    //        }
    //    }
    //}

    // 静态实现的测试类
    //public class SkillCondition_Compile_HasBuff : SkillCondition
    //{
    //    public int Type { get; set; }

    //}

    //public class SkillCondition_Compile_MinHP : SkillCondition
    //{
    //    public int Rate { get; set; }
    //}
}
