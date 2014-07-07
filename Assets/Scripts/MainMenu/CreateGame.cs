using UnityEngine;
using System.Collections;

public class CreateGame : MonoBehaviour {

	public NetworkManager networkManager;

	void OnClick()
	{
		networkManager.StartServer ("GAME");

		Application.LoadLevel ("Lobby");


	}

}
