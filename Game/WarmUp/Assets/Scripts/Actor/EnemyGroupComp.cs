using UnityEngine;
using System.Collections;

public class EnemyGroupComp : MonoBehaviour {

	public EnemyComp EnemyComp;

	// Use this for initialization
	void Start () {
		EnemyComp = transform.FindChild("Enemy").GetComponent<EnemyComp>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
