using UnityEngine;
using System.Collections;

public class Ready : MonoBehaviour {
	public LobbyManager lobbyManager;

	void OnClick() {
		lobbyManager.ToggleReady ();
	}
}
