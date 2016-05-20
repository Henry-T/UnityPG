using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor
{
    public class UserConfigManager : ConfigManager<UserConfig>
    {
    }

    public class UserConfig : BaseConfig
    {

        [Description("配置目录"), NeedRestart]
        public string ExcelDir;

        [Description("资源目录"), NeedRestart]
        public string ResDir;

        [Description("启动时恢复上次退出前的编辑器")]
        public bool RestoreLastEditor;
    }
}
