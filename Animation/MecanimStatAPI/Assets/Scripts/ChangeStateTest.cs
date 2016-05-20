using UnityEngine;
using System.Collections;

public class ChangeStateTest : MonoBehaviour {

    public Animator Animator;
    private bool startRun;

    private string nameOfIdle = "Base Layer.Idle";
    private string nameOfRun = "Base Layer.Run";

	void Start () {
	
	}
	
	void Update () {
        if (startRun)
            Animator.Play(nameOfRun);
        else
            Animator.Play(nameOfIdle);
	}

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 50, 100, 40), new GUIContent("Run")))
        {
            startRun = !startRun;
        }
    }
}
