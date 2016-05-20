using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using UnityEditor.Callbacks;

public class CustomBuild : MonoBehaviour {

    [PostProcessBuild]
    public static void OnBuildDone(BuildTarget target, string pathToBuiltProject)
    {
        Debug.Log("执行生成后事件");
        if (target == BuildTarget.StandaloneWindows)
        {
            // TODO thy 改配置路径
            DirectoryInfo deployDirInfo = new DirectoryInfo(@"E:\proj\UnityPG\Productivity\ConfigEditor\Bin\SpineRenderer");
            if (deployDirInfo.Exists)
            {
                deployDirInfo.Delete(true);
            }

            FileInfo exeInfo = new FileInfo(pathToBuiltProject);

            // TODO thy 改配置路径
            DirectoryInfo buildDirInfo = exeInfo.Directory;

            
            buildDirInfo.MoveTo(deployDirInfo.FullName);

            buildDirInfo.Create();  // 方便下次发布

            // 拷贝I18N库
            DirectoryInfo i18nFixDirInfo = new DirectoryInfo("I18N_fix");
            DirectoryInfo i18nDstDirInfo = new DirectoryInfo(@"E:\proj\UnityPG\Productivity\ConfigEditor\Bin\SpineRenderer\SpineRenderer_Data\Managed\");
            foreach(FileInfo fi in i18nFixDirInfo.GetFiles())
            {
                fi.CopyTo(Path.Combine(i18nDstDirInfo.FullName, fi.Name), true);
            }
        }
    }

    [MenuItem("Build/x86")]
    public static void Build()
    {
        BuildPipeline.BuildPlayer(new string[] { "Assets/Main.unity" }, "Bin/SpineRenderer.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
    }
}
