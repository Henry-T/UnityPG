using UnityEngine;
using System.Collections;

public class PanelMain : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnGUI()
    {
        if (ToolManager.Instance.SpineCurrentToPreload > 0)
        {
            GUILayout.Label("资源预加载中: " + ToolManager.Instance.SpineCurrentToPreload + "/" + ToolManager.Instance.SpineTotalToPreload);
        }
    }
}
