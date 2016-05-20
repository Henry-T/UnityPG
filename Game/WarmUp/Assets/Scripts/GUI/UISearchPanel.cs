using UnityEngine;
using System.Collections.Generic;
using AVOSCloud;
using System.Threading.Tasks;

public class UISearchPanel : MonoBehaviour {

	public AVUser CurMatchUser = null;
	public AVObject CurMatchVillage = null;

	public UILabel lbMatchBeliefAll;
	public UILabel lbMatchUserName;
	public UIButton btnFight;

	// Use this for initialization
	void Start () {
		lbMatchBeliefAll = transform.FindChild("lbMatchBeliefAll").GetComponent<UILabel>();
		lbMatchUserName = transform.FindChild("lbMatchUserName").GetComponent<UILabel>();
		btnFight = transform.FindChild("btnFight").GetComponent<UIButton>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Search()
	{
		var query = new AVQuery<AVObject>("Village").WhereNotEqualTo("UserID", AVUser.CurrentUser.ObjectId);
		query.FindAsync().ContinueWith(t=>{
			List<AVObject> objList = new List<AVObject>();
			objList.AddRange((t as Task<IEnumerable<AVObject>>).Result);
			if(objList.Count > 0)
			{
				var rand = new System.Random();
				int r = rand.Next(objList.Count);
				AVObject villageObject = objList[r];
				AVUser.Query.GetAsync(villageObject.Get<string>("UserID")).ContinueWith(t2=>{
					AVUser user = (t2 as Task<AVUser>).Result;
					GameManager.Instance.LastMatchUser = user;
					GameManager.Instance.LastMathVillage = villageObject;
					GameManager.Instance.EventQueue.Queue.Enqueue(new EventItem(){Type = EEventItemType.MatchFound});
				});
			}
		});
	}

	public void Fight()
	{
		var query = new AVQuery<AVObject>("Building").WhereEqualTo("UserID", CurMatchUser.ObjectId);
		query.FindAsync().ContinueWith(t=>{
			List<BuildingData> buildingDataList = new List<BuildingData>();
			foreach(AVObject buildingObject in (t as Task<IEnumerable<AVObject>>).Result)
			{
				BuildingData buildingData = new BuildingData();
				buildingData.UserID = buildingObject.Get<string>("UserID");
				buildingData.Type = (EBuildingType)buildingObject.Get<int>("Type");
				buildingData.Level = buildingObject.Get<int>("Level");
				buildingData.Value = buildingObject.Get<int>("Value");
				buildingData.SlotID = buildingObject.Get<int>("SlotID");
				buildingDataList.Add(buildingData);
			}
			
			GameManager.Instance.LastMatchBuildingDataList = buildingDataList;
            GameManager.Instance.EventQueue.Queue.Enqueue(new EventItem(){Type = EEventItemType.EnemyBuildingLoaded});
        });
    }
    
	public void SetMatch(AVUser user, AVObject villageObject)
	{
		CurMatchUser = user;
		CurMatchVillage = villageObject;

		lbMatchBeliefAll.text = villageObject.Get<int>("BeliefAll").ToString();
		lbMatchUserName.text = user.Username;

		btnFight.gameObject.SetActive(true);
	}
}
