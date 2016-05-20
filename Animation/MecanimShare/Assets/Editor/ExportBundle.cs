using UnityEngine;
using UnityEditor;


public class ExportBundle : MonoBehaviour {
	[MenuItem ("Assets/BuildAssetBundle")]
	static void DoExport() {
		string str = EditorUtility.SaveFilePanel("Save Bundle...", Application.dataPath, Selection.activeObject.name, "assetbundle");
		if (str.Length != 0) {
			BuildPipeline.BuildAssetBundle(Selection.activeObject, Selection.objects, str, 
			                               BuildAssetBundleOptions.CompleteAssets|BuildAssetBundleOptions.CollectDependencies, BuildTarget.iPhone);
		}
	}

    [MenuItem("Assets/BuildAssetBundleToSingle")]
    static void DoExportSingle()
    {
        string str = EditorUtility.SaveFolderPanel("Save Bundle...", Application.dataPath,"Test");
        if (str.Length != 0)
        {
            foreach (var o in Selection.objects)
            {
                var t = new Object[1];
                string path = str +"/"+ o.name + ".assetbundle";
                t[0] = o;
                print(path);
                BuildPipeline.BuildAssetBundle(Selection.activeObject, t, path,
                                               BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies, BuildTarget.iPhone);
            }
        }
    }

    [MenuItem("Assets/BuildAssetBundleForWebPlayer")]
    static void DoExportWebPlayer()
    {
        string str = EditorUtility.SaveFilePanel("Save Bundle...", Application.dataPath, Selection.activeObject.name, "assetbundle");
        if (str.Length != 0)
        {
            BuildPipeline.BuildAssetBundle(Selection.activeObject, Selection.objects, str,
                                           BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies, BuildTarget.WebPlayerStreamed);
        }
    }
}
