using UnityEngine;
using System.Collections;

public class CombineMeshByCode : MonoBehaviour {

	public GameObject Hair;
	public GameObject Face;
	public GameObject Body;
	public GameObject Hands;
	public GameObject Pants;
	public GameObject Boots;

	public SkinnedMeshRenderer HairRenderer;
	public SkinnedMeshRenderer FaceRenderer;
	public SkinnedMeshRenderer BodyRenderer;
	public SkinnedMeshRenderer HandsRenderer;
	public SkinnedMeshRenderer PantsRenderer;
	public SkinnedMeshRenderer BootsRenderer;

	public SkinnedMeshRenderer CombinedRenderer;

	// Use this for initialization
	void Start () {
		GameObject face = Instantiate(Face) as GameObject;
		face.transform.parent = transform;
		FaceRenderer = face.GetComponentInChildren<SkinnedMeshRenderer>();

		GameObject body = Instantiate(Body) as GameObject;
		body.transform.parent = transform;
		BodyRenderer = body.GetComponentInChildren<SkinnedMeshRenderer>();
		
		CombinedRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
		// 重组Mesh后删除原始GO
		Mesh faceMesh = FaceRenderer.sharedMesh;
		Mesh bodyMesh = BodyRenderer.sharedMesh;

		CombineInstance[] combineInstances = new CombineInstance[2];
		combineInstances[0].mesh = faceMesh;
		combineInstances[0].transform = FaceRenderer.transform.localToWorldMatrix;
		combineInstances[1].mesh = bodyMesh;
		combineInstances[1].transform = BodyRenderer.transform.localToWorldMatrix;

		CombinedRenderer.sharedMesh = new Mesh();
		CombinedRenderer.sharedMesh.CombineMeshes(combineInstances);
		
		// 组合材质球引用
		Material[] sharedMats = new Material[2];
		sharedMats[0] = FaceRenderer.sharedMaterial;
		sharedMats[1] = BodyRenderer.sharedMaterial;
		CombinedRenderer.sharedMaterials = sharedMats;

		// Mesh和材质的关联是按照材质在数组中的顺序的，重组Mesh势必会产生一个新的材质球次序
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
