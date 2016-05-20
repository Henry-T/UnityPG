using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ConfigEditor
{
    [ImplementPropertyChanged]
    public class SkillType : BaseType
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public int CoolDown { get; set; }

        public ESkillTarget Target {get; set;}

        public int TargetCount {get; set;}

        public ESkillSelectRule SelectRule{get; set;}

        public ESkillOpportunity Opportunity { get; set; }

        public ESkillSubtype SkillSubtype { get; set; }

        public String Conditions { get; set; }

        public String Functions { get; set; }

        public SkillType ShallowCopy()
        {
            return this.MemberwiseClone() as SkillType;
        }

    }

}
