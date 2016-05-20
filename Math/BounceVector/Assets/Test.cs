using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	public GameObject VectorStart;
	public GameObject VectorEnd;
	public GameObject VectorBounce;

	// Use this for initialization
	void Start () {
		VectorStart	 	= GameObject.Find("VectorStart");
		VectorEnd	 	= GameObject.Find("VectorEnd");
		VectorBounce 	= GameObject.Find("VectorBounce");
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = new Ray(VectorStart.transform.position, VectorStart.transform.forward);
		RaycastHit hitInfo;
		if(Physics.Raycast(ray, out hitInfo, 100, LayerMask.GetMask(new string[]{"Wall"})))
		{
			VectorEnd.transform.position = hitInfo.point;

			Vector3 bVector = Math.BounceVector(VectorEnd.transform.position - VectorStart.transform.position, hitInfo.normal);
			VectorBounce.transform.position = VectorEnd.transform.position + bVector;
		}
	}

	void OnDrawGizmos()
	{
		if(VectorStart && VectorEnd && VectorBounce)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(VectorStart.transform.position, VectorEnd.transform.position);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(VectorEnd.transform.position, VectorBounce.transform.position);
		}
	}
}