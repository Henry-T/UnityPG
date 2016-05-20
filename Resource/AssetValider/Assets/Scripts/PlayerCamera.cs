using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PlayerCamera : MonoBehaviour 
{
	public Transform Target;
	private Vector3 targetPos
	{
		get {return Target.transform.position + Vector3.up * 1.8f;}
	}

	public float RotX;
	public float RotY;
	public float Distance;

	void Start () {
		if(Target)
		{
			transform.LookAt(targetPos);
		}
		RotX = transform.rotation.eulerAngles.x;
		RotY = transform.rotation.eulerAngles.y;
		Distance = Vector3.Distance(transform.position, targetPos);

		// 初始值
		//RotX = 45;
		//RotY = 0;
		//Distance = 8;
	}

	void Update () {
		if(Target == null)
			return;

		Vector3 camPos = new Vector3(0, 0, -Distance);
		Quaternion posQuat = Quaternion.Euler(RotX, RotY, 0);
		camPos += targetPos;
		camPos = Target.TransformPoint(camPos);

		Vector3 position = targetPos - posQuat * Vector3.forward * Distance;

		transform.position = position;
		transform.rotation = posQuat;
	}
}
