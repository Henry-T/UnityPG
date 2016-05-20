using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log(Quaternion.Euler(0, 0, 0));
		Debug.Log(Quaternion.Euler(0, 45,0));
		Debug.Log(Quaternion.Euler(0, 90,0));
		Debug.Log(Quaternion.Euler(0, 135,0));
		Debug.Log(Quaternion.Euler(0, 180,0));
		Debug.Log(Quaternion.Euler(0, 225,0));
		Debug.Log(Quaternion.Euler(0, 270,0));
		Debug.Log(Quaternion.Euler(0, 315,0));
		Debug.Log("1/8");
		Debug.Log(Quaternion.Euler(0, 0 + 22.5f, 0));
		Debug.Log(Quaternion.Euler(0, 45 + 22.5f,0));
		Debug.Log(Quaternion.Euler(0, 90 + 22.5f,0));
		Debug.Log(Quaternion.Euler(0, 135 + 22.5f,0));
		Debug.Log(Quaternion.Euler(0, 180 + 22.5f,0));
		Debug.Log(Quaternion.Euler(0, 225 + 22.5f,0));
		Debug.Log(Quaternion.Euler(0, 270 + 22.5f,0));
		Debug.Log(Quaternion.Euler(0, 315 + 22.5f,0));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
