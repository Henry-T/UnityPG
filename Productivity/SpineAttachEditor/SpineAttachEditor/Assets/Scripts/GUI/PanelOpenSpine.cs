using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

// 搜索、打开Json面板
public class PanelOpenSpine : MonoBehaviour {

    public string NameFilter = "";

    private List<FileInfo> spineFileList = new List<FileInfo>();

	void Start () {
        reloadSpineList();
	}
	
    void OnGUI() {

        GUI.skin = ToolManager.Instance.Skin;

        GUILayout.Window(0, new Rect(10, 10, Screen.width - 20, Screen.height - 60), (id) => {

            // 菜单栏
            GUILayout.BeginHorizontal();
            var nameFilter = GUILayout.TextField(NameFilter, GUILayout.Width(Screen.width - 40));
            if (nameFilter != NameFilter)
            {
                NameFilter = nameFilter;
                reloadSpineList();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            // TODO Take a note
            List<string> nameList = spineFileList.ConvertAll<string>((fi) => Path.GetFileNameWithoutExtension(fi.Name));

            int sel = GUILayout.SelectionGrid(-1, nameList.ToArray(), 2);
            if(sel>=0 && spineFileList.Count > sel)
            {
                string spineName = nameList[sel];
                string zipFullPath = Path.Combine(Path.Combine(ToolManager.Instance.virtualWorkDir, ToolManager.Instance.SpineFolder), spineName + ".zip");

                // 测试 开始
                ToolManager.Instance.ReloadSpine(spineName, zipFullPath);

                ToolManager.Instance.SceneMode = EScreenMode.EditAttach;
                ToolManager.Instance.PanelOpenSpine.gameObject.SetActive(false);
                ToolManager.Instance.PanelEditAttach.gameObject.SetActive(true);
                // 测试 结束

                //if(ToolManager.Instance.ReadAttachFromExcel(spineName))
                //{
                //    if(ToolManager.Instance.DebugMode)
                //        Debug.Log("zip文件完整路径: " + zipFullPath);

                //    ToolManager.Instance.ReloadSpine(spineName, zipFullPath);

                //    ToolManager.Instance.SceneMode = EScreenMode.EditAttach;
                //    ToolManager.Instance.PanelOpenSpine.gameObject.SetActive(false);
                //    ToolManager.Instance.PanelEditAttach.gameObject.SetActive(true);
                //}
            }
        }, "搜索zip文件", GUI.skin.GetStyle("window"));

        // 状态信息
        GUI.Label(new Rect(0, Screen.height - 25, 200, 25), ToolManager.Instance.InfoStr);
    }

    private void reloadSpineList()
    {
        spineFileList = new List<FileInfo>();

        DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(ToolManager.Instance.virtualWorkDir,ToolManager.Instance.SpineFolder));
        foreach (FileInfo fInfo in dirInfo.GetFiles("*" + NameFilter + "*.zip", SearchOption.TopDirectoryOnly))
        {
            spineFileList.Add(fInfo);
        }
    }
}
