using UnityEngine;
using System.Collections;
using LuaInterface;

public class LuaObjectGet : MonoBehaviour {

	void Start () {
        Debug.Log("== 测试 ==   C#获取Lua对象引用");
        Lua lua = new Lua();
        lua["gameObject"] = gameObject;
        GameObject backRef = (GameObject)lua["gameObject"];
        Debug.Log(backRef.name);
	}
}
