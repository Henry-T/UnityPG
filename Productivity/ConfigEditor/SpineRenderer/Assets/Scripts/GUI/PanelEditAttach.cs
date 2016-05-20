using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

// 挂节点编辑面板
public class PanelEditAttach : MonoBehaviour {

    public Dictionary<string, AttachComp> AttachPointDic = new Dictionary<string, AttachComp>();
    public string CurAttachName;
    public AttachComp CurAttach;

	void Start () {
	}

    public void Initlaize()
    {
        AttachPointDic.Add("Head", GameObject.Find("attach_head").GetComponent<AttachComp>());
        AttachPointDic.Add("Face", GameObject.Find("attach_face").GetComponent<AttachComp>());
        AttachPointDic.Add("Body", GameObject.Find("attach_body").GetComponent<AttachComp>());
    }

    void OnGUI()
    {
        //GUI.skin = ToolManager.Instance.Skin;

        //// 顶部
        //GUI.Box(new Rect(0,0, Screen.width, 50), "");

        //GUI.Label(new Rect(10, 10, 180, 20), "Spine名称: ");
        //GUI.Label(new Rect(100, 10, 200, 20), ToolManager.Instance.CurSpineName);

        //GUILayout.BeginArea(new Rect(250,5, Screen.width-260, 60));
        //GUILayout.BeginHorizontal();
        //foreach(var pair in AttachPointDic)
        //{
        //    if (GUILayout.Toggle(CurAttachName == pair.Key, pair.Key, GUI.skin.GetStyle("toggle")))
        //    {
        //        if (CurAttach)
        //            CurAttach.renderer.material.color = new Color(255, 255, 255, 128);

        //        CurAttachName = pair.Key;
        //        CurAttach = pair.Value;
        //        CurAttach.renderer.material.color = new Color(255, 255, 255, 255);
        //    }
        //}
        //GUILayout.EndHorizontal();
        //GUILayout.EndArea();

        //// 底部
        //GUI.Box(new Rect(0, Screen.height-50, Screen.width, 50), "");
        //GUI.Label(new Rect(0, Screen.height - 25, Screen.width, 25), ToolManager.Instance.InfoStr);

        //GUILayout.BeginArea(new Rect(Screen.width-300, Screen.height - 50, 300, 50));
        //GUILayout.BeginHorizontal();

        //if (GUILayout.Button("重置"))
        //{
        //    Vector3 resetPos = new Vector3(0,0, ToolManager.AttachZ);
        //    foreach(var pair in AttachPointDic)
        //    {
        //        pair.Value.transform.position = resetPos;
        //    }
        //}

        //if(GUILayout.Button("保存"))
        //{
        //    ToolManager.Instance.WriteAttachToExcel(ToolManager.Instance.CurSpineName);
        //    // 转表
        //    Process convertProcess = new Process();
        //    convertProcess.StartInfo.FileName = Path.Combine(ToolManager.Instance.virtualWorkDir, ToolManager.Instance.JsonConvertor);
        //    convertProcess.StartInfo.UseShellExecute = false;
        //    convertProcess.StartInfo.CreateNoWindow = true;
        //    convertProcess.StartInfo.RedirectStandardInput = true;
        //    convertProcess.Start();
        //    convertProcess.StandardInput.Write('\n');       // 写入一个回车让程序退出
        //    convertProcess.WaitForExit();

        //    // 拷贝Lua配置表到游戏中
        //    // TODO 抽取成批处理
        //    string cpySrc = Path.Combine(ToolManager.Instance.virtualWorkDir, ToolManager.Instance.HeroJson);
        //    string cpyDst = Path.Combine(ToolManager.Instance.virtualWorkDir, ToolManager.Instance.HeroJsonInClient);
        //    File.Copy(cpySrc, cpyDst, true);
        //}

        //if(GUILayout.Button("关闭"))
        //{
        //    Destroy(ToolManager.Instance.SkeletonAnim);
        //    ToolManager.Instance.SceneMode = EScreenMode.OpenSpine;
        //    ToolManager.Instance.PanelEditAttach.gameObject.SetActive(false);
        //    ToolManager.Instance.PanelOpenSpine.gameObject.SetActive(true);
        //}
        //GUILayout.EndHorizontal();
        //GUILayout.EndArea();
    }
}
