using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor
{
    [ImplementPropertyChanged]
    public class LevelNPCType : BaseType
{
        public string Name { get; set; }

        public long Level { get; set; }

        public int Wave
        {
            get;
            set;
        }

        public long Type
        {
            get;
            set;
        }

        public int Position { get; set;}

        public int Attack
        {
            get;
            set;
        }

        public int HP
        {
            get;
            set;
        }

        public int Defence { get; set;}

        public int Speed {get; set;}

        public int ComboRate
        {
            get;
            set;
        }

        public int BlockRate
        {
            get;
            set;
        }

        public int CounterRate
        {
            get;
            set;
        }

        public int CritRate
        {
            get;
            set;
        }

        public int CritDamage { get; set; }

        public int ShowLevel { get; set; }

    }
}
