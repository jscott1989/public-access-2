using UnityEngine;
using System.Collections;

public class Day1MorningManager : SceneManager {
	NetworkManager mNetworkManager;

	void Awake() {
		mNetworkManager = (NetworkManager)FindObjectOfType (typeof(NetworkManager));
	}

	void Start () {
		// First we need to set everyone to "Not Ready"
		foreach (Player player in mNetworkManager.players) {
			player.networkView.RPC ("SetReady", RPCMode.All, false);
		}

		// TODO: Start the dialogue
	}
}
