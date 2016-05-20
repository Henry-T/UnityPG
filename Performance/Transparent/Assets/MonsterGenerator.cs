using UnityEngine;
using System.Collections;

public class MonsterGenerator : MonoBehaviour {
	public GameObject CloneBase;

	// Use this for initialization
    void Start () {
        //for (int k=0; k<10; k++)
        //{
            //k = 5;
            int k = 5;
            for (int i=0; i<10; i++)
            {
                for (int j=0; j<10; j++)
                {
                    GameObject newModel = GameObject.Instantiate(CloneBase) as GameObject;
                    newModel.transform.position = new Vector3((i - 5) * 3, (k-5) * 1, (j - 5) * 3);
                    newModel.GetComponentInChildren<Renderer>().name = "XYZ";
                }
            }
        //}

        SwitchShader();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown ("Fire1")) {
            SwitchShader();
        }
	}

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, Screen.width, 50), new GUIContent(shaderName));
    }

	public int ShaderId = -1;
    private string shaderName;
	public void SwitchShader()
	{
		ShaderId ++;
		ShaderId %= 5;

		switch (ShaderId)
        {
            case 0:
                shaderName = "Mobile/Unlit (Supports Lightmap)";
                break;
            case 1:
                shaderName = "Mobile/Diffuse";
                break;
            case 2:
                shaderName = "Transparent/Cutout/Diffuse";
                break;
            case 3:
                shaderName = "Transparent/Diffuse";
                break;
            case 4:
                shaderName = "Transparent/Cutout/Soft Edge Unlit";
                break;
        }
        Renderer[] rendereres = CloneBase.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in rendereres)
        {
            Debug.LogWarning(shaderName);
            renderer.sharedMaterial.shader = Shader.Find(shaderName);
        }
	}
}
