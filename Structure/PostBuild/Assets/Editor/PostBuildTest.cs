using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Diagnostics;

public class PostBuildTest : MonoBehaviour {
	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
		if(target == BuildTarget.Android && File.Exists("PostBuild_Android.bat"))
		{
			Process proc = new Process();
			proc.StartInfo.FileName = "PostBuild_Android.bat";
      		proc.StartInfo.Arguments = pathToBuiltProject.Replace('/', '\\');
			proc.Start();
		}
	}
}
