using UnityEngine;
using System.Collections;

public class AnimationTest : MonoBehaviour {

	public const string ANIMATION_01 = "idle";
	public const string ANIMATION_02 = "run";
	public const string ANIMATION_03 = "walk";
	public const string ANIMATION_04 = "jump_pose";

	// Use this for initialization
	void Start () {
	
		gameObject.animation.wrapMode = WrapMode.Loop;
	}
	
	// Update is called once per frame
	void Update () {
	

	}

	void PlayAnimation() {

	}

	void StateChange() {

	}
}
