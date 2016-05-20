using UnityEngine;
using System.Collections;

public class AnimstionBlendTest : MonoBehaviour {

	public Animation Animation;

	void Start()
	{
		//Animation["hit"].layer = 0;
		//Animation["attack"].layer = 1;
	}

	void OnGUI(){
		if(GUILayout.Button("Hit"))
		{
			Animation.Play("hit", PlayMode.StopAll);
		}

		if(GUILayout.Button("HitBack"))
		{
			Animation.Play("attack", PlayMode.StopAll);
		}

		if(GUILayout.Button("Hit_Blend"))
		{
			Animation.Blend("hit",1f);
		}

		if(GUILayout.Button("HitBack_Blend"))
		{
			Animation.Blend("attack", 1f);
		}
	}
}
