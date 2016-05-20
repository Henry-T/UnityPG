using UnityEngine;
using System.Collections;

public class TestComp : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(loadAll());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator loadAll()
    {
        for(int i = 0; i<100; i++)
        {
            Debug.Log("one!");
            yield return new WaitForSeconds(2);
            for(int k=0; k< 99999; k++)
            {                
                for(int j=0; j< 99999; j++)
                {

                }
            }
        }
    }

    void OnGUI()
    {
        GUILayout.Label(Time.realtimeSinceStartup.ToString());
    }
}
