using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ViewPipeline : MonoBehaviour {

    public static ViewPipeline Instance
    {
        get {
            return instance;
        }
    }
    private static ViewPipeline instance;

    public List<ViewStateInfo> stateInfoStack;

    private ViewPipelineDebugger debugger;

	// Use this for initialization
	void Start () {
        instance = this;
        stateInfoStack = new List<ViewStateInfo>();
	}
	
    public void PushState(Type stateType)
    {
        this.forward(stateType, ViewStateInfo.EMode.Stack);
    }

    public void ReplaceState(Type stateType)
    {
        this.forward(stateType, ViewStateInfo.EMode.Override);
    }

    public void AddState(Type stateType)
    {
        this.forward(stateType, ViewStateInfo.EMode.Layer);
    }

    private void forward(Type stateType, ViewStateInfo.EMode mode)
    {
        var stateInfo = new ViewStateInfo(stateType, mode);
        if (mode == ViewStateInfo.EMode.Stack)
        {
            if (stateInfoStack.Count > 0)
            {
                stateInfoStack[stateInfoStack.Count-1].DestroyInstance();
            }
            stateInfoStack.Add(stateInfo);
        }
        else if (mode == ViewStateInfo.EMode.Layer)
        {
            stateInfoStack.Add(stateInfo);
        }
        else if (mode == ViewStateInfo.EMode.Override)
        {
            var replaceMode = ViewStateInfo.EMode.Layer;
            if (stateInfoStack.Count > 0 )
            {
                replaceMode = stateInfoStack[stateInfoStack.Count - 1].Mode;
                stateInfoStack[stateInfoStack.Count - 1].DestroyInstance();
                stateInfoStack.RemoveAt(stateInfoStack.Count - 1);
            }
            stateInfo.Mode = replaceMode;
            stateInfoStack.Add(stateInfo);
        }

        ViewPipelineDebugger.Instance.Refresh();
    }

    public void Back()
    {
        if (stateInfoStack.Count == 0)
            return;

        stateInfoStack[stateInfoStack.Count - 1].DestroyInstance();
        stateInfoStack.RemoveAt(stateInfoStack.Count - 1);
        if (stateInfoStack.Count > 0)
        {
            if (stateInfoStack[stateInfoStack.Count - 1].Instance != null)
                stateInfoStack[stateInfoStack.Count - 1].RefreshInstance();
            else
                stateInfoStack[stateInfoStack.Count - 1].CreateInstance(true);
        }

        ViewPipelineDebugger.Instance.Refresh();
    }

    public void Clear()
    {
        Destroy();
        stateInfoStack = new List<ViewStateInfo>();

        ViewPipelineDebugger.Instance.Refresh();
    }

    public void Destroy()
    {
        for(int i=0; i<stateInfoStack.Count; i++)
        {
            stateInfoStack[i].DestroyInstance();
        }
        
        ViewPipelineDebugger.Instance.Refresh();
    }

    public void Restore()
    {
        for (int i = 0; i < stateInfoStack.Count; i++)
        {
            if (i == stateInfoStack.Count - 1)
                stateInfoStack[i].CreateInstance();
            else if (stateInfoStack[i + 1].Mode == ViewStateInfo.EMode.Layer)
                stateInfoStack[i].CreateInstance();
        }

        ViewPipelineDebugger.Instance.Refresh();
    }
}
