using UnityEngine;
using System.Collections;

public class Math
{
	public static Vector3 BounceVector(Vector3 input, Vector3 normal)
	{
		Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(normal), Vector3.one);
		Quaternion q1 = Quaternion.LookRotation(normal);
		Vector3 temp = Quaternion.Inverse(q1) * input;
		temp.z = -temp.z;
		temp = q1 * temp;
		return temp;
	}
}
