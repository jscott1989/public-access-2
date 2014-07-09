using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviour {

	public ChatTextList textList;
	int playerNameIncrementor = 1;

	public GameObject[] allPlayers = new GameObject[]{};

	public GameObject playerInfoPrefab;

	Dictionary<Player, GameObject> playerInfoBoxes = new Dictionary<Player, GameObject>();

	NetworkManager networkManager;

	// Use this for initialization
	void Start () {
		networkManager = (NetworkManager) GameObject.FindGameObjectWithTag ("NetworkManager").GetComponent ("NetworkManager");
	}

	public GameObject AddPlayerInfo(string username) {
		GameObject playerInfo = (GameObject)Network.Instantiate (playerInfoPrefab, Vector3.zero, Quaternion.identity, 0);

		PlayerInfoBox i = (PlayerInfoBox) playerInfo.GetComponent (typeof(PlayerInfoBox));
		i.AddPlayerInfo (username);
		return playerInfo;
	}

	void AnnounceAllPlayerNames() {
		foreach (GameObject p in allPlayers) {
			Player player = (Player)p.GetComponent (typeof(Player));
			player.SetPlayerName (player.player_name);
		}
	}

	// Update is called once per frame
	void Update () {
		if (Network.isServer) {
			GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
			bool newPlayer = false;

			foreach (GameObject p in players) {
				if (!(allPlayers.Contains (p))) {
					allPlayers = players;
					// There is at least one new player 
					newPlayer = true;

					Player player = (Player)p.GetComponent (typeof(Player));

					// Set the player's name
					player.SetPlayerID (playerNameIncrementor);
					player.SetPlayerName ("Player " + playerNameIncrementor.ToString ());

					playerNameIncrementor += 1;
					textList.Add (player.player_name + " has joined");

					playerInfoBoxes[player] = AddPlayerInfo(player.player_name);
				}
			}

			if (newPlayer) {
				AnnounceAllPlayerNames();
			}
		}
	}

	/**
	 * Toggle the readiness of the current player
	 */
	public void ToggleReady() {
//		Player myPlayer = networkManager.GetMyPlayer ();
//		myPlayer.ToggleReady ();
//
//		if (myPlayer.ready) {
//			playerInfoBoxes[myPlayer].GetComponentInChildren (typeof(UILabel));
//		}
	}
}
