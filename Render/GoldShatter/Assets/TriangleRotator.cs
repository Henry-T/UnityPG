using UnityEngine;
using System.Collections;

public class TriangleRotator : MonoBehaviour {

    Mesh newMesh;
    Vector3[] newVerts;

    Color[] newColors;
    Vector3[] newNormals;
    Vector4[] newTangents;
    Vector2[] newUV;
    Vector2[] newUV1;
    Vector2[] newUV2;

	void Start () {
		if(!renderer)
			return;

        Mesh oldMesh = GetComponent<MeshFilter>().sharedMesh;
        int subMeshCnt = oldMesh.subMeshCount;
        int[][] oldTriangles = new int[subMeshCnt][];

        for(int i=0; i<subMeshCnt; i++)
        {
            oldTriangles[i] = oldMesh.GetTriangles(i);
        }

        Color[] oldColors = oldMesh.colors;
        Vector3[] oldNormals = oldMesh.normals;
        Vector4[] oldTangents = oldMesh.tangents;
        Vector2[] oldUV = oldMesh.uv;
        Vector2[] oldUV1 = oldMesh.uv1;
        Vector2[] oldUV2 = oldMesh.uv2;
        int oldVertCnt = oldMesh.vertexCount;
        Vector3[] oldVerts = oldMesh.vertices;
        

	}

	void Update () {
		if(!renderer)
			return;
	}
}
