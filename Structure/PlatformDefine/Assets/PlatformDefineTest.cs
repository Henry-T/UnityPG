using UnityEngine;
using System.Collections;

public class PlatformDefineTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		#if UNITY_EDITOR
		Debug.Log("Unity Editor");
		#endif
		
		#if UNITY_IPHONE
		Debug.Log("Iphone");
		#endif
		
		#if UNITY_STANDALONE_OSX
		Debug.Log("Stand Alone OSX");
		#endif
		
		#if UNITY_STANDALONE_WIN
		Debug.Log("Stand Alone Windows");
		#endif
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
