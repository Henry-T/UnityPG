using UnityEngine;
using System.Collections.Generic;

public class Inspector : MonoBehaviour {
    public static Inspector Instance { get { return instance; } }
    private static Inspector instance;

    private Dictionary<string, string> target;

    public Transform Root;

	// Use this for initialization
	void Start () {
        instance = this;
	}
	
    public void SetTarget(Dictionary<string, string> target)
    { 
        this.target = target;
        this.Refresh();
    }

    public void Refresh()
    {
        foreach (Transform child in Root)
        {
            GameObject.Destroy(child.gameObject);
        }

        int index = 0;
        int entryHeight = 30;

        foreach(var kv in this.target)
        {
            var entry = createPropertyEntry(kv.Key, kv.Value);
            entry.SetParent(Root);
            entry.Position(0, index * -entryHeight);
            entry.onDelete = ()=>{
                this.Refresh();
            };
            index = index + 1;
        }

        var holder = createPropertyHolder();
        holder.SetParent(Root);
        holder.Position(0, index * -entryHeight);
        holder.onAdd = ()=>{
            this.Refresh();
        };
    }

    private void changePropName(string oldName, string newName)
    {
        if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
        {
            target[newName] = target[oldName];
            //target
            //target.remove...
        }
    }

    private PropertyEntry createPropertyEntry(string name, string value)
    {
        return new PropertyEntry(target, name, value);
    }

    private PropertyHolder createPropertyHolder()
    {
        return new PropertyHolder(target);
    }

}
