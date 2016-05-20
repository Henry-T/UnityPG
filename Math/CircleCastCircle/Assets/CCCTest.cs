using UnityEngine;
using System.Collections;

public class CCCTest : MonoBehaviour {

	public GameObject from;
	public GameObject to;
	public GameObject marker;
	public GameObject stop;

	// Use this for initialization
	void Start () {
		from = GameObject.Find("From");
		to = GameObject.Find("To");
		marker = GameObject.Find("Marker");
		stop = GameObject.Find("Stop");

		// 测试 1 随便一转
		Matrix4x4 m = Matrix4x4.TRS(new Vector3(10, 0,0), Quaternion.Euler(0, 90, 0), Vector3.one);
		Vector3 tp = m.MultiplyVector(new Vector3(0, 0, 10));
		Debug.Log("1: " + tp);

		// 测试 2 旋转的原
		m = Matrix4x4.TRS(new Vector3(0,0,0), Quaternion.LookRotation(Vector3.forward), Vector3.one);
		Debug.Log("2: " + m.MultiplyVector(new Vector3(0,0,10)));

		Debug.Log(Vector3.forward);

		// 测试 3 转换后反转
		m = Matrix4x4.TRS(new Vector3(0,0,0), Quaternion.LookRotation(Vector3.forward), Vector3.one);
		Debug.Log("3: " + m.inverse.MultiplyVector(m.MultiplyVector(new Vector3(0,0,10))));
	}

	void OnGUI()
	{
		/*
		if(GUILayout.Button("检测"))
		{
			check ();
		}
		if(GUILayout.Button("Maker走开!"))
		{
			marker.transform.position = new Vector3(10,10,10);
		}
		*/
	}

	void check()
	{
		CircleHitInfo hitInfo = null;
		if(CircleCastCircle(from.transform.position, 0.5f, to.transform.position, stop.transform.position, 0.5f, out hitInfo))
		{
			marker.transform.position = hitInfo.CenterPoint;
		}
		else
		{
			Debug.Log("Cast不到");
			marker.transform.position = new Vector3(10,10,10);
		}
	}

	float t = 0;
	void Update () {
		t += Time.deltaTime;
		if(t > 4)
			t-=4;
		float d = t / 4f;
		//to.transform.position = new Vector3(Mathf.Cos(d * Mathf.PI * 2) * 2.5f, 0, Mathf.Sin(d* Mathf.PI * 2) *2.5f);

		check ();
	}

	public static bool CircleCastCircle(Vector3 orgCenterStart, float orgRaduis, Vector3 orgCenterEnd, Vector3 testCenter, float testRadius, out CircleHitInfo hitInfo)
	{
		float saveY = orgCenterStart.y;
		orgCenterStart.y = orgCenterEnd.y = testCenter.y = 0;

		Quaternion quat = Quaternion.LookRotation(orgCenterEnd - orgCenterStart);
		Quaternion quatInverce = Quaternion.Inverse(quat);
		Vector3 _orgCenterStart = quatInverce * (orgCenterStart - orgCenterStart);
		Vector3 _orgCenterEnd = quatInverce * (orgCenterEnd - orgCenterStart);
		Vector3 _testCenter = quatInverce * (testCenter - orgCenterStart);
		
		float radiusSum = orgRaduis + testRadius;
		
		bool hit = false;
		if(_orgCenterStart.z <= _testCenter.z && _testCenter.z <= _orgCenterEnd.z && Mathf.Abs(_testCenter.x) <= radiusSum)
		{
			hit = true;
		}
		
		if(Vector3.Distance(_orgCenterStart, _testCenter) <= radiusSum && Vector3.Distance(_orgCenterEnd, _testCenter) <= radiusSum)
		{
			hit = true;
		}
		
		if(hit)
		{
			float d = Mathf.Sqrt(radiusSum * radiusSum - _testCenter.x * _testCenter.x);
			float z = 0;
			z = _testCenter.z - d;

			Vector3 origin = new Vector3(0, 0, z);
			origin = quat * origin + orgCenterStart;
			origin.y = saveY;
			
			Vector3 touchPos = origin + (testCenter - origin).normalized * orgRaduis / radiusSum;
			touchPos.y = saveY;
			
			hitInfo = new CircleHitInfo();
			hitInfo.CenterPoint = origin;
			hitInfo.TouchPoint = touchPos;
			hitInfo.Normal = (origin - touchPos).normalized;
			return true;
		}
		else
		{
			hitInfo = new CircleHitInfo();
			return false;
		}
	}
}

public class CircleHitInfo
{
	public Vector3 CenterPoint;     // 命中时圆心
	public Vector3 TouchPoint;      // 命中时切点
	public Vector3 Normal;          // 命中时法线
}

