using UnityEngine; 
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(FuncCaller))]
public class FuncCallerEditor : Editor {

	[HideInInspector]
	public int CompIndex = -1;
	[HideInInspector]
	public int FuncIndex = -1;

	private Component[] comps;
	private MethodInfo[] methods;

	public override void OnInspectorGUI()
	{
		FuncCaller Target = (FuncCaller)target;
		comps = Target.gameObject.GetComponents<Component>();

		List<GUIContent> guiContents = new List<GUIContent>();
		foreach(Component comp in comps)
		{
			guiContents.Add(new GUIContent(comp.ToString()));
		}

		int newCompIndex = EditorGUILayout.Popup(CompIndex, guiContents.ToArray());

		if(newCompIndex != CompIndex)
		{
			CompIndex = newCompIndex;
			Component comp = comps[CompIndex];
			methods = comp.GetType().GetMethods();
				//System.Reflection.BindingFlags.DeclaredOnly |
				//System.Reflection.BindingFlags.Public);
		}

		if(methods != null)
		{
			List<GUIContent> guiContents2 = new List<GUIContent>();
			foreach(MethodInfo method in methods)
			{
				guiContents2.Add(new GUIContent(method.Name));
			}

			FuncIndex = EditorGUILayout.Popup(FuncIndex, guiContents2.ToArray());

			if(FuncIndex != -1)
			{
				if(GUILayout.Button("调用"))
				{
					methods[FuncIndex].Invoke(comps[CompIndex], null);
				}
			}
		}
	}
}
