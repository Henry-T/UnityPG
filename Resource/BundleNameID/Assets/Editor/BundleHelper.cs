using UnityEngine;
using System.Collections;
using UnityEditor;

public class BundleHelper : MonoBehaviour {

	[MenuItem("Assets/打包Bundle")]
	static void BundleSelection()
	{
		BuildPipeline.BuildAssetBundle(Selection.activeGameObject, Selection.gameObjects, "test.assetbundle", 
		                               BuildAssetBundleOptions.CollectDependencies|BuildAssetBundleOptions.UncompressedAssetBundle|BuildAssetBundleOptions.CompleteAssets,
		                               EditorUserBuildSettings.activeBuildTarget);

	}

	[MenuItem("Assets/打包Bundle(无依赖)")]
	static void BundleSelection_NoDep()
	{
		BuildPipeline.BuildAssetBundle(Selection.activeGameObject, Selection.gameObjects, "test.assetbundle",
		                               BuildAssetBundleOptions.UncompressedAssetBundle,
		                               EditorUserBuildSettings.activeBuildTarget);
	}

	[MenuItem("Assets/打包Bundle(相对路径)")]
	static void BundleSelection_Relative()
	{
		string[] paths = new string[Selection.objects.Length];
		for( int i=0; i < Selection.objects.Length; i++)
		{
			paths[i] = AssetDatabase.GetAssetPath(Selection.objects[i]);
			Debug.Log(paths[i]);
		}

		BuildPipeline.BuildAssetBundleExplicitAssetNames(Selection.objects, paths, "test.assetbundle", 
		                                                 BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets| BuildAssetBundleOptions.UncompressedAssetBundle, 
		                                                 EditorUserBuildSettings.activeBuildTarget);

	}
}
