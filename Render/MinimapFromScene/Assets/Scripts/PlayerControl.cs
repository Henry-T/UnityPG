using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float length = 500 * Time.deltaTime;
		if (Input.GetKey (KeyCode.A))
			transform.Translate (-length, 0, 0);
		else if(Input.GetKey(KeyCode.D))
			transform.Translate ( length, 0, 0);
		else if(Input.GetKey(KeyCode.S))
			transform.Translate ( 0, 0,-length);
		else if(Input.GetKey(KeyCode.W))
			transform.Translate ( 0, 0, length);
	}
}
