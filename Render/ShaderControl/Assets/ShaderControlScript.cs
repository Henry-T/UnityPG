using UnityEngine;
using System.Collections;

public class ShaderControlScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Material mat = renderer.material;
		
		mat.color = Color.blue;
		mat.mainTexture = Resources.LoadAssetAtPath<Texture>("Texture.tga");
		mat.mainTextureOffset = new Vector2(0.5f, 0.5f);
		mat.mainTextureScale = Vector2.one;
	
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log("Set Shader Value");
		Material mat = renderer.material;

		mat.color = Color.blue;
		mat.mainTexture = Resources.LoadAssetAtPath<Texture>("Texture.tga");
		mat.mainTextureOffset = new Vector2(0.5f, 0.5f);
		mat.mainTextureScale = Vector2.one;

		mat.SetColor("ttt", Color.red);
	}
}
