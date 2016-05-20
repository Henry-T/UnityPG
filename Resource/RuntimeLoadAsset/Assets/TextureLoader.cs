using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class TextureLoader : MonoBehaviour {


	IEnumerator Start () {
		// 测试表明 Resources.LoadAssetAtPath 只能加载经Unity处理过的资源
		//Texture2D texture = Resources.LoadAssetAtPath<Texture2D>(texPath);
		//renderer.material.mainTexture = texture;
		//Debug.Log(texture.name);
		
		string texPath = "RuntimeAsset/dragon.png";
		FileInfo fInfo = new FileInfo(texPath);

		print (fInfo.FullName);
			
		WWW www = new WWW("file://" + fInfo.FullName);
		print("start!");
		yield return www;
		
		print ("finished!");
		renderer.material.mainTexture = www.texture;
		print (www.texture.name);
	}

	void Update () {
	
	}
}
