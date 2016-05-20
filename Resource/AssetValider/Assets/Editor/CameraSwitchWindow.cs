using UnityEngine;
using System.Collections;
using UnityEditor;

public class CameraSwitchWindow : EditorWindow 
{
    private bool initialized = false;

    [MenuItem("Tools/编辑机位")]
    static void OpenThisWindow()
    {
        CameraSwitchWindow thisWindow = EditorWindow.GetWindow<CameraSwitchWindow>();
        thisWindow.initialized = false;
    }

    void OnGUI()
    {
        if(!initialized)
        {
            Initialize();
        }
        
        if(GUILayout.Button("创建新区域"))
        {
            Object prefab = Resources.LoadAssetAtPath<Object>("Assets/EditorPrefab/CameraSwitch.prefab");
            if(prefab)
            {
                GameObject instance = GameObject.Instantiate(prefab) as GameObject;
                instance.transform.position = Vector3.zero;

                ZonePolygon zonePoly = instance.GetComponent<ZonePolygon>();
                zonePoly.CreateBasePoly();
            }
            else
            {
                EditorUtility.DisplayDialog("创建失败 - 找不到Prefab", "Assets/EditorPrefab/CameraSwitch.prefab", "确定");
            }
        }
    }

    void Initialize()
    {
        initialized = true;
    }
}