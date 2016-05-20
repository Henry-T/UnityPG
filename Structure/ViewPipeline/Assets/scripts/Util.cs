using UnityEngine;
using System.Collections;

public class Util : MonoBehaviour {
    public void PushA()
    {
        ViewPipeline.Instance.PushState(typeof(AState));
    }

    public void PushB()
    {
        ViewPipeline.Instance.PushState(typeof(BState));
    }

    public void PushX()
    {
        ViewPipeline.Instance.PushState(typeof(XState));
    }


    public void ReplaceA()
    {
        ViewPipeline.Instance.ReplaceState(typeof(AState));
    }

    public void ReplaceB()
    {
        ViewPipeline.Instance.ReplaceState(typeof(BState));
    }

    public void ReplaceX()
    {
        ViewPipeline.Instance.ReplaceState(typeof(XState));
    }

    public void LayerA()
    {
        ViewPipeline.Instance.AddState(typeof(AState));
    }

    public void LayerB()
    {
        ViewPipeline.Instance.AddState(typeof(BState));
    }

    public void LayerX()
    {
        ViewPipeline.Instance.AddState(typeof(XState));
    }

    public void Back()
    {
        ViewPipeline.Instance.Back();
    }

    public void Clear()
    {
        ViewPipeline.Instance.Clear();
    }

    public void Destroy()
    {
        ViewPipeline.Instance.Destroy();
    }

    public void Restore()
    {
        ViewPipeline.Instance.Restore();
    }
}
