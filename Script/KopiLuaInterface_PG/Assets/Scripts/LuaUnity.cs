using UnityEngine;
using LuaInterface;
using System.Collections.Generic;

public class LuaUnity : MonoBehaviour
{
    public LuaLoad PreLoad;
    public TextAsset[] Scripts;

    protected Lua _lua;
    protected Dictionary<string, LuaFunction> Functions = new Dictionary<string, LuaFunction>();

    public void Initialize()
    {
        _lua = new Lua();

        if (PreLoad != null)
        {
            PreLoad.Load(_lua);
        }

        SetData();

        foreach (TextAsset code in Scripts)
        {
            _lua.DoString(code.text);
        }
    }

    public virtual void SetData()
    {
        _lua["gameObject"] = gameObject;
        _lua["transform"] = transform;
    }

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

    // ¶Ô½ÓUnityAPI
    void Awake()
    {
        Initialize();

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
}