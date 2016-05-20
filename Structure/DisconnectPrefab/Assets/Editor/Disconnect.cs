using UnityEngine;
using System.Collections;
using UnityEditor;

public class DisconnnectHelper {

	[MenuItem("Tool/断开Prefab")]
	public static void Disconnect()
	{
		PrefabUtility.DisconnectPrefabInstance(Selection.activeGameObject);
	}
}
