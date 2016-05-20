using UnityEngine;
using System.Collections;
using UnityEditor;

public class LocalBundleViewer : MonoBehaviour {

	[MenuItem("Assets/加载Bundle")]
	static void LoadLocalBundle()
	{
		if(!EditorApplication.isPlaying)
			return;

		AssetBundle bundle = AssetBundle.CreateFromFile("test.assetbundle");
		Object[] objs = bundle.LoadAll();
		foreach(Object obj in objs)
		{
			GameObject go = obj as GameObject;
			if(go)
			{
				Debug.Log(go.name);
				Instantiate(go);
			}
			//Instantiate(obj);
		}
	}

	[MenuItem("Assets/加载Bundle(验证model_ID)")]
	static void LoadM0001MOdel()
	{
		if(!EditorApplication.isPlaying)
			return;

		AssetBundle bundle = AssetBundle.CreateFromFile("test.assetbundle");
		Object obj = bundle.Load("Assets/Model/m_0010_model.fbx");
		if(obj)
			Instantiate(obj);

		obj = bundle.Load("Assets/m_0010.prefab");
		if(obj)
			Instantiate(obj);

		obj = bundle.Load("m_0010");
		if(obj)
			Instantiate(obj);
	}
}
