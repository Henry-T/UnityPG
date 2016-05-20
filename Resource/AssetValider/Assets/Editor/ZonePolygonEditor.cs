using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(ZonePolygon))]
public class ZonePolygonEditor : Editor {

    public bool EnableVertEdit = false;
    public override void OnInspectorGUI()
    {
        ZonePolygon zonePoly = target as ZonePolygon;

        GUILayout.Label("编辑 - 触发区");
        EditorGUILayout.Separator();

        float oldHeight = zonePoly.Height;

        base.DrawDefaultInspector();

        bool meshDirty = false;


        if(oldHeight != zonePoly.Height)
            meshDirty = true;

        if(!zonePoly)
            return;

        MeshFilter meshFilter = zonePoly.GetComponent<MeshFilter>();

        bool enableVertEdit = EditorGUILayout.Toggle("开启场景点编辑", EnableVertEdit);
        if(enableVertEdit != EnableVertEdit)
        {
            EnableVertEdit = enableVertEdit;
            EditorUtility.SetDirty(target);
        }

        for (int i = 0; i < zonePoly.Verts.Count; i++ )
        {
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[]{});
            Vector3 node = zonePoly.Verts[i];
            Vector3 newPos = EditorGUILayout.Vector3Field("位置", node);
            if(newPos != node)
            {
                node = newPos;
                zonePoly.Verts[i] = node;
                meshDirty = true;
            }
            if(GUILayout.Button(new GUIContent("删")))
            {
                if(zonePoly.Verts.Count > 3)
                {
                    zonePoly.Verts.RemoveAt(i);
                    i--;
                    meshDirty = true;
                }
            }
            if(GUILayout.Button(new GUIContent("加")))
            {
                int zonePolyVertCnt = zonePoly.Verts.Count;
                Vector3 pos = (zonePoly.Verts[(i)%zonePolyVertCnt] + zonePoly.Verts[(i+1)%zonePolyVertCnt]) * 0.5f;
                zonePoly.Verts.Insert((i+1)%zonePolyVertCnt, pos);
                meshDirty = true;
            }
            if(GUILayout.Button(new GUIContent("上")))
            {
                int lastId = (i - 1 + zonePoly.Verts.Count) % zonePoly.Verts.Count;
                Vector3 temp = zonePoly.Verts[i];
                zonePoly.Verts[i] = zonePoly.Verts[lastId];
                zonePoly.Verts[lastId] = temp;
                meshDirty = true;
            }
            if (GUILayout.Button(new GUIContent("下")))
            {
                int nextId = (i + 1 + zonePoly.Verts.Count) % zonePoly.Verts.Count;
                Vector3 temp = zonePoly.Verts[i];
                zonePoly.Verts[i] = zonePoly.Verts[nextId];
                zonePoly.Verts[nextId] = temp;
                meshDirty = true;
            }            
            EditorGUILayout.EndHorizontal();
        }        
        
        if(GUILayout.Button(new GUIContent("添加点")))
        {
            int zonePolyVertCnt = zonePoly.Verts.Count;
            Vector3 pos = (zonePoly.Verts[(zonePolyVertCnt-1)%zonePolyVertCnt] + zonePoly.Verts[0]) * 0.5f;
            zonePoly.Verts.Insert(zonePoly.Verts.Count, pos);
            meshDirty = true;
        }
        if(GUILayout.Button("刷新Mesh"))
        {
            meshDirty = true;
        }

        EditorGUILayout.Separator();

        if(GUILayout.Button("翻转顶点"))
        {
            zonePoly.Verts.Reverse();
            meshDirty = true;
        }
        if(GUILayout.Button("重置为基本体"))
        {
            zonePoly.CreateBasePoly();
        }

        if(meshDirty)
        {
            zonePoly.RegenerateMesh();
        }

        if(GUILayout.Button("保存Mesh到文件"))
        {
            SaveMeshAsset(zonePoly);
        }
    }

    // 将ZonePolygon的Mesh存储为asset文件
    public static void SaveMeshAsset(ZonePolygon zonePolygon)
    {
        if(!zonePolygon || !zonePolygon.Mesh)
            return;
        
        if(!Directory.Exists("Assets/ZonePolygon"))
            Directory.CreateDirectory("Assets/ZonePolygon");
        
        string filePath = AssetDatabase.GetAssetPath(zonePolygon.Mesh);
        bool isFile = !string.IsNullOrEmpty(filePath);
        
        // 不是文件资源 或 强制创建新文件
        if(!isFile)
        {
            int tempId = 0;
            while(string.IsNullOrEmpty(filePath))
            {
                string name = "Assets/ZonePolygon/ZonePolygon_" + tempId.ToString("D4") + ".asset";
                if(!File.Exists(name))
                    filePath = name;
                else
                    tempId += 1;
            }
        }

        if(isFile)
            AssetDatabase.SaveAssets();
        else
            AssetDatabase.CreateAsset(zonePolygon.Mesh, filePath);
        
        // 同步Mesh引用
        MeshCollider collider = zonePolygon.GetComponent<MeshCollider>();
        if(collider)
            collider.sharedMesh = zonePolygon.Mesh;

        MeshFilter meshFilter = zonePolygon.GetComponent<MeshFilter>();
        if(meshFilter)
            meshFilter.sharedMesh = meshFilter.sharedMesh;
    }

    void OnSceneGUI()
    {
        bool meshDirty = false;

        ZonePolygon zonePolygon = target as ZonePolygon;
        Transform Transform = zonePolygon.transform;

        // 画线 & 轴
        Handles.color = Color.green;
        if (EnableVertEdit && zonePolygon.Verts.Count >= 3)
        {
            for (int i = 0; i < zonePolygon.Verts.Count; i++ )
            {
                Vector3 node = zonePolygon.Verts[i];
                Vector3 nextNode = zonePolygon.Verts[(i + 1)%zonePolygon.Verts.Count];
                
                Vector3 beginPos = Transform.TransformPoint(node);
                Vector3 endPos = Transform.TransformPoint(nextNode);
                Handles.DrawLine(beginPos, endPos);
                
                Handles.Label(beginPos, new GUIContent((i + 1).ToString()));
                
                // 更新位置
                Vector3 newPos = Handles.PositionHandle(Transform.TransformPoint(node), Quaternion.identity);
                if(node != newPos)
                {
                    meshDirty = true;
                }
                node = Transform.InverseTransformPoint(newPos);
                node.y = 0;               

                zonePolygon.Verts[i] = node;
            }
        }

        if(meshDirty)
        {
            zonePolygon.RegenerateMesh();
        }

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
