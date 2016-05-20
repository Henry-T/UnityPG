using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GuideSegment))]
public class GuideSegmentEditor : Editor {

	public GuideSegment Target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Target = target as GuideSegment;
	}
		
	void OnSceneGUI()
	{
		if(!Target)
			Target = (GuideSegment)target;

		if(Target.Obj1 && Target.Obj2)
		{
			Vector3 pos1 = Target.Obj1.transform.position;
			Vector3 pos2 = Target.Obj2.transform.position;
			Handles.DrawLine(pos1, pos2);
			Handles.Label((pos1 + pos2)*0.5f, Vector3.Distance(pos1, pos2).ToString("F"));

		}
	}
}
