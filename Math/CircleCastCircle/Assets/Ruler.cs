using UnityEngine;
using System.Collections;

public class Ruler : MonoBehaviour {

	public GameObject A;
	public GameObject B;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		GUILayout.Label("距离: " + Vector3.Distance(A.transform.position, B.transform.position));
	}
}
