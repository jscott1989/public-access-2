using UnityEngine;
using System.Collections;

/**
 * This class holds basic information about each player, and methods
 * to interact with the player
 */
public class Player : MonoBehaviour {

	// The player's ID
	public int uID;

	// The player's display name
	public string _name; //TODO: Change this private when finished debugging
	public string uName {
		get {
			return _name;
		}
		set {
			SetInfo(uID, value);
		}
	}
	
	// The current scene manager
	private SceneManager mSceneManager;

	void Awake() {
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
