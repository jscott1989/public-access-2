using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/**
 * This class sets up and then maintains the server/client
 */
public class NetworkManager : MonoBehaviour {
	// Unique game name for matchmaking
	private const string GAME_NAME = "PublicAccessWars1";
	// The port to connect to
	private const int SERVER_PORT = 25000;

	public string uRoomName = "";

	// A list of known servers we can connect to
	public HostData[] uHostList;

	// One of these will be created for each player
	GameObject mPlayerPrefab;

	// Has the game started?
	public bool uGameHasStarted{ get; set; }

	// The callback to call when the server starts successfully
	private Action mStartServerCallback;

	// The callback when we have joined a game successfully
	private Action mJoinGameCallback;

	// The callback to call when we recieve a list of games
	private Action<HostData[]> mRefreshHostCallback;

	public SceneManager uSceneManager;
	private Game mGame;

	public int mMyClientID;

	public Player[] players {
		get {
			return GameObject.FindObjectsOfType<Player>();
		}
	}

	private Player[] _playersOrderedByStation;

	/**
	 * This caches the players ordered by the station they selected
	 * this ensures that every player will be ordered in the same way on all clients
	 */
	public Player[] playersOrderedByStation {
		get {
			if (_playersOrderedByStation != null) {
				return _playersOrderedByStation;
			}
			List<Player> p = new List<Player>();

			Dictionary<Station, Player> stationToPlayer = new Dictionary<Station, Player>();

			foreach(Player pp in players) {
				stationToPlayer[pp.uSelectedStation] = pp;
			}

			foreach(Station s in mGame.uStations) {
				if (stationToPlayer.ContainsKey(s)) {
					p.Add (stationToPlayer[s]);
				}
			}

			_playersOrderedByStation = p.ToArray ();
			return _playersOrderedByStation;
		}
	}


	Player _myPlayer = null;
	public Player myPlayer {
		get {
			if (_myPlayer != null) {
				return _myPlayer;
			}
			foreach (Player player in players) {
				if (player.uID == mMyClientID) {
					_myPlayer = player;
					return _myPlayer;
				}
			}
			return null;
		}
	}

	Dictionary<int, Player> playerByID = new Dictionary<int, Player>();
	public Player GetPlayerWithID(int pID) {
		if (playerByID.ContainsKey (pID)) {
			return playerByID[pID];
		}
		foreach (Player player in players) {
			playerByID[player.uID] = player;
		}
		if (playerByID.ContainsKey (pID)) {
			return playerByID[pID];
		}
		return null;
	}
	
	ErrorPanel mErrorPanel;
	DialogueManager mDialogueManager;

	void Awake() {
		// This is just for during testing - as the unity master server keeps banning me!
//		MasterServer.ipAddress = "127.0.0.1";
//		MasterServer.port = 23466;

		mDialogueManager = GameObject.FindObjectOfType<DialogueManager>();
		mErrorPanel = GameObject.FindObjectOfType<ErrorPanel>();
		mPlayerPrefab = Resources.Load<GameObject>("Prefabs/Player");
		mGame = FindObjectOfType<Game>();
		/**
		 * Persist the NetworkManager between scenes
		 */
		DontDestroyOnLoad (gameObject);
	}

	/**
	 * Start hosting the server and accepting connections
	 */
	public void StartServer(string pRoomName, Action pStartServerCallback) {
		mStartServerCallback = pStartServerCallback;

		NetworkConnectionError e = Network.InitializeServer (mGame.uStations.Count - 2, SERVER_PORT, !Network.HavePublicAddress ()); // -1 is for Random, another is because I dunno but it adds up - TODO: Come back to this
		if (e != NetworkConnectionError.NoError) {
			mErrorPanel.ShowError (e.ToString ());
			ReturnToMainMenu();
			return;
		}
		uRoomName = pRoomName;
		StopGame (); // This registers everything with hte server - we're accepting new people
	}

	/**
	 * Remove the server from the room list and stop accepting connections
	 */
	public void StartGame() {
		MasterServer.UnregisterHost ();
		uGameHasStarted = true;
	}

	/**
	 * Add the server to the room list and start accepting connections
	 */
	public void StopGame() {
		MasterServer.RegisterHost(GAME_NAME, uRoomName);
		uGameHasStarted = false;
	}

	public void CreatePlayer() {
		mMyClientID = Convert.ToInt16 (Network.player.ToString ());
		GameObject playerObject = (GameObject) Network.Instantiate (mPlayerPrefab, Vector3.zero, Quaternion.identity, 0);
		Player player = (Player)playerObject.GetComponent (typeof(Player));
		// Set the basic player information for me and others
		player.SetInfo (mMyClientID, "Player " + (mMyClientID + 1).ToString ());
	}

