using UnityEngine;
using System.Collections;
using System;

public class ViewStateInfo
{

    public enum EMode
    {
        Stack,          // 堆叠
        Override,       // 覆盖
        Layer,          // 层叠
    }

    public EMode Mode = EMode.Stack;
    public Type Type;

    public ViewState Instance;

    public ViewStateInfo(Type stateType, EMode mode)
    {
        Type = stateType;
        Mode = mode;
        Instance = null;
        CreateInstance();
    }

    public void CreateInstance(bool loadState=false)
    {
        Instance = Type.GetConstructor(Type.EmptyTypes).Invoke(new System.Object[0]) as ViewState;
    }

    public void DestroyInstance()
    {
        Instance = null;
    }

    public void RefreshInstance()
    {
        if (Instance != null)
            Instance.Refresh(null);
    }
}
