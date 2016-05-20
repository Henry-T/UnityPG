using UnityEngine;
using System.Collections;
using GSDKUnityLib;
using System;
using GSDKUnityLib.Pay;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GSDK.Instance.Initialize();
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            onLogin();
        } 
	}

    void OnGUI()
    {
        if(GUILayout.Button("登陆"))
        {
            onLogin();
        }

        if(GUILayout.Button("支付"))
        {
            onPay();
        }

        if (GUILayout.Button("母包更新"))
        {
            onMainUpgrade();
        }
    }

    void onLogin()
    {
        GSDK.Instance.StartLoginAsnyc((s) => {
            Debug.Log("登陆完成");
            Debug.Log("状态:"+s.Status);
            Debug.Log("状态码:" + (int)s.Status);
            Debug.Log("UID:" + s.AccountInfo.UID);
        });
    }

    void onPay()
    {
        PayInfo payInfo = new PayInfo();
        payInfo.PropID = "0001";
        payInfo.PriceInCurrency = 100;
        payInfo.PropName = "10个钻石(1元)";
        GSDK.Instance.StartPayAsync(payInfo, (payResult) => {
            if (payResult.Status == EPayResultStatus.Succeed)
            {
                // 支付成功后，可以向服务器验证此订单(也可以直接发货)
                // 建议此处将订单存入本地文件，若游戏闪退，可用验证接口自动补单
                GSDK.Instance.StartCheckOrderAsync(payResult.OrderIDStr, checkOrderResult =>{
                    if (checkOrderResult.Status == GSDKUnityLib.Pay.CheckOrder.ECheckOrderResultStatus.Paid)
                        Debug.Log("验证成功，发货: " + payInfo.PropID);
                    else
                        Debug.LogWarning(string.Format("验证失败 {0} {1}", checkOrderResult.Status, (int)checkOrderResult.Status));
                });
            }
            else if (payResult.Status == EPayResultStatus.Cancelled)
            {
                Debug.Log("玩家放弃支付了");
            }
            else
            {
                Debug.Log(string.Format("支付失败 {0} {1} {2}", payResult.Status, payResult.InfoStr, payResult.InternalErrorStr));
            }
        });
    }

    void onMainUpgrade()
    {
        GSDK.Instance.StartMainUpgradeAsync("", result => {
            switch (result.Status)
            {
                case GSDKUnityLib.MainUpgrade.EMainUpgradeResultStatus.HasUpgrade:
                    Debug.Log("更新已经开始了(apk下载中或跳转到了应用商店页面)，此时可禁止玩家输入");
                    break;
                case GSDKUnityLib.MainUpgrade.EMainUpgradeResultStatus.NoUpgrade:
                    Debug.Log("没有更新，继续游戏");
                    break;
                case GSDKUnityLib.MainUpgrade.EMainUpgradeResultStatus.UpgradeFailed:
                    Debug.Log("更新失败了，可提示玩家自行下载。如果此更新为强制更新，可在提示后强行关闭游戏");
                    break;
                default:
                    Debug.Log("其他错误状态都是给开发者看的，给玩家一个泛泛的错误提示即可。");
                    break;
            }
        });
    }
}
