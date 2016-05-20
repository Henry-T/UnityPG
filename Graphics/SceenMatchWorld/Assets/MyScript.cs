using UnityEngine;
using System.Collections;

public class MyScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Quaternion q = Quaternion.AngleAxis(120, Vector3.up);
        Debug.LogWarning(q.x + "-"+ q.y +"-"+ q.z + "-" + q.w);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
