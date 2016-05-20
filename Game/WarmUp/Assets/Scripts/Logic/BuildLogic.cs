using UnityEngine;
using System.Collections;

public class BuildLogic : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		// 数据
		GUILayout.BeginArea(new Rect(0, 0, 100, Screen.height));
		GUILayout.BeginHorizontal();
		GUILayout.Label("名字: ", GUILayout.Width(40));
		GUILayout.Label("宙斯");
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("信仰: ", GUILayout.Width(40));
		GUILayout.Label("100");
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("法力: ", GUILayout.Width(40));
		GUILayout.Label("100");
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("保护: ", GUILayout.Width(40));
		GUILayout.Label("100");
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("混淆: ", GUILayout.Width(40));
		GUILayout.Label("100");
		GUILayout.EndHorizontal();

		GUILayout.EndArea();
	}
}
