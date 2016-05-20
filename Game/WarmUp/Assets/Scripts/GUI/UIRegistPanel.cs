using UnityEngine;
using System.Collections;

public class UIRegistPanel : MonoBehaviour {
	
	public UIInput tbUserName;
	public UIInput tbPassword;
	public UIInput tbNickName;
	public UIInput tbEMail;
	
	// Use this for initialization
	void Start () {
		tbUserName = transform.FindChild("tbUserName").GetComponent<UIInput>();
		tbPassword = transform.FindChild("tbPassword").GetComponent<UIInput>();
		tbEMail = transform.FindChild("tbEMail").GetComponent<UIInput>();
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void ChangeScreenToLogin()
	{
		UIManager.Instance.ChangeScreen(EScreen.Login);
	}
	
	public void DoRegist()
	{
		GameManager.Instance.DoRegist(tbUserName.label.text, tbPassword.label.text, tbEMail.label.text);
	}

}
