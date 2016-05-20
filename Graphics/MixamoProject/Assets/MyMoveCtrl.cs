using UnityEngine;
using System.Collections;

public class MyMoveCtrl : MonoBehaviour {
	Animator a;
	// Use this for initialization
	void Start () {
		a = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		a.SetFloat ("Speed", 10);
	}
}
