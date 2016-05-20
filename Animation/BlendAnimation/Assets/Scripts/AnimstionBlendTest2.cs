using UnityEngine;
using System.Collections;

public class AnimstionBlendTest2 : MonoBehaviour {

	public Animation Animation;

	void Start()
	{
		Animation["idle"].layer = 0;
		Animation["attackA"].layer = 1;
		Animation["attackB"].layer = 1;
		Animation["attackC"].layer = 1;
		Animation["attackD"].layer = 1;
	}

	void OnGUI(){
		GUILayout.Label("1 叠化 CrossFade ");
		if(GUILayout.Button("attackA - A") || Input.GetKeyDown(KeyCode.A))
		{
			Animation.CrossFade("attackA", 0.1f);
		}

		if(GUILayout.Button("attackB - S") || Input.GetKeyDown(KeyCode.S))
		{
			Animation.CrossFade("attackB", 0.1f );
		}

		if(GUILayout.Button("attackC - D") || Input.GetKeyDown(KeyCode.D))
		{
			Animation.CrossFade("attackC", 0.1f );
		}

		if(GUILayout.Button("attackD - F") || Input.GetKeyDown(KeyCode.F))
		{
			Animation.CrossFade("attackD", 0.1f );
		}

		GUILayout.Label("2 截断 Play");
		if(GUILayout.Button("attackA - J") || Input.GetKeyDown(KeyCode.J))
		{
			Animation.Play("attackA" );
		}
		
		if(GUILayout.Button("attackB - K") || Input.GetKeyDown(KeyCode.K))
		{
			Animation.Play("attackB");
		}
		
		if(GUILayout.Button("attackC - L") || Input.GetKeyDown(KeyCode.L))
		{
			Animation.Play("attackC");
		}
		
		if(GUILayout.Button("attackD - ;") || Input.GetKeyDown(KeyCode.Semicolon))
		{
			Animation.Play("attackD");
		}

		
		GUILayout.Label("3 同时 Blend");
		if(GUILayout.Button("attackA - Z") || Input.GetKeyDown(KeyCode.Z))
		{
			Animation.Blend("attackA", 0.5f, 0.08f);
		}
		
		if(GUILayout.Button("attackB - X") || Input.GetKeyDown(KeyCode.X))
		{
			Animation.Blend("attackB", 0.5f, 0.08f );
		}
		
		if(GUILayout.Button("attackC - C") || Input.GetKeyDown(KeyCode.C))
		{
			Animation.Blend("attackC", 0.5f, 0.08f );
		}
		
		if(GUILayout.Button("attackD - V") || Input.GetKeyDown(KeyCode.V))
		{
			Animation.Blend("attackD", 0.5f, 0.08f );
		}
	}
}
