using UnityEngine;
using System.Collections;

public class AnimHolder : MonoBehaviour
{
	public Animation Animation;

	// Use this for initialization
	void Start ()
    {
        Animation = GetComponent<Animation>();
	}

	// Update is called once per frame
	void Update () {
	}

	void OnGUI()
	{
        if (!Animation)
            return;
		int id = 0;
        foreach (AnimationState state in Animation) 
		{
			if(GUI.Button(new Rect(0, id*20,60,20), new GUIContent(state.clip.name)))
			{
                Animation.Play(state.clip.name);
			}
			if(Input.GetKey(KeyCode.Alpha1 + id))
			{
				Animation.Play(state.clip.name);
			}
			if(Input.GetKey(KeyCode.J) && state.clip.name == "attackA")
			{
				Animation.Play(state.clip.name);
			}
			if(Input.GetKey(KeyCode.K) && state.clip.name == "attackB")
			{
				Animation.Play(state.clip.name);
			}
			if(Input.GetKey(KeyCode.L) && state.clip.name == "attackC")
			{
				Animation.Play(state.clip.name);
			}
			if(Input.GetKey(KeyCode.U) && state.clip.name == "attackD")
			{
				Animation.Play(state.clip.name);
			}
			id ++;
		}
	}
}