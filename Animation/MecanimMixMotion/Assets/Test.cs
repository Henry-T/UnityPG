using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	public Animator Animator;

	void Start () {
	
	}
	

	void Update () {
	
	}

	void OnGUI()
	{
		if(GUI.Button(new Rect(0,0, 100, 40), new GUIContent("Dive!")))
		{
			Animator.Play("Base Layer.Dive", 0);
		}
	}
}
