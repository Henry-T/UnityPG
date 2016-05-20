using UnityEngine;
using System.Collections;

public class CameraRotater : MonoBehaviour {
	
	public float RotateSpeed = 30;
	public float Distance = 2;
	public float Height = 1;
	private Camera Camera;
	private float angle;
	void Start () {
		Camera = GetComponent<Camera>();
		angle = 0;
	}
	
	// Update is called once per frame
	void Update () {
		angle += RotateSpeed * Time.deltaTime;

		//if (Camera != null)
		{
			Camera.transform.position = new Vector3(Mathf.Cos(angle*Mathf.PI/180)*Distance, Height, Mathf.Sin(angle*Mathf.PI/180)*Distance);
			Camera.transform.LookAt(Vector3.zero);
		}
	}
	
	void OnGUI()
	{
        GUILayout.BeginArea (new Rect (Screen.width - 150, 0, 50, 300));
        GUILayout.Label (new GUIContent ("旋转"));
        GUILayout.Label (new GUIContent ("距离"));
        GUILayout.Label (new GUIContent ("高度"));
        GUILayout.EndArea ();

        GUILayout.BeginArea(new Rect(Screen.width-100, 0, 100, 300));
		RotateSpeed = GUILayout.HorizontalSlider (RotateSpeed, 0, 360);
        Distance = GUILayout.HorizontalSlider (Distance, 1, 10);
        Height = GUILayout.HorizontalSlider (Height, 0, 6);
        GUILayout.EndArea();
	}
}
