using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public string player_name;

	// TODO: Figure out a decent default budget - depends on the price of props
	public int budget = 200;

	// This is an array of strings which are identifiers for all of the props the player has purchased
	public List<string> props = new List<string>();

	// This will be used for multiple things - to decide if the player is ready to continue to the next stage
	public bool ready = false;

	public int player_id;

	void Start() {
		// TODO: Get rid of this once the purchasing functionality is working
		// We just set some default props so the recording can be tested
		Purchase (100, "bible");
		Purchase (100, "bear");
	}

	[RPC] public void Purchase(int price, string prop_id) {
		if (budget >= price) {
			props.Add (prop_id);
			budget -= price;
			if (networkView.isMine) {
				// If this is our player then tell the others about the purchase
				networkView.RPC ("purchase", RPCMode.Others, price, prop_id);
			}
		}
	}

	[RPC] public void SetPlayerName(string player_tempname) {
		player_name = player_tempname;

		if (Network.isServer) {
			networkView.RPC ("SetPlayerName", RPCMode.Others, player_name);
		}
	}

	[RPC] public void SetPlayerID(int player_tempid) {
		player_id = player_tempid;
		
		if (Network.isServer) {
			networkView.RPC ("SetPlayerID", RPCMode.Others, player_id);
		}
	}

	[RPC] public void NotReady() {
		ready = false;
		if (networkView.isMine) {
			networkView.RPC ("NotReady", RPCMode.Others);
		}
	}

	[RPC] public void ToggleReady() {
		ready = !ready;
		if (networkView.isMine) {
			networkView.RPC ("ToggleReady", RPCMode.Others);
		}
	}
}
