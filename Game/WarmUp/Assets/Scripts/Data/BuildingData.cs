using UnityEngine;
using System.Collections;
using AVOSCloud;

public class BuildingData
{
	public string UserID;		// 用户ID

	public EBuildingType Type;	// 类型
	public int Level;			// 等级
	public int Value;			// 主属性
	public int SlotID;			// 位置

	public static void DB_CreateBuilding(int slotId, EBuildingType type)
	{
		AVObject obj = new AVObject("Building");
		obj["UserID"] = AVUser.CurrentUser.ObjectId;
		obj["Type"] = (int)type;
		obj["Level"] = 1;
		obj["Value"] = 1;
		obj["SlotID"] = slotId;

		obj.SaveAsync().ContinueWith(t => {
			BuildingData newBuilding = new BuildingData();
			newBuilding.UserID = AVUser.CurrentUser.ObjectId;
			newBuilding.Type = type;
			newBuilding.Level = 1;
			newBuilding.Value = 1;
			newBuilding.SlotID = slotId;

			GameManager.Instance.LastCreatedBuilding = newBuilding;
			GameManager.Instance.EventQueue.Queue.Enqueue(new EventItem(){Type = EEventItemType.BuildCommandOK});
		});
	}
}


public enum EBuildingType
{
	Defence,		// 守护
	Power,			// 力量
	Trick,			// 诡术
}
