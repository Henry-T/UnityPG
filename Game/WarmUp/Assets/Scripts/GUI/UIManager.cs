using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

	public static UIManager Instance {get; private set;}
	
	public EScreen CurScreen = EScreen.Login;
	public Dictionary<EScreen, List<MonoBehaviour>> ScreenAgentMap = new Dictionary<EScreen, List<MonoBehaviour>>();
	
	public UIRegistPanel UIRegistPanel;
	public UILoginPanel UILoginPanel;
	public UIBuildPanel UIBuildPanel;
	public UISearchPanel UISearchPanel;
	public UIFightPanel UIFightPanel;

	void Start () {
		Instance = this;
		
		UIRegistPanel = transform.FindChild("UIRegistPanel").GetComponent<UIRegistPanel>();
		UILoginPanel = transform.FindChild("UILoginPanel").GetComponent<UILoginPanel>();
		UIBuildPanel = transform.FindChild("UIBuildPanel").GetComponent<UIBuildPanel>();
		UISearchPanel = transform.FindChild("UISearchPanel").GetComponent<UISearchPanel>();
		UIFightPanel = transform.FindChild("UIFightPanel").GetComponent<UIFightPanel>();
		
		ScreenAgentMap.Add(EScreen.Regist, new List<MonoBehaviour>(){UIRegistPanel});
		ScreenAgentMap.Add(EScreen.Login, new List<MonoBehaviour>(){UILoginPanel});
		ScreenAgentMap.Add(EScreen.Build, new List<MonoBehaviour>(){UIBuildPanel});
		ScreenAgentMap.Add(EScreen.Search, new List<MonoBehaviour>(){UISearchPanel});
		ScreenAgentMap.Add(EScreen.Fight, new List<MonoBehaviour>(){UIFightPanel});
		
		ChangeScreen(EScreen.Login);
    }
    
    void Update () {
	
	}

	public void ChangeScreen(EScreen screen)
	{
		HideAllScreen();
		foreach(MonoBehaviour agent in ScreenAgentMap[screen])
		{
			agent.gameObject.SetActive(true);

			if(screen== EScreen.Search && UISearchPanel.btnFight)
				UISearchPanel.btnFight.gameObject.SetActive(false);
		}
	}
	
	void HideAllScreen()
	{
		UIRegistPanel.gameObject.SetActive(false);
		UILoginPanel.gameObject.SetActive(false);
		UIBuildPanel.gameObject.SetActive(false);
		UISearchPanel.gameObject.SetActive(false);
		UIFightPanel.gameObject.SetActive(false);
    }
}


public enum EScreen
{
	Regist,
	Login,
	Build,
	Search,
	Fight,
}

