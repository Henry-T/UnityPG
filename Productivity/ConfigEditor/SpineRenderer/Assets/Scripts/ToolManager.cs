using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;
using System.Text;
using System.IO;
using System.Xml;
using System.Linq;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using ConfigType;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Collections;
using System.Threading;

public class ToolManager : MonoBehaviour {
    public static ToolManager Instance;

    public EScreenMode SceneMode;           // 当前的编辑器状态

    public string EffectAttachStr = ""; // 临时存储用于读写的Excel Cell值
    public JSONNode EffectAttachJson;   // 字符串转的Json

    // 配置
    public string virtualWorkDir;       // 调试时让Unity使用的虚拟工作目录
    public string SpineFolder;          // Spine所在的json文件夹
    public string HeroExcel;            // Hero配置文件，读取写入都是这一个文件
    public string JsonConvertor;        // 转表工具所在目录
    public string HeroJson;             // Hero的Json配置
    public string HeroJsonInClient;     // 前端的HeroJson拷贝
    public string DefaultAnim;          // 设点的基准动画
    public int ResWidth;                // 分辨率 - 宽
    public int ResHeight;               // 分辨率 - 高
    public bool DebugMode;              // 输出更多调试信息辅助排错

    public string CurSpineName;                 // 当前打开的Spine名称
    public Transform SpineRoot;                 // Spine骨骼动画根节点
    public Transform LoadedRoot;                // 已加载Spine根
    public SkeletonAnimation SkeletonAnim;      // 当前加载的Spine实例

    public PanelOpenSpine PanelOpenSpine;       // 打开面板
    public PanelEditAttach PanelEditAttach;     // 编辑面板

    // 状态栏文本
    public string InfoStr = "信息: 无";

    public static float AttachZ = -2;          // 所有Attach标记物的z值

    public GUISkin Skin;

    public bool NeedPreloadSpine = false;
    public string SpineDir;
    public int SpineCurrentToPreload = 0;
    public int SpineTotalToPreload = 100;

    public bool NeedLoadSpine = false;
    public string SpineName;
    public string SpineZipPath;

    public bool NeedPlayAnim = false;
    public string AnimName = "";

    public bool PreloadEnabled = false;

    public Dictionary<string, SkeletonAnimation> LoadedSpines;

	void Start () {
        Instance = this;

        LoadedSpines = new Dictionary<string, SkeletonAnimation>();

        // 加载配置文件
        XmlDocument config = new XmlDocument();

        if(File.Exists("config.xml"))
            // 发布环境
            config.Load("config.xml");
        else
            // 开发环境
            config.Load(@"E:\proj\ProjectS\ProjectS\tools\SpineAttachEditor\config.xml");

        XmlNode docNode = (XmlNode)config.DocumentElement;
        if (Application.isEditor)
            virtualWorkDir = docNode.SelectSingleNode("WorkingDir").InnerText;
        else
            virtualWorkDir = Directory.GetCurrentDirectory();
        SpineFolder = docNode.SelectSingleNode("SpineFolder").InnerText;
        HeroExcel = docNode.SelectSingleNode("HeroExcel").InnerText;
        JsonConvertor = docNode.SelectSingleNode("JsonConvertor").InnerText;
        HeroJson = docNode.SelectSingleNode("HeroJson").InnerText;
        HeroJsonInClient = docNode.SelectSingleNode("HeroJsonInClient").InnerText;
        DefaultAnim = docNode.SelectSingleNode("DefaultAnim").InnerText;
        ResWidth = int.Parse(docNode.SelectSingleNode("ResWidth").InnerText);
        ResHeight = int.Parse(docNode.SelectSingleNode("ResHeight").InnerText);
        DebugMode = bool.Parse(docNode.SelectSingleNode("DebugMode").InnerText);

        // NOTE 禁用了分辨率配置窗口，可在配置文件中修改
        Screen.SetResolution(ResWidth, ResHeight, false);

        SpineRoot = GameObject.Find("SpineRoot").transform;
        LoadedRoot = GameObject.Find("LoadedRoot").transform;
        PanelOpenSpine = GameObject.Find("PanelOpenSpine").GetComponent<PanelOpenSpine>();
        PanelEditAttach = GameObject.Find("PanelEditAttach").GetComponent<PanelEditAttach>();

        LoadedRoot.gameObject.SetActive(false);

        SceneMode = EScreenMode.OpenSpine;
        PanelEditAttach.Initlaize();
        PanelOpenSpine.gameObject.SetActive(false);
        PanelEditAttach.gameObject.SetActive(true);

        //Stopwatch watch = new Stopwatch();
        //watch.Start();
        //LoadSpine("spine001", @"E:\proj\ProjectS\ProjectS\client\trunk\projects\res\spine\spine001.zip");
        //watch.Stop();

        //Debug.Log("Spine 加载时间: " + watch.ElapsedMilliseconds.ToString());

        // NeedLoadSpine = true;
        // SpineName = "spine001";
        // SpineZipPath = @"E:\proj\ProjectS\ProjectS\client\trunk\projects\res\spine\spine001.zip";

        // NeedPreloadSpine = true;
        // SpineDir = @"E:\proj\ProjectS\ProjectS\client\trunk\projects\res\spine\";

        ThreadStart threadStart = new ThreadStart(testLoad);
        Thread thread = new Thread(threadStart);
        thread.Start();
    }

