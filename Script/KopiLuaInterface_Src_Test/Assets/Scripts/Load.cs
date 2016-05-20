using UnityEngine;
using System.Collections;

public class Load : MonoBehaviour {

	void Start () 
    {
        string url = GetPath() + "data.unity3d";
        //string url = "http://192.168.5.129:8080/unity/data2.unity3d";
        StartCoroutine(LoadAsset(url));

        //string url2 = "http://192.168.5.129:8080/unity/data.unity3d";
        //StartCoroutine(LoadAsset(url2));

        //string url = GetPath() + "data.unity3d";
        //StartCoroutine(LoadAsset(url));
		Debug.Log(url);
	}

    string GetPath()
    {
        string path;
        
        switch(Application.platform)
        {
            case RuntimePlatform.Android:
                path = "jar:file://" + Application.dataPath + "!/assets/";
                break;
            case RuntimePlatform.IPhonePlayer:
                path = Application.dataPath + "/Raw/";
                break;
            default:
                path = "file://" + Application.dataPath + "/StreamingAssets/";
                break;
        }

        return path;
    }

    IEnumerator LoadAsset(string url)
    {
        WWW www = new WWW(url);
        yield return www;

        GameObject obj = Instantiate(www.assetBundle.Load("pre",typeof(GameObject))) as GameObject;
        Vector3 rawScale = obj.transform.localScale;
        obj.transform.parent = transform;
        obj.transform.localScale = rawScale;
        obj.name = "load_test";
    }
}
