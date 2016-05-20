using UnityEngine;
using System.Collections;

public class UIFightPanel : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width-100, 0, 100, Screen.height));
		GUILayout.Button(new GUIContent("模拟胜利"));
		GUILayout.Button(new GUIContent("模拟失败"));
		GUILayout.EndArea();
	}
}
