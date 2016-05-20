using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ModelPreProcesser : AssetPostprocessor
{
	void OnPreprocessModel()
	{
		ModelImporter modelImporter = assetImporter as ModelImporter;
		
		modelImporter.materialName = ModelImporterMaterialName.BasedOnMaterialName;
		modelImporter.globalScale = 1;
		
		modelImporter.animationType = ModelImporterAnimationType.None;
		modelImporter.importAnimation = false;

		modelImporter.optimizeMesh = false;		// 禁止优化

	}
	
	void OnPostprocessModel(GameObject gameObject)
	{
		Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

		MeshFilter filter = gameObject.GetComponentInChildren<MeshFilter>();
		Mesh mesh = filter.sharedMesh;
		
		Debug.Log("原始索引数量: " + mesh.triangles.Length);
		Debug.Log("原始顶点数量: " +mesh.vertexCount );

		Vector3[] srcVertices = mesh.vertices;		// 原始顶点
		Vector3[] srcNormals = mesh.normals;		// 原始法线
		int[] srcTriangles = mesh.triangles;			// 原始索引

		// 重新构建不共享的顶点列表
		Vector3[] dstVertices = new Vector3[srcTriangles.Length];
		for(int i=0; i<srcTriangles.Length; i++)
		{
			Vector3 srcVert = srcVertices[srcTriangles[i]];
			dstVertices[i] = new Vector3(srcVert.x, srcVert.y, srcVert.z);
		}
		
		Vector3[] dstNormals = new Vector3[srcTriangles.Length];
		for(int i=0; i<srcTriangles.Length; i++)
		{
			Vector3 srcNorm = srcVertices[srcTriangles[i]];
			dstNormals[i] = new Vector3(srcNorm.x, srcNorm.y, srcNorm.z);
		}

		int[] dstTriangles = new int[srcTriangles.Length];
		for(int i=0; i<dstTriangles.Length; i++)
		{
			dstTriangles[i] = i;
		}

		mesh.vertices = dstVertices;
		mesh.normals = dstNormals;
		mesh.triangles = dstTriangles;
		
		Debug.Log("分离后索引数量: " + mesh.triangles.Length);
		Debug.Log("分离后顶点数量: " +mesh.vertexCount );

		// Color做三角形中心
		Color[] triangleCenters = new Color[mesh.triangles.Length / 3];
		Vector3[] pos2 = new Vector3[2];
		for(int i=0; i<mesh.triangles.Length; i++)
		{
			Vector3 position = mesh.vertices[mesh.triangles[i]];
			if(i % 3 == 2)
			{
				Vector3 center = (pos2[0] + pos2[1] + position) / 3;
				triangleCenters[i/3] = new Color(center.x, center.y, center.z, 0);
			}
			else
				pos2[i%3] = position;
		}

		Color[] triangleCentersExpand = new Color[mesh.triangles.Length];

		Color[] jo = new Color[]{new Color(0,0,0,0), new Color(1,0,0,0), new Color(0,1,0,0), new Color(1,1,0,0)};
		for(int i=0; i<mesh.triangles.Length; i++)
		{
			Debug.Log("中心: " + triangleCenters[i/3]);
		 	//triangleCentersExpand[i] = triangleCenters[i/3];
			triangleCentersExpand[i] = jo[i%4];
		}

		//mesh.colors = triangleCentersExpand;

		// uv1 用做旋转速度
		Vector2[] rotateSpeeds = new Vector2[mesh.triangles.Length];
		for(int i=0; i<rotateSpeeds.Length; i++)
		{
			rotateSpeeds[i] = new Vector2(Random.Range(0,1), Random.Range(0,1));
		}

		mesh.uv1 = rotateSpeeds;
	}
}