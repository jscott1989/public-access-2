using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LobbyManager : SceneManager {
	GameObject mMyPlayerInfoPrefab;
	GameObject mPlayerInfoPrefab;
	dfScrollPanel mPlayersList;
	dfRichTextLabel mChat;
	LoadingPanel mLoadingPanel;
	NetworkManager mNetworkManager;
	Game mGame;

	// The current typed chat message
	public string uChatMessage;

	/**
	 * This is used to countdown 5 seconds before the game begins
	 */
	float mCountdown = -1;
	// This is used to announce each second as it passes
	int mLastCountdownAnnouncement = 0;

	void Awake() {
		mMyPlayerInfoPrefab = (GameObject)Resources.Load ("Lobby/Prefabs/MyPlayerInfoBox");
		mPlayerInfoPrefab = (GameObject)Resources.Load ("Lobby/Prefabs/PlayerInfoBox");
		mPlayersList = (dfScrollPanel)GameObject.FindObjectOfType (typeof(dfScrollPanel));
		mChat = (dfRichTextLabel)GameObject.FindObjectOfType (typeof(dfRichTextLabel));
		mLoadingPanel = (LoadingPanel)GameObject.FindObjectOfType (typeof(LoadingPanel));
		mNetworkManager = (NetworkManager)GameObject.FindObjectOfType (typeof(NetworkManager));
		mGame = (Game)FindObjectOfType (typeof(Game));
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

		PlayerInfoBox p = (PlayerInfoBox)playerInfo.GetComponent (typeof(PlayerInfoBox));
		p.uID = pPlayer.uID;

		dfTextureSprite d = (dfTextureSprite) playerInfo.GetComponentsInChildren (typeof(dfTextureSprite))[0];
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

		dfTextureSprite d = (dfTextureSprite) playerInfo.GetComponentsInChildren (typeof(dfTextureSprite))[0];
		dfPropertyBinding.Bind (d.gameObject, pPlayer,"uReadyTexture",d,"Texture");
	}

	public override void PlayerConnected(int pID, NetworkPlayer pPlayer) {
		// A new player has joined - so we should fill them in on the state of the lobby
		// (This is only called on the Server)
		foreach (Player p in mNetworkManager.players) {
			if (!p.Equals(pPlayer)) {
				p.SendInfoTo(pPlayer);
			}
		}
}

	public override void PlayerDisconnected(int pID, NetworkPlayer pPlayer) {
		// A player has left - so we should inform the players
		// (This is only called on the Server)
		networkView.RPC ("AddChatMessage", RPCMode.All, "<br /><i style=\"color: black;\">" + mNetworkManager.GetPlayerWithID(pID).uName + " has left</i>");
	}

	public override void NewPlayer(Player pPlayer) {
		// A player has joined the server (this is called for everyone)
		if (pPlayer.uID == mNetworkManager.mMyClientID) {
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

			CheckForAllReady();
		} else {
			networkView.RPC ("AddChatMessage", RPCMode.All, "<br /><i style=\"color: black;\">" + pPlayer.uName + " is not ready</i>");
			if (mCountdown > -1) {
				CancelCountdown();
			}
		}
	}

	void CheckForAllReady() {
		if (Game.DEBUG_MODE) {
			mCountdown = -1;
			ServerStartGame ();
		}
		
		// Check if all players are ready - if so we can start
		foreach (Player p in mNetworkManager.players) {
			if (!p.uReady) {
				return;
			}
		}
		
		// TODO: Add 3 player minimum (I'm not adding this now as it's easier to test things with 2)
		
		// Everyone is ready, let's start the countdown
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
		Application.LoadLevel ("Morning");
	}

	void ServerStartGame() {
		networkView.RPC ("StartingGame", RPCMode.All);
		Player[] players = mNetworkManager.players;
		GameSetup gameSetup = GameSetup.generate (players.Length);
		for (var i = 0; i < players.Length; i++) {
			players[i].networkView.RPC ("SetGameInfo",RPCMode.All, gameSetup.uThemes[i], gameSetup.uNeeds[i], RPCEncoder.Encode(gameSetup.uAvailableProps));
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
