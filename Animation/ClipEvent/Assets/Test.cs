using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    public Animator LeftAnimator;
    public Animator RightAnimator;

    void OnGUI()
    {
        if(GUI.Button(new Rect(0,0, 100, 40), new GUIContent("左边Fire")))
        {
            LeftAnimator.Play("Base Layer.Fire");
        }

        if (GUI.Button(new Rect(0, 50, 100, 40), new GUIContent("左边Fire")))
        {
            RightAnimator.Play("Base Layer.Fire");
        }
    }
}
