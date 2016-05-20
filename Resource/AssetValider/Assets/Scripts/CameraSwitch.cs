using UnityEngine;
using System.Collections;

// =========================================
// WARN 此项需在资源和客户端项目完全同步！
// =========================================
public class CameraSwitch : MonoBehaviour {

    public Transform CamTarget;
    public Transform CamTrans;

    public static bool ShowGizmo;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    void OnDrawGizmos()
    {
        if(ShowGizmo)
        {
            if(CamTarget)
            {
                Gizmos.DrawIcon(CamTarget.position, "CS_Target", false);
                Gizmos.DrawWireCube(CamTarget.position, new Vector3(1,2,1));
            }
            else
            {
                Gizmos.DrawIcon(transform.position, "CS_Target", false);
                Gizmos.DrawWireCube(transform.position, new Vector3(1,2,1));
            }

            if(CamTrans)
                Gizmos.DrawIcon(CamTrans.position, "CS_Camera");
        }
    }
}
