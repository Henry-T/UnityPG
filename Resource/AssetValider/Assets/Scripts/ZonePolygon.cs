using UnityEngine;
using System.Collections.Generic;

// =========================================
// WARN 此项需在资源和客户端项目完全同步！
// =========================================
public class ZonePolygon : MonoBehaviour {
    public List<Vector3> Verts = new List<Vector3>();
    public float Height = 2;
    public Mesh Mesh;
    private List<Vector3> polyVerts = new List<Vector3>();

    public bool CreateBasePoly()
    {
        Verts = new List<Vector3>();
        Verts.Add(new Vector3(-2f, 0, -2f));
        Verts.Add(new Vector3(-2f, 0,  2f));
        Verts.Add(new Vector3( 2f, 0,  2f));
        Verts.Add(new Vector3( 2f, 0, -2f));

        Height = 1;

        return RegenerateMesh();
    }

    public bool RegenerateMesh()
    {
        if(Verts.Count < 3)
            return false;

        if(!Mesh)
            Mesh = new Mesh();
        polyVerts = new List<Vector3>();
        
        // 构建体积
        for(int i = 0; i<Verts.Count; i++)
        {
            Vector3 vert = Verts[i];
            polyVerts.Add(new Vector3(vert.x,  Height*0.5f, vert.z));
            polyVerts.Add(new Vector3(vert.x, -Height*0.5f, vert.z));
        }
        Mesh.triangles = new int[0]; // 这里先清空下 否则删减点时下一行会报错
        Mesh.vertices = polyVerts.ToArray();
        int polyVertCnt = Mesh.vertices.Length;

        // 索引
        List<int> triangleList = new List<int>();

        // 顶 逆时针
        int topTriCnt = Verts.Count - 2;
        for(int i = 0; i< topTriCnt; i++)
        {
            int first = 0;
            int second = i * 2 + 2;
            int third = i * 2 + 4;
            triangleList.AddRange(new int[]{first, second, third});
        }

        // 底
        for(int i = 0; i< topTriCnt; i++)
        {
            int first = 1;
            int second = i * 2 + 5;
            int third = i * 2 + 3;
            triangleList.AddRange(new int[]{first, second, third});
        }
        

        // 周
        for(int i = 0; i<Verts.Count; i++)
        {
            int id = i * 2;
            triangleList.AddRange(new int[]{  id, id+1, (id+2)%polyVertCnt});
            triangleList.AddRange(new int[]{(id+2)%polyVertCnt, id+1, (id+3)%polyVertCnt});
        }

        Mesh.triangles = triangleList.ToArray();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if(meshFilter)
        {
            meshFilter.mesh = Mesh;
        }

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if(meshCollider)
        {
            meshCollider.sharedMesh = Mesh;
        }

        return true;
    }
}
