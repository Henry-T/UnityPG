using UnityEngine;
using System.Collections.Generic;

public class TestMatch : MonoBehaviour {

    private Dictionary<GameObject, GameObject> matchDic = new Dictionary<GameObject, GameObject>();
    public GameObject MatchPrefab;
    public Camera SceneCamera;
    public Camera ScreenCamera;

	void Start () {
        Debug.Log("屏幕尺寸: " + Screen.width + "-" + Screen.height);
        Object[] targets =  GameObject.FindGameObjectsWithTag("target");

        foreach(GameObject go in targets)
        {
            GameObject match = Instantiate(MatchPrefab) as GameObject;
            matchDic.Add(go, match);
        }
	}
	
	void Update () {
	    foreach(GameObject k in matchDic.Keys)
        {
            GameObject v = matchDic[k];
            Vector3 screenPos = SceneCamera.WorldToScreenPoint(k.transform.position);
            v.transform.position = screenPos;
        }
	}
}
