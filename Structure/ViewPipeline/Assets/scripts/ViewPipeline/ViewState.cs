using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ViewState
{
    private Dictionary<string, string> data;

    public void Refresh(Dictionary<string, string> parameters)
    {
    }

    public virtual Dictionary<string, string> SaveState()
    {
        var persist = new Dictionary<string, string>();
        foreach(var pair in data)
        {
            persist.Add(pair.Key, pair.Value);
        }
        return persist;
    }

    public void LoadState(Dictionary<string, string> persist)
    {
        data = persist;
        Inspector.Instance.SetTarget(data);
    }
}
