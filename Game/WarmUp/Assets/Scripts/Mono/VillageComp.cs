using UnityEngine;
using System.Collections.Generic;
using AVOSCloud;

public class VillageComp : MonoBehaviour {

	public EffectRootComp EffectRootComp;
	public SensorGroupComp SensorGroupComp;
	public BuildingGroupComp BuildingGroupComp;
	public EnemyGroupComp EnemyGroupComp;

	public bool IsPlayerVillage = true;
	public AVUser CurUser;
	public VillageData CurVillageData;
	public List<BuildingData> CurBuildingDataList;

	// Use this for initialization
	void Start () {
		EffectRootComp = GetComponentInChildren<EffectRootComp>();
		SensorGroupComp = GetComponentInChildren<SensorGroupComp>();
		BuildingGroupComp = GetComponentInChildren<BuildingGroupComp>();
		EnemyGroupComp = GetComponentInChildren<EnemyGroupComp>();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void SetVillageTo(AVUser user, VillageData villageData, List<BuildingData> buildingDataList)
	{
		CurUser = user;
		CurVillageData = villageData;
		CurBuildingDataList = buildingDataList;

		if(user != AVUser.CurrentUser)
			IsPlayerVillage = false;

		BuildingGroupComp.ReloadAllBuildings(buildingDataList);
	}
}
