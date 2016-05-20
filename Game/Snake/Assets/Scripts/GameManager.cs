using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public GameObject go;
	public static Texture2D SnakeTexture;
	//public TestGame tg;

	// Use this for initialization
	void Start() {
		//SnakeGame.Instance.Initialize ();
		print ("hello world");
		Debug.Log ("hello log");

		go = new GameObject ();
		go.transform.position = new Vector3(0.5f, 0.5f,0);
		go.transform.rotation = Quaternion.identity;
		go.transform.localScale = new Vector3 (0.1f, 0.1f, 1.0f);

		// 创建Texture
		Texture2D tex = new Texture2D (1, 1);
		tex.SetPixel (0, 0, Color.white);
		tex.Apply();

		// 创建GUITexture
		GUITexture guiTexture = go.AddComponent<GUITexture>();
		guiTexture.texture = tex;

		GUIText text = go.AddComponent<GUIText> ();
		text.text = "fine this is it!";

		//
		//go = new GameObject ();

		if (SnakeTexture == null) {
			SnakeTexture = new Texture2D (1, 1);
			SnakeTexture.SetPixel(0,0, Color.green);
			SnakeTexture.Apply();
		}

		for (int i = 0; i<10; i++) {
			SnakeBody body = go.AddComponent<SnakeBody>();
			body.Initialize();
			body.transform.position.Set(0.1f * i, 0.1f * i, 0);
		}
	}

	void OnGUI(){
		//GUI.Button (new Rect (10, 10, 100, 20), "Score: 0");
		//GUI.Button (new Rect (Screen.width - 100 - 10, 10, 100, 20), "Life: ");
	}
}
