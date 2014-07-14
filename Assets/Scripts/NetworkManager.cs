using UnityEngine;
using System;
using System.Collections;

/**
 * This class sets up and then maintains the server/client
 */
public class NetworkManager : MonoBehaviour {
	// Unique game name for matchmaking
	private const string GAME_NAME = "PublicAccessWars1";
	// Maximum number of players
	private const int MAX_PLAYERS = 10;
	// The port to connect to
	private const int SERVER_PORT = 25000;

	private string mRoomName = "";

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

	private SceneManager mSceneManager;

	public int mMyClientID;

	public Player[] players {
		get {
			return (Player[])GameObject.FindObjectsOfType (typeof(Player));
		}
	}

	public Player myPlayer {
		get {
			foreach (Player player in players) {
				if (player.uID == mMyClientID) {
					return player;
				}
			}
			return null;
		}
	}

	public Player GetPlayerWithID(int pID) {
		foreach (Player player in players) {
			if (player.uID == pID) {
				return player;
			}
		}
		return null;
	}
	
	ErrorPanel mErrorPanel;

	void Awake() {
		// This is just for during testing - as the unity master server keeps banning me!
//		MasterServer.ipAddress = "127.0.0.1";
//		MasterServer.port = 23466;

		mErrorPanel = (ErrorPanel)GameObject.FindObjectOfType (typeof(ErrorPanel));
		mPlayerPrefab = (GameObject)Resources.Load ("Prefabs/Player");
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

		Network.InitializeServer (MAX_PLAYERS, SERVER_PORT, !Network.HavePublicAddress ());
		mRoomName = pRoomName;
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
		MasterServer.RegisterHost(GAME_NAME, mRoomName);
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
			mStartServerCallback();
		} else {
			// There has been an error setting up the server
			mErrorPanel.ShowError ("There was an error starting the server");
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
		// We don't care about this event really because if it's not registered with
		// the master server nobody can join anyway
	}

	/**
	 * Called on the server when a player connects
	 */
	void OnPlayerConnected(NetworkPlayer pPlayer) {
		if (uGameHasStarted) {
			Network.CloseConnection (pPlayer, true);
		} else {
			int id = Convert.ToInt16(pPlayer.ToString ());
			mSceneManager.PlayerConnected(id, pPlayer);
		}
	}

	/**
	 * Called on the server when a player disconnects
	 */
	void OnPlayerDisconnected(NetworkPlayer pPlayer) {
		int id = Convert.ToInt16(pPlayer.ToString ());
		mSceneManager.PlayerDisconnected(id, pPlayer);
		Network.RemoveRPCs(pPlayer);
		Network.DestroyPlayerObjects(pPlayer);
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
		foreach (Player player in players) {
			Destroy (player.gameObject);
		}
		
		Destroy (gameObject);

		Application.LoadLevel ("MainMenu");
	}

	void OnLevelWasLoaded(int level) {
		mSceneManager = (SceneManager) GameObject.FindObjectOfType (typeof(SceneManager));
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
}