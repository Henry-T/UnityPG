using UnityEngine;
using System.Collections;

public class FillGrid : MonoBehaviour {
    public GameObject Base;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < 10; i++)
        {
            GameObject newObj = Instantiate(Base) as GameObject;
            newObj.transform.parent = transform;
            newObj.transform.localPosition = Vector3.zero;
            newObj.transform.localScale = Vector3.one;
        }
        this.GetComponent<UIGrid>().repositionNow = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
