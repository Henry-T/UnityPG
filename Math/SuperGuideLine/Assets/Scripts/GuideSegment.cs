using UnityEngine;
using System.Collections;

public class GuideSegment : MonoBehaviour {

	public GameObject Obj1;
	public GameObject Obj2;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnDrawGizmos()
	{
		if(Obj1 && Obj2)
		{
			Gizmos.DrawLine(Obj1.transform.position, Obj2.transform.position);
			//Gizmos.
		}
	}
}
