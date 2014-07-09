using UnityEngine;
using System.Collections;

/**
 * This class holds basic information about each player, and methods
 * to interact with the player
 */
public class Player : MonoBehaviour {

	// The player's ID
	public int uID;

	// The player's display name
	public string uName;

	void Awake() {
		// Persist the Player between scenes
		DontDestroyOnLoad(gameObject);
	}

	/**
	 * Use this method to set basic information for the player
	 */
	[RPC] public void SetInfo(int pID, string pName) {
		uID = pID;
		uName = pName;
		print("Setting to " + uName);

		if (networkView.isMine) {
			networkView.RPC ("SetInfo", RPCMode.Others, pID, pName);
		}
	}

	/**
	 * Send my info to another network player
	 */
	public void SendInfoTo(NetworkPlayer pPlayer) {
		print("Sending info");
		networkView.RPC ("SetInfo", pPlayer, uID, uName);
	}
}
