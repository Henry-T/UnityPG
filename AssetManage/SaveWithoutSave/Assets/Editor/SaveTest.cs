using UnityEngine;
using System.Collections;
using UnityEditor;

public class SaveTest  {

	[MenuItem("Tools/保存命令")]
	static void Save()
	{
		EditorApplication.SaveAssets();
	}
}
