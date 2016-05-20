using UnityEngine;
using System.Collections;

public class SpriteResizeTest : MonoBehaviour {

	void OnGUI()
	{
		if(GUI.Button(new Rect(0,0,100, 20), new GUIContent("调整")))
		{
			UISprite sprite = GameObject.FindObjectOfType<UISprite>();
			if(sprite)
			{
				//sprite.SetRect(0,0, 1136, 60);
				sprite.width = 1136;
				sprite.height = 640;
				sprite.pivotOffset.Set(0,0);
				//UIRoot.
			}
		}
	}
}
