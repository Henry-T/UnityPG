using UnityEngine;
using LuaInterface;
using System.Collections;

public class LuaFloatGet : MonoBehaviour {

    void Start()
    {
        Debug.Log("== 测试 ==   C#获取Lua Float");
        Lua lua = new Lua();
        lua.DoString("num1 = -0.9999999");
        Debug.Log(lua["num1"]);
	}
}
