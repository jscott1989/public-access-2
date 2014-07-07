using UnityEngine;
using System.Collections;

public class JoinGame : MonoBehaviour {

	public NetworkManager networkManager;

	void OnClick(){

		networkManager.JoinServer (networkManager.hostList [0]);
		}

}
