using UnityEngine;
using UnityEditor;
 
public class BuildPackage 
{
   [MenuItem("Build/BuildPackage")]
   static void ExportResource () 
   {
       string path = EditorUtility.SaveFilePanel ("save", "", "resource", "unity3d");
       if (path.Length != 0)
       {
           Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
           BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets,BuildTarget.Android);
           Selection.objects = selection;
       }
     }
}