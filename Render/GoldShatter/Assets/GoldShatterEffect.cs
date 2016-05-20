using UnityEngine;
using System.Collections;

public class GoldShatterEffect : MonoBehaviour {

	void Start () {
	
	}

	void Update () {
	
	}

	void OnPreRender() {
		GL.wireframe = true;
	}
	void OnPostRender() {
		//GL.wireframe = false;
	}

}
