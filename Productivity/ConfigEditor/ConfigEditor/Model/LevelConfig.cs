using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Reflection;
using NPOI.HPSF;

namespace ConfigEditor
{
    public class LevelConfig : BaseConfig<Level_Type>
    {
        public LevelConfig(String name, int startID, int endID) : base(name, startID, endID) { }
    }
}
