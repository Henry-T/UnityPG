using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerControl : MonoBehaviour {
	
	private int serverPort = 10000;
	private string serverIP = "127.0.0.1";
	private bool useNAT = false;
	private int limitUserCount = 10;

	void OnGUI() {

		switch (Network.peerType) {

		case NetworkPeerType.Disconnected:
			CreateServer();
			break;
		case NetworkPeerType.Server:
			OnServer();
			break;
		case NetworkPeerType.Client:
			OnClient();
			break;
		case NetworkPeerType.Connecting:
			GUILayout.Label("Connecting");
			break;
		}
	}

	void CreateServer() {

		GUILayout.BeginVertical();

		if (GUILayout.Button("Create Server")) {

			NetworkConnectionError error = Network.InitializeServer(limitUserCount, serverPort, useNAT);

			Debug.Log(error);
		}

		if (GUILayout.Button("Connect To Server")) {
			
			NetworkConnectionError error = Network.Connect(serverIP, serverPort);
			
			Debug.Log(error);
		}

		GUILayout.EndVertical();
	}

	void OnServer() {

		GUILayout.Label("Server created, wait for client....");

		int length = Network.connections.Length;

		for (int i = 0; i < length; i++) {

			GUILayout.Label("client ip : " + Network.connections[i].ipAddress);
			GUILayout.Label("client port : " + Network.connections[i].port);
		}

		if (GUILayout.Button("Disconnect")) {

			Network.Disconnect();
		}
	}

	void OnClient() {
		
		GUILayout.Label("Connect successed!");
		
		if (GUILayout.Button("Disconnect")) {
			
			Network.Disconnect();
		}
	}
}
