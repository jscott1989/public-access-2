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
		// Create a NewPlayer event for all players currently in the game
		print(mNetworkManager.players.Length);
		foreach (Player player in mNetworkManager.players) {
			NewPlayer (player);
		}
	}

	void CreatePlayerInfoBox(Player pPlayer) {
		GameObject playerInfo = (GameObject) Instantiate (mPlayerInfoPrefab, Vector3.zero, Quaternion.identity);
		playerInfo.transform.parent = mPlayersList.gameObject.transform;

		dfLabel nameLabel = (dfLabel)playerInfo.GetComponentInChildren (typeof(dfLabel));
		dfPropertyBinding b = dfPropertyBinding.Bind (pPlayer,"uName", nameLabel,"Text");
	}

	void CreateMyPlayerInfoBox(Player pPlayer) {
		GameObject playerInfo = (GameObject) Instantiate (mMyPlayerInfoPrefab, Vector3.zero, Quaternion.identity);
		playerInfo.transform.parent = mPlayersList.gameObject.transform;

		dfTextbox myNameTextBox = (dfTextbox)playerInfo.GetComponentInChildren (typeof(dfTextbox));
		myNameTextBox.Text = pPlayer.uName;
		dfPropertyBinding b = dfPropertyBinding.Bind (myNameTextBox,"Text",pPlayer,"uName");

	}

	public override void PlayerConnected(int pID, NetworkPlayer pPlayer) {
		// A new player has joined - so we should fill them in on the state of the lobby
		foreach (Player p in mNetworkManager.players) {
			if (!p.Equals(pPlayer)) {
				p.SendInfoTo(pPlayer);
			}
		}
	}

	public override void NewPlayer(Player pPlayer) {
		if (pPlayer.uID == mNetworkManager.mMyClientID) {
			// This is me, and I won't be ready yet so make a myplayerinfobox
			CreateMyPlayerInfoBox (pPlayer);
		} else {
			CreatePlayerInfoBox (pPlayer);
		}
	}
}
