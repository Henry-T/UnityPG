using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;

public class BackgroundBuild : MonoBehaviour {
	[MenuItem("Build/BuildWindows86")]
	static void PerformBuild ()
	{
		string[] scenes = { "Assets/Main.unity" };
		BuildPipeline.BuildPlayer(scenes, "Bin/Windows/TestBuild/game.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
	}

	[MenuItem("Build/BuildAndroid")]
	static void PerformBuildAndroid()
	{
		string[] scenes = {"Assets/Main.unity"};
		DateTime now = DateTime.Now;
		string path = "Bin/Android/game_" + now.ToString ("yyyy-MM-dd_HH-mm-ss") + ".apk";
		FileInfo apkInfo = new FileInfo (path);
		if (!apkInfo.Directory.Exists)
            Directory.CreateDirectory (apkInfo.Directory.FullName);

		BuildPipeline.BuildPlayer (scenes, path, BuildTarget.Android, BuildOptions.None);
	}
	
	[MenuItem("Build/BuildIOS")]
	static void PerformBuildIOS()
	{
		string[] scenes = {"Assets/Main.unity"};
		DateTime now = DateTime.Now;

		Debug.LogWarning ("BackgroundBuild::BuildIOS Windows下无法发布IOS版本");
		//BuildPipeline.BuildPlayer (scenes, "Bin/IOS/game_" + now.ToString() + ".apk", BuildTarget.Android, BuildOptions.None);
	}

	[MenuItem("Build/BuildWeb")]
	static void PerformBuildWeb()
	{
		string[] scenes = {"Assets/Main.unity"};
		DateTime now = DateTime.Now;
		string path = "Bin/Web/" + now.ToString("yyyy-MM-dd_HH-mm-ss");
		FileInfo apkInfo = new FileInfo (path);
		if (!apkInfo.Directory.Exists)
			Directory.CreateDirectory (apkInfo.Directory.FullName);
		
		BuildPipeline.BuildPlayer (scenes, path, BuildTarget.WebPlayer, BuildOptions.None);
	}
}
