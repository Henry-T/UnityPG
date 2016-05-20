using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SkinnedMeshCombiner : MonoBehaviour {

	public Transform BoneRoot;		// 动画骨骼&共享骨骼	// 改进-从资源读取

	void Start()
	{
		SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
		List<Transform> bones = new List<Transform>();							// 拷贝自标准骨骼资源的骨骼列表
		List<BoneWeight> boneWeights = new List<BoneWeight>();					// 所有元SkinnedMeshRenderer权重冗余集合
		List<Texture2D> textures = new List<Texture2D>();						// 所有SkinnedMeshRenderer引用的贴图 用于合并Atlas
		List<CombineInstance> combineInstances = new List<CombineInstance>();	// 每隔元SkinnedMeshRenderer对应一个CombineInstance
		int numSubs = 0;	// Sub Mesh 计数

		// 读取标准骨骼
		bones.AddRange(BoneRoot.GetComponentsInChildren<Transform>());

		// 计数 Sub Mesh
		foreach(SkinnedMeshRenderer r in renderers)
		{
			numSubs += r.sharedMesh.subMeshCount;
		}
		
		int[] subMeshIndex = new int[numSubs];		// 存储子Mesh顶点数量
		int boneOffset = 0;			// 循环中记录合并后骨骼序
		for( int s = 0; s < renderers.Length; s++ )
		{
			SkinnedMeshRenderer curRenderer = renderers[s];

			BoneWeight[] meshBoneweight = curRenderer.sharedMesh.boneWeights;

			foreach( BoneWeight bw in meshBoneweight )
			{
				BoneWeight bWeight = bw;

				string oldName0 = curRenderer.bones[bWeight.boneIndex0].name;
				bWeight.boneIndex0 = bones.FindIndex(b=>b.name == oldName0);				
				string oldName1 = curRenderer.bones[bWeight.boneIndex1].name;
				bWeight.boneIndex1 = bones.FindIndex(b=>b.name == oldName1);
				string oldName2 = curRenderer.bones[bWeight.boneIndex2].name;
				bWeight.boneIndex2 = bones.FindIndex(b=>b.name == oldName2);
				string oldName3 = curRenderer.bones[bWeight.boneIndex3].name;
				bWeight.boneIndex3 = bones.FindIndex(b=>b.name == oldName3);

				boneWeights.Add( bWeight );
			}
			boneOffset += curRenderer.bones.Length;

			if( curRenderer.material.mainTexture != null )
				textures.Add( curRenderer.renderer.material.mainTexture as Texture2D );
			
			CombineInstance ci = new CombineInstance();
			ci.mesh = curRenderer.sharedMesh;
			subMeshIndex[s] = ci.mesh.vertexCount;
			ci.transform = curRenderer.transform.localToWorldMatrix;
			combineInstances.Add( ci );
			
			Object.Destroy( curRenderer.gameObject );
		}
	
		List<Matrix4x4> bindposes = new List<Matrix4x4>();

		for ( int b = 0; b < bones.Count; b++)
		{
			bindposes.Add( bones[b].worldToLocalMatrix * transform.worldToLocalMatrix );
		}
		
		SkinnedMeshRenderer combinedRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
		combinedRenderer.sharedMesh = new Mesh();
		combinedRenderer.sharedMesh.CombineMeshes( combineInstances.ToArray(), true, true );
		
		Texture2D skinnedMeshAtlas = new Texture2D( 128, 128, TextureFormat.ARGB32, true );
		Rect[] packingResult = skinnedMeshAtlas.PackTextures( textures.ToArray(), 0 );
		Vector2[] originalUVs = combinedRenderer.sharedMesh.uv;
		Vector2[] atlasUVs = new Vector2[originalUVs.Length];
		
		int rectIndex = 0;
		int vertTracker = 0;
		for( int i = 0; i < atlasUVs.Length; i++ )
		{
			if( i >= subMeshIndex[rectIndex] + vertTracker ) {   // 这是什么意思? 原本是有错误 "01.png"的，这块挪上来就对了            
				vertTracker += subMeshIndex[rectIndex];
				rectIndex++;                
			}

			atlasUVs[i].x = Mathf.Lerp( packingResult[rectIndex].xMin, packingResult[rectIndex].xMax, originalUVs[i].x );
			atlasUVs[i].y = Mathf.Lerp( packingResult[rectIndex].yMin, packingResult[rectIndex].yMax, originalUVs[i].y );
		}
		
		Material combinedMat = new Material( Shader.Find( "Unlit/Unlit_2Side" ) );
		combinedMat.mainTexture = skinnedMeshAtlas;
		combinedRenderer.sharedMesh.uv = atlasUVs;
		combinedRenderer.sharedMaterial = combinedMat;

		combinedRenderer.bones = bones.ToArray();

		combinedRenderer.sharedMesh.boneWeights = boneWeights.ToArray();
		combinedRenderer.sharedMesh.bindposes = bindposes.ToArray();
		combinedRenderer.sharedMesh.RecalculateBounds();
	}
}