using UnityEngine;
using System.Collections;

public class PlayAnim : MonoBehaviour {

	void Start () {
		Animator a = GetComponent<Animator> ();
		a.Play ("attack");
	}

	void Update () {
	
	}
}