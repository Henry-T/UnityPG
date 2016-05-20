using UnityEngine;
using LuaInterface;
using System.Collections.Generic;

public class LuaUnity : MonoBehaviour
{
    public LuaLoad PreLoad;

    public TextAsset[] Scripts;

    #region RUN LUA

    Lua _lua;

    /// <summary>
    /// 缓存调用过的Methond
    /// </summary>
    Dictionary<string, LuaFunction> Functions = new Dictionary<string, LuaFunction>();

    void Run()
    {
        _lua = new Lua();

        if(PreLoad != null)
        {
            PreLoad.Load(_lua);
        }

        SetData(_lua);

        foreach(TextAsset code in Scripts)
        {
            _lua.DoString(code.text);
        }
    }

    void SetData(Lua lua)
    {
        lua["gameObject"] = gameObject;
        lua["transform"] = transform;
    }

    #endregion

    /// <summary>
    /// 执行一个LUA方法
    /// </summary>
    public object[] CallMethond(string func)
    {
        if (!Functions.ContainsKey(func))
        {
            Functions[func] = _lua.GetFunction(func);
        }

        if (Functions[func] != null)
        {
            return Functions[func].Call();
        }
        return null;
    }

    #region Call Methond
    void Awake()
    {
        Run();

        CallMethond("Awake");
    }

	void Start()
	{
        CallMethond("Start");
	}

    void Update()
    {
        CallMethond("Update");
    }

    void LateUpdate()
    {
        CallMethond("LateUpdate");
    }

    void FixedUpdate()
    {
        CallMethond("FixedUpdate");
    }

    void OnEnable()
    {
        CallMethond("OnEnable");
    }

    void OnDisable()
    {
        CallMethond("OnDisable");
    }

    void OnDestroy()
    {
        CallMethond("OnDestroy");
    }

    void OnGUI()
    {
        CallMethond("OnGUI");
    }
#endregion
}