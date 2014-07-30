using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LobbyManager : SceneManager {
	GameObject mMyPlayerInfoPrefab;
	GameObject mPlayerInfoPrefab;
	dfScrollPanel mPlayersList;
	dfRichTextLabel mChat;
	LoadingPanel mLoadingPanel;
	NetworkManager mNetworkManager;

	// The current typed chat message
	public string uChatMessage;

	/**
	 * This is used to countdown 5 seconds before the game begins
	 */
	float mCountdown = -1;
	// This is used to announce each second as it passes
	int mLastCountdownAnnouncement = 0;


	public string uLobbyName {
		get {
			if (mNetworkManager == null) {
				return "";
			}
			return mNetworkManager.uRoomName + " Lobby";
		}
	}
	void Awake() {
		mMyPlayerInfoPrefab = (GameObject)Resources.Load ("Lobby/Prefabs/MyPlayerInfoBox");
		mPlayerInfoPrefab = (GameObject)Resources.Load ("Lobby/Prefabs/PlayerInfoBox");
		mPlayersList = (dfScrollPanel)GameObject.FindObjectOfType (typeof(dfScrollPanel));
		mChat = (dfRichTextLabel)GameObject.FindObjectOfType (typeof(dfRichTextLabel));
		mLoadingPanel = (LoadingPanel)GameObject.FindObjectOfType (typeof(LoadingPanel));
		mNetworkManager = (NetworkManager)GameObject.FindObjectOfType (typeof(NetworkManager));
	}

	void Start() {
		// Create a NewPlayer event for all players currently in the game
	
		foreach (Player player in mNetworkManager.players) {
			NewPlayer (player);
		}

		networkView.RPC ("AddChatMessage", RPCMode.All, "<br /><i style=\"color: black;\">Player " + (mNetworkManager.mMyClientID + 1).ToString () + " has joined</i>");

	}

	/**
	 * Create an info box for another player
	 */
	void CreatePlayerInfoBox(Player pPlayer) {
		GameObject playerInfo = (GameObject) Instantiate (mPlayerInfoPrefab, Vector3.zero, Quaternion.identity);
		playerInfo.transform.parent = mPlayersList.gameObject.transform;

		dfLabel nameLabel = (dfLabel)playerInfo.GetComponentsInChildren (typeof(dfLabel))[0];
		dfPropertyBinding.Bind (nameLabel.gameObject, pPlayer,"uName", nameLabel,"Text");

		dfLabel stationLabel = (dfLabel)playerInfo.GetComponentsInChildren (typeof(dfLabel))[1];
		dfPropertyBinding.Bind (stationLabel.gameObject, pPlayer,"uStationName", stationLabel,"Text");

		dfTextureSprite stationLogo = playerInfo.transform.FindChild ("StationLogo").GetComponent<dfTextureSprite>();
		dfPropertyBinding.Bind (stationLogo.gameObject, pPlayer,"uStationLogo", stationLogo,"Texture");

		PlayerInfoBox p = (PlayerInfoBox)playerInfo.GetComponent (typeof(PlayerInfoBox));
		p.uID = pPlayer.uID;

		dfTextureSprite d = playerInfo.transform.FindChild ("IsPlayerReady").GetComponent<dfTextureSprite>();
		dfPropertyBinding.Bind (d.gameObject, pPlayer,"uReadyTexture",d,"Texture");
	}

	/**
	 * Create my own info box
	 */
	void CreateMyPlayerInfoBox(Player pPlayer) {
		GameObject playerInfo = (GameObject) Instantiate (mMyPlayerInfoPrefab, Vector3.zero, Quaternion.identity);
		playerInfo.transform.parent = mPlayersList.gameObject.transform;

		dfTextbox myNameTextBox = (dfTextbox)playerInfo.GetComponentInChildren (typeof(dfTextbox));
		myNameTextBox.Text = pPlayer.uName;
		dfPropertyBinding.Bind (myNameTextBox.gameObject, myNameTextBox,"Text",pPlayer,"uName");

		dfDropdown myStation = (dfDropdown)playerInfo.GetComponentInChildren (typeof(dfDropdown));

		dfEventDrivenPropertyBinding.Bind (myStation.gameObject, pPlayer, "uAvailableStationNames", "AvailableStationsHaveChanged", myStation, "Items", null);

		dfPropertyBinding.Bind (myStation.gameObject, myStation, "SelectedIndex", pPlayer, "uSelectedStationIndex");

		dfTextureSprite stationLogo = playerInfo.transform.FindChild ("StationLogo").GetComponent<dfTextureSprite>();
		dfPropertyBinding.Bind (stationLogo.gameObject, pPlayer,"uStationLogo", stationLogo,"Texture");

		dfTextureSprite d = playerInfo.transform.FindChild ("IsPlayerReady").GetComponent<dfTextureSprite>();
		dfPropertyBinding.Bind (d.gameObject, pPlayer,"uReadyTexture",d,"Texture");
	}

	public override void PlayerConnected(int pID, NetworkPlayer pPlayer) {
		print ("Player " + pID.ToString () + " has connected");
		// A new player has joined - so we should fill them in on the state of the lobby
		// (This is only called on the Server)
		foreach (Player p in mNetworkManager.players) {
			if (p.uID != pID) {
				p.SendInfoTo(pPlayer);
			}
		}
	}

	public override void PlayerDisconnected(int pID, NetworkPlayer pPlayer) {
		// A player has left - so we should inform the players
		// (This is only called on the Server)
		networkView.RPC ("AddChatMessage", RPCMode.All, "<br /><i style=\"color: black;\">" + mNetworkManager.GetPlayerWithID(pID).uName + " has left</i>");
		Network.DestroyPlayerObjects(pPlayer);
	}

	public override void NewPlayer(Player pPlayer) {
		// A player has joined the server (this is called for everyone)
		if (pPlayer.uID == mNetworkManager.mMyClientID && GameObject.FindGameObjectsWithTag("MyPlayerInfoBox").Length == 0) { // I Check for an existing PlayerInfoBox 
			// This is me, so make a myplayerinfobox
			CreateMyPlayerInfoBox (pPlayer);
		} else {
			CreatePlayerInfoBox (pPlayer);
		}
	}

	public override void PlayerLeaves(Player pPlayer) {
		// We need to remove the player's info box
		foreach (PlayerInfoBox infoBox in GameObject.FindObjectsOfType(typeof(PlayerInfoBox))) {
			if (infoBox.uID == pPlayer.uID) {
				Destroy (infoBox.gameObject);
			}
		}
	}

	[RPC] void AddChatMessage(string pMessage) {
		mChat.Text += pMessage;
		mChat.ScrollToBottom (); // TODO: This isn't working for some reason
	}

	/**
	 * This is when the chat input is submitted
	 */
	public void SubmitChatMessage() {
		networkView.RPC ("AddChatMessage", RPCMode.All, "<br /><b style=\"color: black;\">&lt;" + mNetworkManager.myPlayer.uName + "&gt;</b> " + uChatMessage);
		uChatMessage = "";
	}

	/**
	 * This is when the "Quit" button is pressed
	 */
	public void Quit() {
		// Destroy the NetworkManager - the user will be kicked to the main menu
		mNetworkManager.Shutdown ();
	}

	/**
	 * This is called on the server when any player changes their ready status
	 */
	public override void ReadyStatusChanged(Player pPlayer) {
		if (pPlayer.uReady) {
			networkView.RPC ("AddChatMessage", RPCMode.All, "<br /><i style=\"color: black;\">" + pPlayer.uName + " is ready</i>");
		} else {
			networkView.RPC ("AddChatMessage", RPCMode.All, "<br /><i style=\"color: black;\">" + pPlayer.uName + " is not ready</i>");
			if (mCountdown > -1) {
				CancelCountdown();
			}
		}
	}

	public override void AllReady() {
		StartCountdown();
	}

	/**
	 * Start the countdown until the game begins
	 */
	void StartCountdown() {
		mNetworkManager.StartGame ();
		mCountdown = Game.LOBBY_COUNTDOWN;
	}

	void CancelCountdown() {
		mNetworkManager.StopGame ();
		mCountdown = -1;
		networkView.RPC ("AddChatMessage", RPCMode.All, "<br /><i style=\"color: black;\">Game stopped</i>");
	}

	/**
	 * Inform the players the game is starting
	 */
	[RPC] void StartingGame() {
		mLoadingPanel.ShowAlert ("Starting game...");
	}

	/**
	 * Move to the Day 1 morning phase
	 */
	[RPC] void StartGame() {
		mLoadingPanel.HideAlert ();
		mNetworkManager.LoadLevel ("Morning");
	}

	void ServerStartGame() {
		networkView.RPC ("StartingGame", RPCMode.All);
		Player[] players = mNetworkManager.players;
		GameSetup gameSetup = new GameSetup(players.Length);
		System.Random r = new System.Random();
		for (var i = 0; i < players.Length; i++) {
			if (players[i].uSelectedStation.uID == Game.RANDOM_STATION_ID) {
				// We need to pick an available station at random for this user
				players[i].networkView.RPC ("SetSelectedStation", RPCMode.All, players[i].uAvailableStations[r.Next (1, players[i].uAvailableStations.Count())].uID);
			}
			players[i].networkView.RPC ("SetGameInfo",RPCMode.All, gameSetup.uOldThemes[i], gameSetup.uBosses[i], gameSetup.uSonNames[i], RPCEncoder.Encode (gameSetup.uFemaleNames[i]), gameSetup.uThemes[i], RPCEncoder.Encode(gameSetup.uNeeds[i]), RPCEncoder.Encode(gameSetup.uAvailableProps), RPCEncoder.Encode(gameSetup.uAvailableBackdrops), RPCEncoder.Encode(gameSetup.uAvailableAudio));
		}
		networkView.RPC ("StartGame", RPCMode.All);
	}

	void Update() {
		if (Network.isServer) {
			if (mCountdown > -1) {
				mCountdown -= Time.deltaTime;

				if (mCountdown <= 0) {
					mCountdown = -1;
					ServerStartGame ();
					return;
				}

				if ((int)mCountdown != mLastCountdownAnnouncement) {
					mLastCountdownAnnouncement = (int)mCountdown;
					if (mLastCountdownAnnouncement > 0) {
						networkView.RPC ("AddChatMessage", RPCMode.All, "<br /><i style=\"color: black;\">Game starting in " + mLastCountdownAnnouncement.ToString() + "</i>");
					} else {
						networkView.RPC ("AddChatMessage", RPCMode.All, "<br /><i style=\"color: black;\">Game starting...</i>");
					}
				}
			}
		}
	}
}
