using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	public GameObject A;
	public GameObject B;
	public GameObject AEnd;
	public GameObject BEnd;

	// Use this for initialization
	void Start () {
		A = GameObject.Find("A");
		B = GameObject.Find("B");
		AEnd = GameObject.Find("AEnd");
		BEnd = GameObject.Find("BEnd");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 vecA = AEnd.transform.position - A.transform.position;
		Vector3 vecB = BEnd.transform.position - B.transform.position;

		Debug.LogWarning("Vector3.Angle(vecA, vecB) : " + Vector3.Angle(vecA, vecB));

		Quaternion q = Quaternion.FromToRotation(vecA, vecB);
		Debug.LogWarning("FromToRotation.Euler : " + q.eulerAngles.y);
	}
}
