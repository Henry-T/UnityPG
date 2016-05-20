using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

	public float Speed = 5;

	void Start () {
	
	}

	void Update () {

		Vector3 dir = Vector3.zero;
		bool anyKeyPressed = false;
		float rot = transform.rotation.eulerAngles.y;

		if(Input.GetKey(KeyCode.A))
		{
			dir.x = 1;
			anyKeyPressed = true;
		}
		if(Input.GetKey(KeyCode.W))
		{
			dir.z = 1;
			anyKeyPressed = true;
		}
		if(Input.GetKey(KeyCode.S))
		{
			dir.z = -1;
			anyKeyPressed = true;
		}
		if(Input.GetKey(KeyCode.D))
		{
			dir.x = -1;
			anyKeyPressed = true;
		}

		if(anyKeyPressed)
		{
			Vector3 camForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
			Vector3 cross = Vector3.Cross(camForward, dir);
			rot = Vector3.Angle(camForward, dir);
			if(cross.y > 0)
				rot = -rot;

			transform.rotation = Quaternion.Euler(0, rot, 0);
		}

		if(dir != Vector3.zero)
			animation.Play("run");
		else
			animation.Play("idle");

		Vector3 oldPos = transform.position;
		Vector3 newPos = transform.position;
		if(anyKeyPressed)
			newPos += transform.forward * Speed * Time.deltaTime;
		Ray ray = new Ray(new Vector3(newPos.x, newPos.y + 50, newPos.z), Vector3.down);
		RaycastHit hitInfo;
		bool ok = true;
		if(Physics.Raycast(ray, out hitInfo, 100, 1<<LayerMask.NameToLayer("path")))
		{
			newPos.y = hitInfo.point.y;


			Vector3 p1 = new Vector3(oldPos.x, oldPos.y + 0.3f, oldPos.z);
			Vector3 p2 = new Vector3(newPos.x, newPos.y + 0.3f, newPos.z);
			Ray rayFront = new Ray(p1, p2-p1);
			float d = Vector3.Distance(p1, p2);
			if(Physics.Raycast(rayFront, out hitInfo, d, 1<<LayerMask.NameToLayer("path")))
			{
				ok = false;
			}
		}
		else
		{
			ok = false;
		}

		if(ok)
		{
			Debug.DrawRay(ray.origin, ray.direction, Color.red);
			transform.position = newPos;
		}
		else
		{
			Debug.DrawRay(ray.origin, ray.direction, Color.green);
			transform.position = oldPos;
		}
	}
}
