using UnityEngine;
using System.Collections;

public class LobbyManager : SceneManager {
	GameObject mMyPlayerInfoPrefab;
	GameObject mPlayerInfoPrefab;
	dfScrollPanel mPlayersList;
	LoadingPanel mLoadingPanel;
	NetworkManager mNetworkManager;

	void Awake() {
		mMyPlayerInfoPrefab = (GameObject)Resources.Load ("Lobby/Prefabs/MyPlayerInfoBox");
		mPlayerInfoPrefab = (GameObject)Resources.Load ("Lobby/Prefabs/PlayerInfoBox");
		mPlayersList = (dfScrollPanel)GameObject.FindObjectOfType (typeof(dfScrollPanel));
		mLoadingPanel = (LoadingPanel)GameObject.FindObjectOfType (typeof(LoadingPanel));
		mNetworkManager = (NetworkManager)GameObject.FindObjectOfType (typeof(NetworkManager));
	}

	void Start() {
		// When the lobby loads we're definitely not "Ready" so we create our own info box
		CreateMyPlayerInfoBox ();

		// Then we need to loop through all active players and create boxes for them
		// TODO


	}

	void CreatePlayerInfoBox() {
		GameObject playerInfo = (GameObject) Instantiate (mPlayerInfoPrefab, Vector3.zero, Quaternion.identity);
		playerInfo.transform.parent = mPlayersList.gameObject.transform;
	}

	void CreateMyPlayerInfoBox() {
		GameObject playerInfo = (GameObject) Instantiate (mMyPlayerInfoPrefab, Vector3.zero, Quaternion.identity);
		playerInfo.transform.parent = mPlayersList.gameObject.transform;
	}

	public override void PlayerConnected(int pID, NetworkPlayer pPlayer) {
		// A new player has joined - so we should fill them in on the state of the lobby
		foreach (Player p in mNetworkManager.players) {
			if (!p.Equals(pPlayer)) {
				p.SendInfoTo(pPlayer);
			}
		}
	}
}
