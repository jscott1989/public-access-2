using UnityEngine;
using System.Collections;

/**
 * Manage the Ready button on the Lobby
 */
public class ReadyButton : MonoBehaviour {

	NetworkManager mNetworkManager;

	void Awake() {
		mNetworkManager = (NetworkManager)FindObjectOfType (typeof(NetworkManager));
	}

	public string uReadyButtonText {
		get {
			if (mNetworkManager.myPlayer.uReady) {
				return "Cancel";
			} else {
				return "Ready";
			}
		}
	}

	void OnClick() {
		mNetworkManager.myPlayer.networkView.RPC ("SetReady", RPCMode.All, !mNetworkManager.myPlayer.uReady);
	}
}
