using UnityEngine;
using System.Collections;

public class CreateTeddy : MonoBehaviour {

	public Transform playerPrefab;

	private IList list;
	
	void Start () {
	
		list = new ArrayList();
	}

	void OnServerInitialized() {

		MovePlayer(Network.player);
	}

	//Server side
	void OnPlayerConnected(NetworkPlayer player) {

		MovePlayer(player);
	}
	
	void MovePlayer(NetworkPlayer player) {

		int playerID = int.Parse(player.ToString());

		Transform playerTransform = (Transform)Network.Instantiate(playerPrefab, transform.position, transform.rotation, playerID);

		NetworkView playerObjNetWorkView = playerTransform.networkView;

		list.Add(playerTransform.GetComponent("PlayerControl"));

		playerObjNetWorkView.RPC("SetPlayer", RPCMode.AllBuffered, player);
	}
	
	void OnPlayerDisconnected(NetworkPlayer player) {

		Debug.Log("Clean up after player " + player);

		foreach (PlayerControl script in list) {

			Network.RemoveRPCs(script.gameObject.networkView.viewID);

			Network.Destroy(script.gameObject);

			list.Remove(script);

			break;
		}

		int playerNumber = int.Parse(player + "");
		Network.RemoveRPCs(Network.player, playerNumber);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) {

		Application.LoadLevel(Application.loadedLevel);
	}
}
