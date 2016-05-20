using UnityEngine;
using System.Collections;
using System.Drawing;
using System.IO;
using UnityEditor;

public class ConvertFileToAsset : MonoBehaviour {

	// Use this for initialization
	void Start () {


        Bitmap bitmap = new Bitmap("man_ok.png");

        Texture2D tex2D = new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.ARGB32, false);
        tex2D.name = Path.GetFileNameWithoutExtension("man_ok.png");

        for (int row = 0; row < bitmap.Height; row++)
        {
            for (int col = 0; col < bitmap.Width; col++)
            {
                System.Drawing.Color srcColor = bitmap.GetPixel(col, row);
                UnityEngine.Color tgtColor = new UnityEngine.Color(srcColor.R, srcColor.G, srcColor.B, srcColor.A);
                tex2D.SetPixel(col, row, tgtColor);
            }
        }

        renderer.material.mainTexture = tex2D;

        System.Drawing.Color srcColor1 = bitmap.GetPixel(100, 100);
        UnityEngine.Color tgtColor1 = new UnityEngine.Color(srcColor1.R, srcColor1.G, srcColor1.B, srcColor1.A);
        Debug.Log(tgtColor1.ToString());
        Debug.Log(srcColor1.ToString());

        tex2D.Apply();

        // AssetDatabase.CreateFolder("", "hello");
         //AssetDatabase.CreateAsset(tex2D, "dragon.asset");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
