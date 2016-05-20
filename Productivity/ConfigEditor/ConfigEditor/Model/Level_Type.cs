using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConfigEditor
{
    [ImplementPropertyChanged]
    public class Level_Type : BaseType
    {
        public string Name { get; set;}

        public string Description { get; set;}

        public int SceneType { get; set;}

        public long Formation { get; set;}

        public long LastLevel { get; set;}

        public long NextLevel { get; set;}

        public int Ep { get; set;}

        public int Coin { get; set;}

        public int Exp { get; set;}

        public int SysLevel { get; set; }

        public ELevelType LevelType { get; set;}

        public int IsBoss { get; set;}

        public int MaxTimes { get; set;}

        public string ShowAward { get; set; }

        public string Awards { get; set; }

        public string ExtraAwards { get; set; }

        public int BattleBackground { get; set; }

        //public RandomGroup Awards
        //{
        //    get;
        //    set;
        //}

        //public RandomGroup ExtraAwards
        //{
        //    get;
        //    set;
        //}
    }
}
