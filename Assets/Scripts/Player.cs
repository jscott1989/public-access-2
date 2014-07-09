using UnityEngine;
using System.Collections;

/**
 * This class holds basic information about each player, and methods
 * to interact with the player
 */
public class Player : MonoBehaviour {

	// The player's ID
	public int uID;

	// This is used anywhere we need to wait for everyone to be ready before continuing
	public bool uReady = false;

	Texture2D mReadyTexture;
	Texture2D mNotReadyTexture;

	/**
	 * I don't think this should be here - as it's lobby specific - but I'm not sure 
	 * where else to put it right now - move it if you think of somewhere better
	 */
	public Texture2D uReadyTexture {
		get {
			if (uReady) {
				return mReadyTexture;
			}
			return mNotReadyTexture;
		}
	}

	// The player's display name
	private string _name;
	public string uName {
		get {
			return _name;
		}
		set {
			SetInfo(uID, value);
		}
	}

	[RPC] public void SetReady(bool pReady) {
		uReady = pReady;
		if (Network.isServer) {
			mSceneManager.ReadyStatusChanged(this);
		}
	}

	
	// The current scene manager
	private SceneManager mSceneManager;

	void Awake() {
		mReadyTexture = (Texture2D)Resources.Load ("Lobby/Images/ready");
		mNotReadyTexture = (Texture2D)Resources.Load ("Lobby/Images/not_ready");

		// Ensure we configure ourselves for the level we're created on
		OnLevelWasLoaded (0);

		// Persist the Player between scenes
		DontDestroyOnLoad(gameObject);
	}

	void OnLevelWasLoaded(int level) {
		mSceneManager = (SceneManager) GameObject.FindObjectOfType (typeof(SceneManager));
	}
	
	void Start() {
		mSceneManager.NewPlayer (this);
	}

	void OnDestroy() {
		mSceneManager.PlayerLeaves (this);
	}

	/**
	 * Use this method to set basic information for the player
	 */
	[RPC] public void SetInfo(int pID, string pName) {
		uID = pID;
		_name = pName;

		if (networkView.isMine) {
			networkView.RPC ("SetInfo", RPCMode.Others, pID, pName);
		}
	}

	/**
	 * Send my info to another network player
	 */
	public void SendInfoTo(NetworkPlayer pPlayer) {
		networkView.RPC ("SetInfo", pPlayer, uID, uName);
	}
}
