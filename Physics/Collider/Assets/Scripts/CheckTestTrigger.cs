using UnityEngine;
using System.Collections;

public class CheckTestTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other)
	{
		Debug.LogWarning("Triggering !");
	}
}
