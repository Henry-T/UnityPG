using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text.RegularExpressions;

public class BackgroundWorker : MonoBehaviour {
	
	void CustomCommand()
	{
		// 解析启动命令参数
		string[] args = System.Environment.GetCommandLineArgs();

		string customArg = "";

		Regex regex = new Regex(@"[\[].*[\]]");

		foreach(string arg in args)
		{
			if(regex.IsMatch(arg))
			{
				customArg = arg.Substring(1, arg.Length -2);
				break;
			}
		}

		if(!string.IsNullOrEmpty(customArg))
		{
			if(customArg == "BuildWeb")
			{
				BuildWebSetPath("Bin/Web");
			}
		}
	}

	void BuildWebSetPath(string tgtPath)
	{
		BuildPipeline.BuildPlayer(new string[]{"Main"}, tgtPath, BuildTarget.WebPlayer, BuildOptions.None);
	}

	void BuildWeb()
	{
		BuildPipeline.BuildPlayer(new string[]{"Main"}, "Bin/Web", BuildTarget.WebPlayer, BuildOptions.None);
	}
}
