using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	GameObject A;
	GameObject B;

	// Use this for initialization
	void Start () {
		A = GameObject.Find("A");
		B = GameObject.Find("B");
	}
	
	// Update is called once per frame
	void Update () {
		if(A.collider.bounds.Intersects(B.collider.bounds))
		{
			Debug.Log("Intersecting 内部也是");
		}
	}
}
