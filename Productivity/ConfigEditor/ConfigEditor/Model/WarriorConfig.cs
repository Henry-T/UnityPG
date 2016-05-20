using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor
{
    public class WarriorConfig : XlsData
    {
        public static int NatureStartID = 19980;

        public WarriorConfig(String name, String keyName)
            : base(name, keyName)
        {

        }

        public override void Load()
        {
            base.Load();

            // 把品格数据从DataTable里抽掉
            // DataTable.DefaultView.RowFilter = "ID < 19980";
        }
    }
}
