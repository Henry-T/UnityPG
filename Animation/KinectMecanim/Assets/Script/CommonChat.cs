using UnityEngine;
using System.Collections;

public class CommonChat : MonoBehaviour {

	//使用聊天功能
	public bool usingChat = false;
	//自定义skin
	public GUISkin skin;
	//显示聊天框
	public bool showChat = false;

	private string inputField = "";
	private Vector2 scrollPosition;
	private int width = 300;
	private int height = 180;
	private string playerName = "";
	private float lastUnfocusTime = 0;
	private Rect window;

	private IList playerList = new ArrayList();
	class PlayerNode {

		public string playerName = "";
		public NetworkPlayer networkPlayer;
	}

	private IList chatEntries = new ArrayList();
	class ChatEntry {

		public string name = "";
		public string text = "";
	}

	void Awake() {
		window = new Rect(0, Screen.height - height, width, height);

		playerName = PlayerPrefs.GetString("playerName", "");
		if (playerName != null || playerName == "") {
			playerName = "RandomName" + Random.Range(1,999);
		}
	}

	void OnGUI() {

		if (!showChat)
			return;

		GUI.skin = skin;

		if (Event.current.type == EventType.keyDown && Event.current.character == '\n' && inputField.Length <= 0) {

			if (lastUnfocusTime + 0.25 < Time.time) {
				usingChat = true;
				GUI.FocusWindow(5);
				GUI.FocusControl("Chat input field");

			}
		}

		window = GUI.Window(5, window, GlobalChatWindow, "");
	}

	//client side
	void OnConnectedToServer() {

		ShowChatWindow();
		networkView.RPC("TellServerOurName", RPCMode.Server, playerName);
	}

	//server side
	void OnServerInitialized() {

		ShowChatWindow();

		PlayerNode newNode = new PlayerNode();
		newNode.playerName = playerName;
		newNode.networkPlayer = Network.player;
		playerList.Add(newNode);

		addGameChatMessage(playerName + "joined the game.");
	}

	PlayerNode GetPlayerNode(NetworkPlayer player) {

		foreach (PlayerNode node in playerList) {

			if(node.networkPlayer == player) {
				return node;
			}
		}

		Debug.LogError("GetPlayerNode: Requested a playernode of non-existing player!");
		return null;
	}

	void OnPlayerDisconnected(NetworkPlayer player) {

		addGameChatMessage("Player disconnected from: " + player.ipAddress+":" + player.port);

		playerList.Remove(GetPlayerNode(player));
	}

	void OnDisconnectedFromServer() {

		CloseChatWindow();
	}

	void OnPlayerConnected(NetworkPlayer player) {

		addGameChatMessage("Player connected from: " + player.ipAddress +":" + player.port);
	}

	[RPC]
	void TellServerOurName(string name, NetworkMessageInfo info) {

		PlayerNode node = new PlayerNode();
		node.playerName = name;
		node.networkPlayer = info.sender;
		playerList.Add(node);

		addGameChatMessage(name+" joined the chat");
	}

	void CloseChatWindow() {

		showChat = false;
		inputField = "";
		chatEntries = new ArrayList();
	}

	void ShowChatWindow() {

		showChat = true;
		inputField = "";
		chatEntries = new ArrayList();
	}

	void GlobalChatWindow(int windowId) {

		GUILayout.BeginVertical();
		GUILayout.Space(10);
		GUILayout.EndVertical();
		
		// Begin a scroll view. All rects are calculated automatically - 
		// it will use up any available screen space and make sure contents flow correctly.
		// This is kept small with the last two parameters to force scrollbars to appear.
		scrollPosition = GUILayout.BeginScrollView (scrollPosition);

		foreach (ChatEntry item in chatEntries) {

			GUILayout.BeginHorizontal();
			if (item.name == "") {

				GUILayout.Label(item.text);
			}
			else {

				GUILayout.Label(item.name + ": " + item.text);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(3);
		}

		// End the scrollview we began above.
		GUILayout.EndScrollView ();
		
		if (Event.current.type == EventType.keyDown && Event.current.character == '\n' && inputField.Length >= 0)
		{
			HitEnter(inputField);
		}
		GUI.SetNextControlName("Chat input field");
		inputField = GUILayout.TextField(inputField);

		//bug
		if(Input.GetKeyDown("mouse 0")){

			if(usingChat){
				usingChat=false;
				GUI.UnfocusWindow ();//Deselect chat
				lastUnfocusTime=Time.time;
			}
		}
	}

	void HitEnter(string msg) {

		msg = msg.Replace("\n", "");
		//判断，如果没有输入，则不发送RPC消息
		if (msg.Length > 0) {
			networkView.RPC("ApplyGlobalChatText", RPCMode.All, playerName, msg);
		}
		inputField = ""; //Clear line
		GUI.UnfocusWindow ();//Deselect chat
		lastUnfocusTime=Time.time;
		usingChat=false;
	}

	[RPC]
	void ApplyGlobalChatText(string name, string msg) {

		ChatEntry entry = new ChatEntry();
		entry.name = name;
		entry.text = msg;
		
		chatEntries.Add(entry);
		
		//Remove old entries
		if (chatEntries.Count > 4){
			chatEntries.RemoveAt(0);
		}
		
		scrollPosition.y = 1000000;
	}

	void addGameChatMessage(string message) {
		ApplyGlobalChatText("", message);
		if(Network.connections.Length > 0){
			networkView.RPC("ApplyGlobalChatText", RPCMode.Others, "", message);	
		}
	}
}