    void testLoad()
    {
        SkeletonAnim = SpineZipReader.CreateSkeAnimFromZip(@"E:\proj\ProjectS\ProjectS\client\trunk\projects\res\spine\spine001.zip");
    }

	void Update () {
        // 仅在编辑模式下有效
        if (SceneMode == EScreenMode.EditAttach)
        {
            // 屏幕上下距50的区块是Attach标记的拖拽区
            Rect interactZone = new Rect(0, 50, Screen.width, Screen.height - 100);
            if (!interactZone.Contains(Input.mousePosition))
            {
                // 取消选择
                if (PanelEditAttach.CurAttach)
                {
                    PanelEditAttach.CurAttach.renderer.material.color = new Color(1, 1, 1, 0.5f);
                }
                PanelEditAttach.CurAttach = null;
                PanelEditAttach.CurAttachName = "";
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                int maskBg = LayerMask.GetMask(new string[] { "Sensor" });
                int maskA = LayerMask.GetMask(new string[] { "AttachSensor" });

                if (Input.GetMouseButtonDown(0))
                {
                    // 选择Attach标记
                    RaycastHit hitInfo;
                    if(Physics.Raycast(ray, out hitInfo, 100, maskA))
                    {
                        PanelEditAttach.CurAttach = hitInfo.collider.GetComponent<AttachComp>();
                        PanelEditAttach.CurAttachName = PanelEditAttach.CurAttach.AttachName;
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    // 取消选择
                    if (PanelEditAttach.CurAttach)
                    {
                        PanelEditAttach.CurAttach.renderer.material.color = new Color(1, 1, 1, 0.5f);
                    }
                    PanelEditAttach.CurAttach = null;
                    PanelEditAttach.CurAttachName = "";
                }
                else if (Input.GetMouseButton(0))
                {
                    // 拖拽Attach标记
                    if(PanelEditAttach.CurAttach)
                    {
                        RaycastHit hitInfo;
                        if (Physics.Raycast(ray, out hitInfo, 100, maskBg))
                        {
                            Vector3 newPos = new Vector3(hitInfo.point.x, hitInfo.point.y, AttachZ);
                            PanelEditAttach.CurAttach.transform.position = newPos;
                        }
                    }
                }
            }
        }

        if (NeedLoadSpine)
        {
            if (PreloadEnabled)
            {
                if (SpineCurrentToPreload == 0)
                {
                    bool reloadDone = ReloadSpine(SpineName, SpineZipPath);
                    NeedLoadSpine = reloadDone;
                }
            }
            else
            {
                NeedLoadSpine = false;
                ReloadSpineSync(SpineName, SpineZipPath);
            }

        }

        if (NeedPreloadSpine)
        {
            NeedPreloadSpine = false;
            PreloadAllSpine();
        }

        if (NeedPlayAnim)
        {
            NeedPlayAnim = false;
            if(SkeletonAnim)
            {
                SkeletonAnim.AnimationName = AnimName;
            }
        }
	}

    public void PreloadAllSpine()
    {
        DirectoryInfo spineDirInfo = new DirectoryInfo(SpineDir);
        List<FileInfo> spineZips = spineDirInfo.GetFiles("*.zip").ToList();

        SpineCurrentToPreload = SpineTotalToPreload = spineZips.Count;

        StartCoroutine(preloadSpines(spineZips));
        Debug.Log("move on!");

        //foreach (FileInfo spineZip in spineZips)
        //{
        //    StartCoroutine("preloadSpine", spineZip);
        //}

        // lbSpines.DataContext = spineNameList;
        
    }

    private IEnumerator preloadSpines(List<FileInfo> spineZips)
    {
        yield return null;
        foreach (FileInfo spineZip in spineZips)
        {
            Debug.Log("开始单项加载");
            preloadSpine(spineZip);
            // yield break;
        }
        yield return null;
    }

    private void preloadSpine(FileInfo spineZipInfo)
    {
        string spineName = spineZipInfo.Name.Replace(spineZipInfo.Extension, "");

        SkeletonAnim = SpineZipReader.CreateSkeAnimFromZip(spineZipInfo.FullName);
        SkeletonAnim.name = spineName;
        SkeletonAnim.transform.parent = LoadedRoot;
        LoadedSpines.Add(spineName, SkeletonAnim);
    }

    public void SetAttach(EAttachPoint attachPoint, int x, int y)
    {
        foreach (var pair in PanelEditAttach.AttachPointDic)
        {
            var attachData = EffectAttachJson[pair.Key];
            Vector3 pos = new Vector3(x * SpineZipReader.defaultScale, y * SpineZipReader.defaultScale, 0);
            pair.Value.transform.position = pos;
        }
    }

    // 从Excel中读取EffectAttach
    public bool ReadAttachFromExcel(string codeVal)
    {
        EffectAttachStr = ExcelAccessAgent.ReadAttachInfo(codeVal);
        if(string.IsNullOrEmpty( EffectAttachStr))
            return false;

        EffectAttachJson = JSON.Parse(EffectAttachStr);
        foreach (var pair in PanelEditAttach.AttachPointDic)
        {
            var attachData = EffectAttachJson[pair.Key];
            Vector3 pos = new Vector3(attachData["x"].AsFloat * SpineZipReader.defaultScale, attachData["y"].AsFloat * SpineZipReader.defaultScale, 0);
            pair.Value.transform.position = pos;
        }
        return true;
    }

    // 向Excel写入EffectAttach
    public void WriteAttachToExcel(string codeVal)
    {
        // NOTE SimpleJson 转字符串不太好控制，自己来做这件事吧
        StringBuilder builder = new StringBuilder();
        builder.Append("{");
        List<AttachComp> attachCompList = PanelEditAttach.AttachPointDic.Values.ToList();
        for (int i = 0; i < attachCompList.Count; i++ )
        {
            AttachComp ac = attachCompList[i];
            if(i != attachCompList.Count -1)
                builder.AppendFormat("\\\"{0}\\\":{{\\\"x\\\":{1},\\\"y\\\":{2}}},", ac.AttachName, (ac.transform.position.x / SpineZipReader.defaultScale).ToString(), (ac.transform.position.y / SpineZipReader.defaultScale).ToString());
            else
                builder.AppendFormat("\\\"{0}\\\":{{\\\"x\\\":{1},\\\"y\\\":{2}}}", ac.AttachName, (ac.transform.position.x / SpineZipReader.defaultScale).ToString(), (ac.transform.position.y / SpineZipReader.defaultScale).ToString());
        }
        builder.Append("}");

        EffectAttachStr = builder.ToString();

        Debug.Log(EffectAttachStr);
        ExcelAccessAgent.WriteAttachInfo(codeVal, EffectAttachStr);
    }

    public void ReloadSpineSync(string spineName, string fullZipPath)
    {
        if (CurSpineName == spineName)
            return;

        if (SkeletonAnim)
            Destroy(SkeletonAnim.gameObject);

        SkeletonAnim = SpineZipReader.CreateSkeAnimFromZip(fullZipPath);
        SkeletonAnim.name = spineName;
        SkeletonAnim.loop = true;
        SkeletonAnim.transform.parent = SpineRoot;
        SkeletonAnim.AnimationName = ToolManager.Instance.DefaultAnim;
    }

    public bool ReloadSpine(string spineName, string fullZipPath)
    {
        if (CurSpineName == spineName)
            return true;

        if (SkeletonAnim)
            Destroy(SkeletonAnim.gameObject);

        if (!LoadedSpines.Keys.Contains(spineName))
        {
            preloadSpine(new FileInfo(fullZipPath));
            return false;
        }

        SkeletonAnimation templete = LoadedSpines[spineName];
        SkeletonAnim = (Instantiate(templete.gameObject) as GameObject).GetComponent<SkeletonAnimation>();
        SkeletonAnim.name = spineName;
        SkeletonAnim.loop = true;
        SkeletonAnim.transform.parent = SpineRoot;
        SkeletonAnim.AnimationName = ToolManager.Instance.DefaultAnim;
        return true;
    }
}

public enum EScreenMode
{
    OpenSpine,
    EditAttach,
}
