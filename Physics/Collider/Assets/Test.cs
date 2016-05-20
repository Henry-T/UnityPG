using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    public GameObject Capsule;

	void Start () {
        Debug.Log("Bound: " + Capsule.collider.bounds.size);
	}
	
	void Update () {
	
	}
}