	/**
	 * This is called when we recieve a message from the master server
	 */
	void OnMasterServerEvent(MasterServerEvent pEvent) {
		if (pEvent == MasterServerEvent.HostListReceived) {
			uHostList = MasterServer.PollHostList ();
			mRefreshHostCallback(uHostList);
		} else if (pEvent == MasterServerEvent.RegistrationSucceeded) {
			// This means we are now hosting the game successfully
			if (mStartServerCallback != null) {
				mStartServerCallback();
				mStartServerCallback = null;
			}
		} else if (pEvent == MasterServerEvent.RegistrationFailedGameName) {
			// There has been an error setting up the server
			mErrorPanel.ShowError ("There was an error starting the server (Invalid game name)");
		} else if (pEvent == MasterServerEvent.RegistrationFailedGameType) {
			// There has been an error setting up the server
			mErrorPanel.ShowError ("There was an error starting the server (Invalid game type)");
		} else if (pEvent == MasterServerEvent.RegistrationFailedNoServer) {
			// There has been an error setting up the server
			mErrorPanel.ShowError ("There was an error starting the server (No server)");
		}
	}

	public void RefreshHostList(Action<HostData[]> pRefreshHostCallback) {
		mRefreshHostCallback = pRefreshHostCallback;
		MasterServer.RequestHostList (GAME_NAME);
	}

	/**
	 * Join a server from the list
	 */
	public void JoinServer(HostData pHostData, Action pJoinGameCallback) {
		mJoinGameCallback = pJoinGameCallback;
		uRoomName = pHostData.gameName;
		Network.Connect(pHostData);
	}

	/**
	 * Called when we have connected to the server
	 */
	void OnConnectedToServer() {
		CreatePlayer ();

		mJoinGameCallback ();
	}

	/**
	 * Called when there is an error connecting to the server
	 */
	void OnFailedToConnect(NetworkConnectionError error) {
		mErrorPanel.ShowError (error.ToString ());
	}

	/**
	 * Called when the server is listening for connections
	 */
	void OnServerInitialized() {
		CreatePlayer();
	}

	/**
	 * Called on the server when a player connects
	 */
	void OnPlayerConnected(NetworkPlayer pPlayer) {
		if (uGameHasStarted) {
			Network.CloseConnection (pPlayer, true);
		} else {
			int id = Convert.ToInt16(pPlayer.ToString ());

			uSceneManager.PlayerConnected(id, pPlayer);
		}
	}

	/**
	 * Called on the server when a player disconnects
	 */
	void OnPlayerDisconnected(NetworkPlayer pPlayer) {
		int id = Convert.ToInt16(pPlayer.ToString ());
		uSceneManager.PlayerDisconnected(id, pPlayer);
		Network.RemoveRPCs(pPlayer);
	}

	/**
	 * Called on both server and client - we care about it when the connection
	 * drops and need to warn the client
	 */
	void OnDisconnectedFromServer(NetworkDisconnection pInfo) {
		if (Network.isClient) {
			if (pInfo == NetworkDisconnection.LostConnection) {
				mErrorPanel.ShowError("You have been disconnected from the server.");
			}
		}
		ReturnToMainMenu();
	}

	public void ReturnToMainMenu() {
		foreach (Player player in players) {
			Destroy (player.gameObject);
		}

		// Destroy the Game object, which will be recreated
		Destroy(FindObjectOfType<Game>().gameObject);

		Destroy (gameObject);
		mDialogueManager.EndDialogue();
		LoadLevel ("MainMenu");
	}

	void OnLevelWasLoaded(int level) {
		uSceneManager = (SceneManager) GameObject.FindObjectOfType (typeof(SceneManager));
	}

	void Start() {
		// It doesn't appear that OnLevelWasLoaded runs for the MainMenu
		OnLevelWasLoaded (0);
	}

	/**
	 * If we're the server, stop it and unregister it.
	 * If we're a client, disconnect.
	 * 
	 * Then destroy the network manager object
	 */
	public void Shutdown() {
		if (Network.isServer) {
			if (!uGameHasStarted) {
				MasterServer.UnregisterHost ();
			}
		}
		Network.Disconnect();
	}

	/**
	 * This is used because if you don't reset the level prefix before loading a new scene
	 * the RPC system gets confused and routes the RPCs all over the place
	 * 
	 * I ran into this problem earlier which led to some ugly code where I store game state
	 * inside the players instead of the Game object
	 * 
	 * TODO: Go back and move stuff like the gameSetup to the Game object rather than
	 * the players
	 */
	public void LoadLevel(string pLevelName) {
		// Since we have all scenes let's just hardcode the possible level prefixes

		string[] levels = new string[]{
			"MainMenu", "Lobby", "Morning", "PropSelection", "Afternoon", "Evening", "Feedback", "EndOfGame"
		};

		Network.SetLevelPrefix(Array.IndexOf (levels, pLevelName));
		Application.LoadLevel(pLevelName);
	}
}