using UnityEngine;
using System.Collections;

public class IllusionTest : MonoBehaviour {

	public GameObject Player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			createIllusion();
		}
	}

	private void createIllusion()
	{
		GameObject illusionClone = Instantiate(Player) as GameObject;
		
		// 只保留必要的组件
		var comps = illusionClone.GetComponentsInChildren<Component>();
		foreach(var comp in comps)
		{
			if(comp is Animation || comp is MeshFilter || comp is Renderer || comp is Transform )
			{

			}
			//else if(comp is CharacterController)
			//{
				// TEMP 绕过依赖问题
			//}
			else
			{
				Destroy(comp);
			}
		}

		// 转换Shader
		Renderer[] renderers = illusionClone.GetComponentsInChildren<Renderer>();
		foreach(Renderer renderer in renderers)
		{
			foreach(Material mat in renderer.materials)
			{
				mat.shader = Shader.Find("Transparent/Diffuse");
				mat.color = new Color(0,0,0,1);
			}
		}

		// 停止动画
		Animation[] anims = illusionClone.GetComponentsInChildren<Animation>();
		foreach(Animation anim in anims)
		{
			anim.Stop();
		}

		// 附加控制
		illusionClone.AddComponent<IllusionControl>();
	}
}
