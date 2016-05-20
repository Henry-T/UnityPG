using UnityEngine;
using System.Collections;

public class AnimTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		if(GUILayout.Button("Idle"))
		{
			animation.Play("BG_AttackStandy");
		}
		if(GUILayout.Button("Attack"))
		{
			animation.Play("BG_Attack00");
		}
	}
}
