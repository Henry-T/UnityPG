using UnityEngine;
using System.Collections.Generic;

public class EventQueue 
{
	public Queue<EventItem> Queue = new Queue<EventItem>();
}

public enum EEventItemType
{
	RegistSuccess,				// 注册成功
	CreateVillageSuccess,		// 创建村落成功
	LoginSuccess,				// 登陆成功
	LoginFailed,				// 登陆失败 - 唯一的网络验证
	PlayerVillageDataLoaded,	// 玩家村落数据已加载
	MatchFound,					// 找到敌对玩家
	EnemyBuildingLoaded,		// 敌对玩家建筑已加载
	BuildCommandOK,				// 建造命令成功
	UpgradeCommandOK,			// 升级命令成功
	AttackCommandOK,			// 进攻命令成功
	SearchCommandOK,			// 索敌命令成功
	WinCommandOK,				// 胜利提交成功
	LoseCommandOK,				// 失败提交成功
}

public class EventItem
{
	public EEventItemType Type;		// 事件类型
	public bool Handled = false;	// 已处理
}