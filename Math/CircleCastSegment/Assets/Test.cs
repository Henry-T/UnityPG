using UnityEngine;
using System.Collections.Generic;

public class Test : MonoBehaviour {

	public GameObject From;
	public GameObject To;
	public List<Segment> Segments = new List<Segment>();

	void Start () {
		GameObject[] segs = GameObject.FindGameObjectsWithTag("Segment");
		foreach(GameObject seg in segs)
		{
			Segment newSeg = new Segment();
			newSeg.Start 	= seg.transform.FindChild("Start");
			newSeg.End 		= seg.transform.FindChild("End");
		}
	}

	void Update () {
		
	}

	void check()
	{
		float hitDistance = 9999;
		CircleHitInfo hitInfo = null;
		bool anyHit = false;
		foreach(Segment seg in Segments)
		{
			CircleHitInfo _hitInfo = null;
			if(CircleCastSegment(From.transform.position, 0.5f, To.transform.position, seg.Start.position, seg.End.position, out _hitInfo ))
			{
				anyHit = true;
				float _distance = Vector3.Distance(From.transform.position, _hitInfo.CenterPoint);
				if(_distance < hitDistance)
				{
					hitDistance = _distance;
					hitInfo = _hitInfo;
				}
			}
		}
	}

	public static bool CircleCastSegment(Vector3 orgCenterStart, float orgRaduis, Vector3 orgCenterEnd, Vector3 segmentStart, Vector3 segmentEnd, out CircleHitInfo hitInfo)
	{
		hitInfo = null;
		// 必须圆线相切才算碰撞，即使线段端点在圆内部也不一定是碰撞
		float saveY = orgCenterStart.y;
		orgCenterStart.y = orgCenterEnd.y = segmentStart.y = segmentEnd.y = 0;
		Quaternion quat = Quaternion.LookRotation(orgCenterEnd - orgCenterStart);
		Quaternion quatInverce = Quaternion.Inverse(quat);
		Vector3 _orgCenterStart = quatInverce * (orgCenterStart - orgCenterStart);
		Vector3 _orgCenterEnd = quatInverce * (orgCenterEnd - orgCenterStart);
		Vector3 _segmentStart = quatInverce * (segmentStart - orgCenterStart);
		Vector3 _segmentEnd = quatInverce * (segmentEnd - orgCenterStart);

		// 交换线段端点，便于计算
		if(_segmentStart.z > _segmentEnd.z)
		{
			Vector3 temp = _segmentStart;
			_segmentEnd = _segmentStart;
			_segmentStart = temp;
		}

		// 情况X  已经碰撞/已经处于碰撞相对位置
		if(Mathf.Abs(_segmentStart.x) < orgRaduis)
		{
			return false;
		}

		// 情况一 平行 绝对无碰撞
		if(_segmentStart.x == _segmentEnd.x)
		{
			return false;
		}

		// 情况二 远离 在球运动方向上 垂线越来越长 (间隔变大) 无碰撞
		if(_segmentStart.x * _segmentEnd.x > 0 && Mathf.Abs(_segmentEnd.x) > Mathf.Abs(_segmentStart.x))
		{
			return false;
		}

		// 情况三 靠近 在球运动方向上 垂线越来越短 (间隔变小) 可能碰撞 <else of above>

		// 计算向量 SegS -> Segment与Z轴交点
		Vector3 SegDir = (_segmentEnd - _segmentStart).normalized;
		Vector3 A = SegDir * SegDir.x / _segmentStart.x;
		float ALen = A.magnitude;

		// 计算切位圆心到两线交点向量
		Vector3 P = A - new Vector3(-_segmentStart.x, 0, 0);		// SegmentStart垂直Z轴垂点到两线交点向量
		Vector3 PwEnd = Vector3.forward * ALen * orgRaduis / Mathf.Abs(_segmentStart.x);
		Vector3 origin = P - PwEnd;

		if(0>origin.z || origin.z>_orgCenterEnd.z)
		{
			return false;
		}
	
		Vector3 PB = A.normalized * PwEnd.magnitude * Mathf.Abs(_segmentStart.x) / orgRaduis;

		Vector3 Vx = A - PB;

		Vector3 touch = _segmentStart + Vx;


		// 写返回值
		hitInfo = new CircleHitInfo();
		hitInfo.CenterPoint = quat * origin + orgCenterStart;
		hitInfo.TouchPoint = quat * touch + orgCenterStart;
		hitInfo.Normal = (hitInfo.TouchPoint - hitInfo.CenterPoint).normalized;

		return true;
	}
}

public class CircleHitInfo
{
	public Vector3 CenterPoint;     // 命中时圆心
	public Vector3 TouchPoint;      // 命中时切点
	public Vector3 Normal;          // 命中时法线
}

public class Segment
{
	public Transform Start;
	public Transform End;
}


