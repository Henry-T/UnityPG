using UnityEngine;
using System.Collections;

public class StopEmitTest : MonoBehaviour {

    public ParticleSystem Particle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if(GUI.Button(new Rect(0,0,100, 20), new GUIContent("停止Emit")))
        {
            Particle.enableEmission = false;
        }
    }
}
