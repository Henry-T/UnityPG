using UnityEngine;
using System.Collections;

public class LevelButtonGenerate : MonoBehaviour {

	public GameObject btnPrefab;

	// Use this for initialization
	void Start () {
		for (int i =0; i<8; i++) {
			GameObject newBtn = Instantiate (btnPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			newBtn.transform.parent = this.gameObject.transform;
			UILabel lb = newBtn.transform.FindChild("lb_name").gameObject.GetComponent<UILabel>();
			lb.text = i + "!";
			

			//GameObject.Find(newBtn.name + ""
		}

	}
	
	// Update is called once per frame
	void Update () {

	}
}
