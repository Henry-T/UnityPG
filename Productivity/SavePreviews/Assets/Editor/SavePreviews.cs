using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class SavePreviews : MonoBehaviour {

	[MenuItem("Assets/保存预览图")]
	static void SaveAll()
	{
		if(!Directory.Exists("Previews"))
		{
			Directory.CreateDirectory("Previews");
		}

		foreach(Object obj in Selection.objects)
		{
		    Texture2D tex = AssetPreview.GetAssetPreview(obj);
			if(tex != null)
			{
				
				byte[] bt = tex.EncodeToPNG();
				string path = "Previews/" + obj.name + ".png";
				FileStream fs = File.Create(path);
				fs.Write(bt, 0, bt.Length);
				fs.Close();
			}
			else
			{
				Debug.LogWarning("无法生成缩略图 Name:" + obj.name);
			}
		}
	}
}
