using UnityEngine;
using System.Collections.Generic;
using AVOSCloud;
using System;

public class GameManager : MonoBehaviour 
{
	public static GameManager Instance {get; private set;}
	public GameObject BuildingRoot;
	public VillageComp VillageComp;

	public UserLogic UserLogic;
	public MenuLogic MenuLogic;
	public LoginLogic LoginLogic;
	public BuildLogic BuildLogic;
	public SearchLogic SearchLogic;
	public FightLogic FightLogic;

	public bool IsLogin = false;
	public string CurUserName {
		get { if(AVUser.CurrentUser != null) return AVUser.CurrentUser.Username; return "";}
	}
	public string UID;

	public VillageData PlayerVillageData;
	public List<BuildingData> PlayerBuildingDataList;

	public bool IsFighting = false;

	public int HitCount = 0;

	// ==== 临时事件数据 ====
	public EventQueue EventQueue;
	public BuildingData LastCreatedBuilding;
	public VillageData LastGetVillageData;
	public List<BuildingData> LastGetBuildingDataList;
	public AVUser LastMatchUser;
	public AVObject LastMathVillage;
	public List<BuildingData> LastMatchBuildingDataList;

	// Use this for initialization
	void Start () {
		Instance = this;
		
		EventQueue = new EventQueue();

		string name = CurUserName;
		
		UserLogic = GameObject.Find("UserAgent").GetComponent<UserLogic>();
		MenuLogic = GameObject.Find("MenuAgent").GetComponent<MenuLogic>();
		LoginLogic = GameObject.Find("LoginAgent").GetComponent<LoginLogic>();
		BuildLogic = GameObject.Find("BuildAgent").GetComponent<BuildLogic>();
		SearchLogic = GameObject.Find("SearchAgent").GetComponent<SearchLogic>();
		FightLogic = GameObject.Find("FightAgent").GetComponent<FightLogic>();

	}
	
	// Update is called once per frame
	void Update () {
		// TODO 不断获取事件队列，并逐个处理事件
		// 避免线程访问冲突，事件不会被删除，仅标记为已处理

		while(EventQueue.Queue.Count > 0)
		{
			EventItem eventItem = EventQueue.Queue.Dequeue();
			HandleEventItem(eventItem);
		}
	}

	public void DoRegist(string userName, string password, string email)
	{
		AVUser user = new AVUser();
		user.Username = userName;
		user.Password = password;
		user.Email = email;
		user.SignUpAsync().ContinueWith(t => {
			UID = user.ObjectId;

			// 创建玩家的Village
			VillageData.DB_CreateVillage();
		});
	}

	public void DoLogin(string userName, string passWord, Action action)
	{
		AVUser.LogInAsync(userName, passWord).ContinueWith(t=>{
			if(t.IsFaulted || t.IsCanceled)
			{
				var error = t.Exception.Message;
			}
			else
			{
				IsLogin = true;

				if(action != null)
					action();
			}
		});
	}

	public void StartFight()
	{
		UIManager.Instance.ChangeScreen(EScreen.Fight);
		VillageComp.SetVillageTo(LastMatchUser, VillageData.CreateFromAVObject(LastMathVillage), LastMatchBuildingDataList);
		IsFighting = true;
		HitCount = 0;
		VillageComp.EnemyGroupComp.EnemyComp.Restart();
	}

	public void EndFight()
	{
		IsFighting = false;
		VillageComp.SetVillageTo(AVUser.CurrentUser, PlayerVillageData, PlayerBuildingDataList);
		UIManager.Instance.ChangeScreen(EScreen.Build);
	}

	public void HandleEventItem(EventItem item)
	{
		switch(item.Type)
		{
		case EEventItemType.LoginSuccess:
			VillageData.DB_QueryPlayerVillageData();
			break;
		case EEventItemType.PlayerVillageDataLoaded:
			PlayerVillageData = LastGetVillageData;
			PlayerVillageData.BuildingDic = new Dictionary<int, BuildingData>();
			foreach(BuildingData buildingData in LastGetBuildingDataList)
			{
				PlayerVillageData.BuildingDic.Add(buildingData.SlotID, buildingData);
			}
			PlayerBuildingDataList = LastGetBuildingDataList;
				VillageComp.SetVillageTo(AVUser.CurrentUser, PlayerVillageData, PlayerBuildingDataList);
			UIManager.Instance.ChangeScreen(EScreen.Build);
			break;
		case EEventItemType.BuildCommandOK:
			PlayerVillageData.AddNewBuilding(LastCreatedBuilding);
			VillageComp.BuildingGroupComp.AddNewBuilding(LastCreatedBuilding);
			break;

		case EEventItemType.MatchFound:
			UIManager.Instance.UISearchPanel.SetMatch(LastMatchUser, LastMathVillage);
			break;

		case EEventItemType.EnemyBuildingLoaded:
			StartFight();
			break;
		}
	}
}
