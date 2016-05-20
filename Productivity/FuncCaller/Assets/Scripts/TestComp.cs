using UnityEngine;
using System.Collections;

public class TestComp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Func1()
	{
		Debug.LogWarning("Func1 调用");
	}

	public void Func2()
	{
		Debug.LogWarning("Func2 调用");
	}
}
