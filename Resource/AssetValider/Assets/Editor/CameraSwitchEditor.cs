using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CameraSwitch))]
public class CameraSwitchEditor : Editor {

    public bool CamFollow = false;

    public override void OnInspectorGUI ()
    {

        EditorGUILayout.LabelField("编辑 - 摄像机位");
        
        EditorGUILayout.Separator();
        base.DrawDefaultInspector();

        // EditorGUILayout.LabelField("控制");

        /*
        EditorGUILayout.Toggle("依附Path", true);

        CamFollow = EditorGUILayout.Toggle("相机跟随标记",CamFollow);
        if(CamFollow)
        {
            if(Camera.current && (target as CameraSwitch).camera)
            {
                Camera.current.transform.position = (target as CameraSwitch).Camera.transform.position;
                Camera.current.transform.rotation = (target as CameraSwitch).Camera.transform.rotation;
            }
        }

        if(GUILayout.Button("跟随机位"))
        {
        }

        if(GUILayout.Button("脱离机位"))
        {

        }
        */

        bool showCSGizmo = EditorGUILayout.Toggle("开启场景标记", CameraSwitch.ShowGizmo);
        if(showCSGizmo != CameraSwitch.ShowGizmo)
        {
            CameraSwitch.ShowGizmo = showCSGizmo;
            EditorUtility.SetDirty(target);
        }

        // 自由模式

        // 精确模式
    }
}
