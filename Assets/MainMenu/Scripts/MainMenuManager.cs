using UnityEngine;
using System;
using System.Collections;

/**
 * Main Menu Manager
 */
public class MainMenuManager : SceneManager {

	NetworkManager mNetworkManager;
	dfListbox mGamesList;
	LoadingPanel mLoadingPanel;
	ErrorPanel mErrorPanel;

	HostData[] mHosts;

	public string uCreateGameRoomName;


	public bool hasHosts {
		get {
			return (mHosts != null) && (mHosts.Length > 0);
		}
	}

	public bool gameNameIsEntered {
		get {
			return uCreateGameRoomName != "";
		}
	}


	void Awake() {
		mNetworkManager = (NetworkManager)GameObject.FindObjectOfType (typeof(NetworkManager));
		mGamesList = (dfListbox)GameObject.FindObjectOfType (typeof(dfListbox));
		mLoadingPanel = (LoadingPanel)GameObject.FindObjectOfType (typeof(LoadingPanel));
		mErrorPanel = (ErrorPanel)GameObject.FindObjectOfType (typeof(ErrorPanel));
	}

	void Start() {
		// Start by populating the games list
		RefreshGamesList ();
	}

	/**
	 * The Create Game button has been pushed
	 */
	public void CreateGame() {
		if (uCreateGameRoomName == "") {
			mErrorPanel.ShowError ("You must choose a room name");
		} else {
			mLoadingPanel.ShowAlert ("Creating Game...");

			Action serverStarted =
				() => {
					mLoadingPanel.HideAlert ();
					Application.LoadLevel ("Lobby");
			};
			mNetworkManager.StartServer (uCreateGameRoomName, serverStarted);
		}
	}

	/**
	 * The Refresh button has been pushed
	 */
	public void RefreshGamesList() {
		Action<HostData[]> gamesLoaded =
			(pHosts) => {
				mHosts = pHosts;
				mGamesList.Items = new string[]{};

				foreach (HostData h in pHosts) {
					mGamesList.AddItem (h.gameName + " (" + h.connectedPlayers.ToString () + "/" + h.playerLimit.ToString () + ")");
				}
				mLoadingPanel.HideAlert ();
			};
		mLoadingPanel.ShowAlert ("Refreshing Games List...");
		mNetworkManager.RefreshHostList (gamesLoaded);
	}

	/**
	 * Join the selected game
	 */
	public void JoinGame() {
		if (mGamesList.SelectedIndex < 0) {
			mErrorPanel.ShowError ("You must select a game to join, or create a new game");
		} else {
			mLoadingPanel.ShowAlert ("Joining " + mHosts[mGamesList.SelectedIndex].gameName);

			Action gameJoined =
				() => {
					mLoadingPanel.HideAlert ();
					Application.LoadLevel ("Lobby");
			};
			mNetworkManager.JoinServer (mHosts[mGamesList.SelectedIndex], gameJoined);
		}
	}
}
