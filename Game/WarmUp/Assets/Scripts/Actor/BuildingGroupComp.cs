using UnityEngine;
using System.Collections.Generic;

public class BuildingGroupComp : MonoBehaviour {

	public List<BuildingComp> BuildingCompList = new List<BuildingComp>();

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void AddNewBuilding(BuildingData newBuildingData)
	{
		string prefabPath = "Building/Building_" + newBuildingData.Type.ToString() + "_" + newBuildingData.Level.ToString("D2");
		GameObject prefab = Resources.Load<GameObject>(prefabPath);
		GameObject newBuildingGO = GameObject.Instantiate(prefab) as GameObject;
		newBuildingGO.transform.parent = transform;
		BuildingComp buildingComp = newBuildingGO.GetComponent<BuildingComp>();
		buildingComp.Data = newBuildingData;
		BuildingCompList.Add(buildingComp);
		
		RepositionBuilding(buildingComp);
	}

	public void ReloadAllBuildings(List<BuildingData> buildingDataList)
	{
		foreach(BuildingComp buildingComp in BuildingCompList)
		{
			if(buildingComp)
				Destroy(buildingComp.gameObject);
		}

		BuildingCompList.Clear();

		foreach(BuildingData buildingData in buildingDataList)
		{
			AddNewBuilding(buildingData);
		}
	}
	
	public void RepositionBuilding(BuildingComp building)
	{
		SensorComp sensorComp = GameManager.Instance.VillageComp.SensorGroupComp.SensorCompDic[building.Data.SlotID];
		building.transform.position = sensorComp.transform.position;
	}
}
