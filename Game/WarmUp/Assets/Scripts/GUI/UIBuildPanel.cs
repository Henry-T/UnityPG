using UnityEngine;
using System.Collections;

public class UIBuildPanel : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void QuickBuild_Defence()
	{
		BuildingData.DB_CreateBuilding(GameManager.Instance.PlayerVillageData.GetEmptySlot(), EBuildingType.Defence);
	}
	
	public void QuickBuild_Power()
	{
		BuildingData.DB_CreateBuilding(GameManager.Instance.PlayerVillageData.GetEmptySlot(), EBuildingType.Power);
	}

	public void QuickBuild_Trick()
	{
		BuildingData.DB_CreateBuilding(GameManager.Instance.PlayerVillageData.GetEmptySlot(), EBuildingType.Trick);
	}

	public void ChangeScreenToSearch()
	{
		UIManager.Instance.ChangeScreen(EScreen.Search);
	}
}
