using UnityEngine;
using System.Collections;

public class AnimClipCtrl : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Animator animator = GetComponent<Animator>();
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void AnimClipEventHandler(AnimationEvent animEvent)
    {
        Debug.Log(animEvent.stringParameter);
    }
}
