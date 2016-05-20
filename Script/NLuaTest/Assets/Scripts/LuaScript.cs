using UnityEngine;
using NLua;
using System.Collections;

public class LuaScript : MonoBehaviour {
    public Lua lua;
    public TextAsset CommonLuaScript;
    public TextAsset BindLuaScript;

	void Start () {
        lua = new Lua();
        // 加载定义脚本
        lua.DoString(CommonLuaScript.text);
        lua.DoString(BindLuaScript.text);

        lua["gameObject"] = gameObject;
        lua["transform"] = transform;

        LuaFunction luaFunc = lua.GetFunction("test");
        luaFunc.Call();

	}
	
	void Update () {
        LuaFunction updateFunc = lua.GetFunction("Update");
        updateFunc.Call();
	}

    void OnGUI()
    {
        LuaFunction onGuiFunc = lua.GetFunction("OnGUI");
        onGuiFunc.Call();
    }
}
