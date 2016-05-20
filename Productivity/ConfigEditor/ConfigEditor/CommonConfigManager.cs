using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor
{
    public class CommonConfigManager : ConfigManager<CommonConfig>
    {
    }

    public class CommonConfig : BaseConfig
    {

        [Description("渲染器"), NeedRestart]
        public ERenderer Renderer;
    }

    public enum ERenderer
    {
        [Description("Unity3D")]
        Unity3d,
        [Description("Quick-Cocos2d-x")]
        Cocos2d,
    }
}
