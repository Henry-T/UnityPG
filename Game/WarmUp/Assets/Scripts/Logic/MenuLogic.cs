using UnityEngine;
using System.Collections.Generic;

public class MenuLogic : MonoBehaviour {

	public EScreen CurScreen = EScreen.Login;
	public Dictionary<EScreen, List<GameObject>> ScreenAgentMap;
	
	GameObject menuAgent;
	GameObject loginAgent;
	GameObject buildAgent;
	GameObject searchAgent;
	GameObject fightAgent;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
	}
}
