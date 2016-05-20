using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigType
{
    public enum EProtocol
    {
        PreloadAllSpine = 1,           // 预读全部 Spine
        LoadSpine = 100, 
        SetAttach,
        PlayAnim,
    }
}
