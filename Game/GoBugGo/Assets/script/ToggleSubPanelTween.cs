using UnityEngine;
using System.Collections;

public class ToggleSubPanelTween : MonoBehaviour {
	public GameObject target;

	// Use this for initialization
	void Start () {
		//this.GetComponent (UIToggle);

	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnClick(){
		if (GetComponent<UIToggle> ().value) {
			Debug.Log ("toggle on!");
			target.GetComponents<TweenPosition>()[0].PlayForward();
			target.GetComponents<TweenAlpha>()[0].PlayForward();

		} else {
			Debug.Log ("toggle off!");
			target.GetComponents<TweenPosition>()[0].PlayReverse();
			target.GetComponents<TweenAlpha>()[0].PlayReverse();
		}
	}
}
