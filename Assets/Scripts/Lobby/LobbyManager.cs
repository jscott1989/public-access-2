using UnityEngine;
using System.Linq;
using System.Collections;

public class LobbyManager : MonoBehaviour {

	public ChatTextList textList;
	int playerNameIncrementor = 1;

	GameObject[] existingPlayers = new GameObject[]{};

	// Use this for initialization
	void Start () {
	
	}

	void AnnounceAllPlayerNames() {
		foreach (GameObject p in existingPlayers) {
			Player player = (Player)p.GetComponent (typeof(Player));
			if (player.player_name == "") {
				// This player is new, set a new name
				player.SetPlayerName ("Player " + playerNameIncrementor.ToString ());
				playerNameIncrementor += 1;
				textList.Add (player.player_name + " has joined");
			} else {
				player.SetPlayerName (player.player_name);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Network.isServer) {
			GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
			foreach (GameObject p in players) {
				if (!(existingPlayers.Contains (p))) {
					existingPlayers = players;
					// There is at least one new player 
					AnnounceAllPlayerNames();
					return;
				}
			}
		}
	}
}
