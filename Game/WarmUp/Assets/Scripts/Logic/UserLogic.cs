using UnityEngine;
using System.Collections;

public class UserLogic : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width/2 - 50, 0, 100, 30));
		GUILayout.BeginHorizontal();
		GUILayout.Label("账号: ");
		GUILayout.Label(GameManager.Instance.CurUserName);
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}
