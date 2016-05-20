using UnityEngine;
using System.Collections;

public class UILoginPanel : MonoBehaviour {

	public UIInput tbUserName;
	public UIInput tbPassword;

	// Use this for initialization
	void Start () {
		tbUserName = transform.FindChild("tbUserName").GetComponent<UIInput>();
		tbPassword = transform.FindChild("tbPassword").GetComponent<UIInput>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DoRegist()
	{
		UIManager.Instance.ChangeScreen(EScreen.Regist);
	}

	public void DoLogin()
	{		
		GameManager.Instance.DoLogin(tbUserName.label.text, tbPassword.label.text, ()=>{
			GameManager.Instance.EventQueue.Queue.Enqueue(new EventItem(){Type= EEventItemType.LoginSuccess});
		});
	}
}
