using UnityEngine;
using System.Collections;
using UnityEditor;

public class TestWindow : EditorWindow {

	public string PublicStr = "This is public string";
	public static string StaticStr = "This is static string";

	public static int StaticInt = 0;
	public int MemberInt = 0;

	[MenuItem("TestWindow/Open")]
	static void CreateWindow()
	{
		TestWindow window = EditorWindow.GetWindow<TestWindow>();
	}

	void OnGUI()
	{
		if(GUILayout.Button ("静态计数器递增"))
		{
			StaticInt++;
			Debug.Log(StaticInt);
		}

		if(GUILayout.Button ("成员计数器递增"))
		{
			MemberInt++;
			Debug.Log(MemberInt);
		}
	}
}
