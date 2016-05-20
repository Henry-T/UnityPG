using UnityEngine;
using System.Collections;

public class TestClass : MonoBehaviour
{
	public string username = "username";
	public string pwd = "a pwd";
	void OnGUI() {
		GUI.SetNextControlName("MyTextField");
		username = GUI.TextField(new Rect(10, 10, 100, 20), username);
		pwd = GUI.TextField(new Rect(10, 40, 100, 20), pwd);
		if (GUI.Button(new Rect(10, 70, 80, 20), "Move Focus"))
			GUI.FocusControl("MyTextField");
		
	}

//		if (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint) {
//		}
//		else {
//			Debug.Log("Current detected event: " + Event.current);
//			Debug.Log("Current detected event: " + Event.current.keyCode);
//			Debug.Log("Current detected event: " + Event.current.character);
//			Debug.Log("----------------------------------------------------------------------------");
//		}
}

