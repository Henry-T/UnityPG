//
//  AssetBundleWindow.cs
//
//  Created by Niklas Borglund
//  Copyright (c) 2012 Cry Wolf Studios. All rights reserved.
//  crywolfstudios.net
//
// Creates a button that deletes all AssetBundle content 
// that has been cached by the current application.

using UnityEngine;
using UnityEditor;

public class ClearCache
{
    [MenuItem("Assets/Bundle Creator/Clear Cache")]
    static void ClearGameCache()
    {
        Caching.CleanCache();
    }
}
