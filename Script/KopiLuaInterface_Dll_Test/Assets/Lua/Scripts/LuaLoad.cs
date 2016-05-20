using UnityEngine;
using LuaInterface;

/// <summary>
/// ‘§º”‘ÿLuaΩ≈±æ
/// </summary>
public class LuaLoad : MonoBehaviour 
{

    public TextAsset[] Scripts;

    public void Load(Lua lua)
    {
        if(lua == null)
        {
            return;
        }

        foreach(TextAsset code in Scripts)
        {
            lua.DoString(code.text);
        }
    }
}