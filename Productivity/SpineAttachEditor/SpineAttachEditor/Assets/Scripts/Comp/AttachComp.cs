using UnityEngine;
using System.Collections;

// 挂点标记物
public class AttachComp : MonoBehaviour {

    public string AttachName = "";

	void Start () {
	    
	}
	
	void Update () {
	}

    void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Gizmos.DrawRay(ray);
    }
}
