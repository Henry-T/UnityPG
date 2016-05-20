//
//  AssetBundleContent.cs
//
//  Created by Niklas Borglund
//  Copyright (c) 2012 Cry Wolf Studios. All rights reserved.
//  crywolfstudios.net
//
// Editor window that specifies the files in the bundle according to
// the bundle contents file generated when building the asset bundles.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

public class AssetBundleContent : EditorWindow
{
    private List<string> currentContentList;
    private string currentBundleName;
    private string exportPath;
    private float sizeInKiloBytes = 0;

    public void SelectAssetBundle(string currentBundleName, List<string> currentContentList, string exportPath, float sizeInKiloBytes)
    {
        this.currentContentList = currentContentList;
        this.currentBundleName = currentBundleName;
        this.exportPath = exportPath;
        this.sizeInKiloBytes = sizeInKiloBytes;
    }

    void OnGUI()
    {
        if (currentBundleName != null && currentContentList != null)
        {
            GUILayout.Label("Bundle: " + currentBundleName, EditorStyles.boldLabel);
            GUILayout.Label("File Size: " + sizeInKiloBytes + "kb", EditorStyles.boldLabel);

            foreach (string assetName in currentContentList)
            {
                GUILayout.Label(assetName, EditorStyles.label);
            }
			
			if (Application.platform != RuntimePlatform.OSXEditor)
			{
            	if (GUILayout.Button("Show in explorer"))
            	{
               	 string itemPath = exportPath + currentBundleName;
               	 itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like forward slashes
               	 System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
            	}
			}
        }
    }

    //Get the window
    public static AssetBundleContent CreateContentWindow()
    {
        AssetBundleContent thisWindow = (AssetBundleContent)EditorWindow.GetWindow<AssetBundleContent>("Bundle Contents",true,typeof(AssetBundleWindow));
        return thisWindow;
    }
}
