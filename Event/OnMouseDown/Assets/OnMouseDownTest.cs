using UnityEngine;
using System.Collections;

public class OnMouseDownTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator OnMouseDown() {
		yield return new WaitForSeconds(2);
		Debug.Log("mouse down!");
	}
}
