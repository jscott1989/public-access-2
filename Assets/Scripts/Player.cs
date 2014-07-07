using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public string player_name;
	// Use this for initialization
	void Start () {
		print ("HELLO!");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	[RPC] public void SetPlayerName(string player_tempname)
	{
		player_name = player_tempname;

		if (Network.isServer) {
						networkView.RPC ("SetPlayerName", RPCMode.All, player_name);
				}

	}
}
