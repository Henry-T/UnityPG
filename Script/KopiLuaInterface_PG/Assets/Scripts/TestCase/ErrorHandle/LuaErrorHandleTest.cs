using UnityEngine;
using System.Collections;
using LuaInterface;

public class LuaErrorHandleTest : MonoBehaviour {
    Lua lua;

	void Start () {
	    Debug.Log("========= Error Handle Test ========");
        lua = new Lua();

        string luaScript = @"
                -- UnityEngine
                UnityEngine	= luanet.UnityEngine
                Debug		= UnityEngine.Debug
                GameObject	= UnityEngine.GameObject

                -- 自定义类导入
                luanet.load_assembly('Assembly-CSharp')
                GlobalVar 	= luanet.GlobalVar
                Extentions	= luanet.Extentions

                local Character

                local lbBindPower

                function Test()
                {
                    Debug.Log('Hello');
                }
              ";

        try
        {
            lua.DoString("local x = 0");
            lua.DoString("local y = 1");
            //lua.DoString(luaScript);
        }
        catch(KopiLua.Lua.LuaException e)
        {
            if (e.c.status == KopiLua.Lua.LUA_ERRSYNTAX)
            {
                Debug.Log( "Syntax error." + "\n\n");
            }

            else if (e.c.status == KopiLua.Lua.LUA_ERRRUN)
            {
                Debug.Log( "LUA_ERRRUN" + "\n\n");
            }

            else if (e.c.status == KopiLua.Lua.LUA_ERRMEM)
            {
                Debug.Log( "LUA_ERRMEM" + "\n\n");
            }

            else if (e.c.status == KopiLua.Lua.LUA_ERRERR)
            {
                Debug.Log( "Error in error handling." + "\n\n");
            }

            Debug.Log("HelpLink: " + e.HelpLink);
            Debug.Log("Message: " + e.Message);
            Debug.Log("Source: " + e.Source);
            Debug.Log("StackTrace: " + e.StackTrace);

            //lua.DoString("err = debug.traceback()");
            //string output = lua.GetString("err");
            object[] returnValues = lua.DoString("return debug.traceback()");
            foreach (object o in returnValues)
                Debug.Log(o);
        }

        Debug.Log("========= Error Handle Test End ========");
	}
	
	void Update () {
	
	}
}
