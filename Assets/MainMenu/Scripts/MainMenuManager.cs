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
	Playlist mPlaylist;

	Texture mSoundPlayingTexture;
	Texture mSoundNotPlayingTexture;

	GameObject mGamePrefab;

	HostData[] mHosts;

	public string uCreateGameRoomName;

	public bool uIsCreatingGame = false;
	public bool uVoiceChatEnabled = true;

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
		mNetworkManager = GameObject.FindObjectOfType<NetworkManager>();
		mGamesList = GameObject.FindObjectOfType<dfListbox>();
		mLoadingPanel = GameObject.FindObjectOfType<LoadingPanel>();
		mErrorPanel = GameObject.FindObjectOfType<ErrorPanel>();
		mPlaylist = GameObject.FindObjectOfType<Playlist>();
		mSoundPlayingTexture = (Texture)Resources.Load ("MainMenu/Images/sound_enabled");
		mSoundNotPlayingTexture = (Texture)Resources.Load ("MainMenu/Images/sound_disabled");
		mGamePrefab = (GameObject)Resources.Load ("Prefabs/Game");
	}

	void Start() {
		// Start by populating the games list
		RefreshGamesList ();

		if (Game.DEBUG_MODE) {
			uCreateGameRoomName = "test";
		}
	}

	/**
	 * The Create Game button has been pushed
	 */
	public void CreateGame() {
		uIsCreatingGame = true;

		// TODO: For some reason this doesn't work:
//		GameObject.Find ("Game Name").GetComponent<dfTextbox>().Focus();
	}

	public void CancelCreateGame() {
		uIsCreatingGame = false;
	}

	/**
	 * The Create Game button has been pushed
	 */
	public void CreateGameSubmit() {
		if (uCreateGameRoomName == "") {
			mErrorPanel.ShowError ("You must choose a room name");
		} else {
			GameObject g = (GameObject)Instantiate (mGamePrefab, Vector3.zero, Quaternion.identity);
			Game game = g.GetComponent<Game>();

			// TODO: Set settings on game (which content packs will be loaded, if sound is enabled, etc.)
			game.uRoomName = uCreateGameRoomName;
			game.uVoiceChatEnabled = uVoiceChatEnabled;

			mLoadingPanel.ShowAlert ("Creating Game...");
			
			Action serverStarted =
			() => {
				mNetworkManager.LoadLevel("Lobby");
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
					if (h.comment == "Closed") {
						continue;
					}
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
					mNetworkManager.LoadLevel ("Lobby");
			};
			mNetworkManager.JoinServer (mHosts[mGamesList.SelectedIndex], gameJoined);
		}
	}

	public Texture StartStopMusicTexture {
		get {
			if (mPlaylist.uIsPlaying) {
				return mSoundPlayingTexture;
			} else {
				return mSoundNotPlayingTexture;
			}
		}
	}

	public void StartStopMusic() {
		if (mPlaylist.uIsPlaying) {
			mPlaylist.StopPlaying();
		} else {
			mPlaylist.StartPlaying();
		}
	}
}
