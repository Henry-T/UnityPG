using UnityEngine;
using System.Collections;

public class Exp : MonoBehaviour {

	// Use this for initialization
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<MeshExploder>().Explode();
        GameObject.Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
