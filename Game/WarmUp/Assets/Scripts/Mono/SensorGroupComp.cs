using UnityEngine;
using System.Collections.Generic;

public class SensorGroupComp : MonoBehaviour {

	public Dictionary<int, SensorComp> SensorCompDic;

	// Use this for initialization
	void Start () {
		SensorCompDic = new Dictionary<int, SensorComp>();
		SensorComp[] sensorComps = GetComponentsInChildren<SensorComp>();
		foreach(SensorComp sensorComp in sensorComps)
		{
			SensorCompDic.Add(sensorComp.SlotId, sensorComp);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetMouseButtonDown(0)){
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			int sensorMask = LayerMask.GetMask(new string[]{"sensor"});

			if(Physics.Raycast(ray, out hit,1000, sensorMask))
			{
				int hitSlot = hit.collider.transform.parent.GetComponent<SensorComp>().SlotId;
				Debug.LogWarning("点中 " + hitSlot);
				GameObject prefab = Resources.Load<GameObject>("Effects/lightningBoltBase");
				GameObject lightning = Instantiate(prefab) as GameObject;
				lightning.transform.parent = GameManager.Instance.VillageComp.EffectRootComp.transform;
				lightning.transform.position = new Vector3(hit.point.x, 2.05f, hit.point.z);

				EnemyComp enemyComp = GameManager.Instance.VillageComp.EnemyGroupComp.EnemyComp;
				if(GameManager.Instance.IsFighting && enemyComp.CurSlot == hitSlot && enemyComp.State == EnemyComp.EEnemyState.Show)
				{
					GameManager.Instance.HitCount ++;
					if(GameManager.Instance.HitCount >= 3)
					{
						GameManager.Instance.EndFight();
					}
				}
			}
		}
	}
}
