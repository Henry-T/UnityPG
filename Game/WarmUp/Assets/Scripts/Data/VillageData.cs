using UnityEngine;
using System.Collections.Generic;
using AVOSCloud;
using System.Threading.Tasks;

public class VillageData
{
	public int BUILDING_SLOTS = 15;

	public Dictionary<int, BuildingData> BuildingDic = new Dictionary<int, BuildingData>();

	public string UserID;	// 用户ID

	public int Defence;		// 守护
	public int Power;		// 力量
	public int Trick;		// 诡术
	public int Belief;		// 信仰
	public int BeliefAll;	// 累计信仰

	public VillageData()
	{

	}

	public static VillageData CreateFromAVObject(AVObject obj)
	{
		VillageData villageData = new VillageData();
		villageData.UserID = obj.Get<string>("UserID");
		villageData.Defence = obj.Get<int>("Defence");
		villageData.Power = obj.Get<int>("Power");
		villageData.Trick = obj.Get<int>("Trick");
		villageData.Belief = obj.Get<int>("Belief");
		villageData.BeliefAll = obj.Get<int>("BeliefAll");
		return villageData;
	}

	public int GetEmptySlot()
	{
		for(int i = 0; i < BUILDING_SLOTS; i++)
		{
			if(!BuildingDic.ContainsKey(i))
				return i;
		}
		return -1;
	}

	public void AddNewBuilding(BuildingData buildingData)
	{
		BuildingDic.Add(buildingData.SlotID, buildingData);
	}

	public static void DB_CreateVillage()
	{
		AVObject obj = new AVObject("Village");
		obj["UserID"] = AVUser.CurrentUser.ObjectId;
		obj["Defence"] = 10;
		obj["Power"] = 10;
		obj["Trick"] = 0;
		obj["Belief"] = 0;
		obj["BeliefAll"] = 0;
		
		obj.SaveAsync().ContinueWith(t => {
			GameManager.Instance.EventQueue.Queue.Enqueue(new EventItem(){Type = EEventItemType.CreateVillageSuccess});
		});
	}

	public static void DB_QueryPlayerVillageData()
	{
		AVQuery<AVObject> query=new AVQuery<AVObject>("Village").WhereEqualTo("UserID", AVUser.CurrentUser.ObjectId);
		query.FirstAsync().ContinueWith(t =>{
			AVObject villageObject = (t as Task<AVObject>).Result;
			VillageData villageData = new VillageData();
			villageData.UserID = villageObject.Get<string>("UserID");
			villageData.Defence = villageObject.Get<int>("Defence");
			villageData.Power = villageObject.Get<int>("Power");
			villageData.Trick = villageObject.Get<int>("Trick");
			villageData.Belief = villageObject.Get<int>("Belief");
			villageData.BeliefAll = villageObject.Get<int>("BeliefAll");
			GameManager.Instance.LastGetVillageData = villageData;
			Debug.LogWarning(villageData.UserID);

			AVQuery<AVObject> buildingQuery = new AVQuery<AVObject>("Building").WhereEqualTo("UserID", AVUser.CurrentUser.ObjectId);
			buildingQuery.FindAsync().ContinueWith(t2=>{
				List<BuildingData> buildingDataList = new List<BuildingData>();
				foreach(AVObject buildingObject in (t2 as Task<IEnumerable<AVObject>>).Result)
				{
					BuildingData buildingData = new BuildingData();
					buildingData.UserID = buildingObject.Get<string>("UserID");
					buildingData.Type = (EBuildingType)buildingObject.Get<int>("Type");
					buildingData.Level = buildingObject.Get<int>("Level");
					buildingData.Value = buildingObject.Get<int>("Value");
					buildingData.SlotID = buildingObject.Get<int>("SlotID");
					buildingDataList.Add(buildingData);
				}

				GameManager.Instance.LastGetBuildingDataList = buildingDataList;
				GameManager.Instance.EventQueue.Queue.Enqueue(new EventItem(){Type = EEventItemType.PlayerVillageDataLoaded});
			});
		});
	}
}
