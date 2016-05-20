using UnityEngine;
using System.Collections;

public class SnakeBody : MonoBehaviour {
	public GameObject obj;
	public int Id;
	public GUITexture GuiTex;
	public void Initialize(){
		obj = new GameObject ();
		obj.transform.position = Vector3.zero;
		obj.transform.rotation = Quaternion.identity;
		obj.transform.localScale = new Vector3 (0.05f, 0.05f, 1);

		GuiTex = obj.AddComponent<GUITexture>();
		GuiTex.texture = GameManager.SnakeTexture;
	}
}
