using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PlayAnim))]
public class AnimCtrlEditor : Editor {
	public override void OnInspectorGUI(){
		DrawDefaultInspector();

		PlayAnim animCtrl = (PlayAnim)target;
		if(GUILayout.Button("Play Idle")){

		}

	}
}