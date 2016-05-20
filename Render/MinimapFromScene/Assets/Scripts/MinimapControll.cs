using UnityEngine;
using System.Collections;

public class MinimapControll : MonoBehaviour {
    public GameObject Map;
    public GameObject Player;

	void Start () {
	
	}
	
	void Update () {
        if (Map && Player)
        {
            Map.renderer.material.SetTextureOffset("_MainTex", new Vector2(
                Player.transform.position.x * 0.0005f,
                Player.transform.position.z * 0.0005f
            ));
        }
	}
}
