using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConfigEditor
{
    [ImplementPropertyChanged]
    public class LevelBgType : BaseType
    {

        public string Name { get; set;}

        public string Description { get; set;}

        public string FG { get; set; }

        public string MG { get; set; }

        public string BG { get; set; }
    }
}
