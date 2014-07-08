using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public string player_name;

	// TODO: Figure out a decent default budget - depends on the price of props
	public int budget = 200;

	// This is an array of strings which are identifiers for all of the props the player has purchased
	public List<string> props = new List<string>();

	void Start() {
		// TODO: Get rid of this once the purchasing functionality is working
		// We just set some default props so the recording can be tested
		purchase (100, "bible");
		purchase (100, "bear");
	}

	[RPC] public void purchase(int price, string prop_id) {
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
}
