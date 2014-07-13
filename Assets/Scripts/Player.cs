using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/**
 * This class holds basic information about each player, and methods
 * to interact with the player
 */
public class Player : MonoBehaviour {

	// The player's ID
	public int uID;

	// This is used anywhere we need to wait for everyone to be ready before continuing
	public bool uReady = false;

	public int uBudget = 300;

	public Game mGame;
	public Dictionary<string, PurchasedProp> uPurchasedProps = new Dictionary<string, PurchasedProp>();

	Texture2D mReadyTexture;
	Texture2D mNotReadyTexture;

	public List<RecordingChange> uRecordingChanges = new List<RecordingChange>();

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

	public Prop[] uUnpurchasedProps {
		get {
			List<Prop> props = new List<Prop>();
			props.AddRange(uAvailableProps);
			foreach(KeyValuePair<string, PurchasedProp> p in uPurchasedProps) {
				PurchasedProp purchasedProp = p.Value;
				props.Remove (purchasedProp.uProp);
			}
			return props.ToArray();
		}
	}

	[RPC] public void SetReady(bool pReady) {
		uReady = pReady;
		if (Network.isServer) {
			mSceneManager.ReadyStatusChanged(this);
		}
	}

	public List<Prop> uAvailableProps = new List<Prop>();
	
	[RPC] public void AddAvailableProp(string pProp) {
		uAvailableProps.Add (mGame.uProps[pProp]);
	}

	public string uTheme;
	public string uNeed;

	[RPC] public void SetGameInfo (string pTheme, string pNeed) {
		uTheme = pTheme;
		uNeed = pNeed;
	}

	
	// The current scene manager
	private SceneManager mSceneManager;

	void Awake() {
		mReadyTexture = (Texture2D)Resources.Load ("Lobby/Images/ready");
		mNotReadyTexture = (Texture2D)Resources.Load ("Lobby/Images/not_ready");
		mGame = (Game)FindObjectOfType(typeof(Game));

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



	/**
	 * Ensure that I have at least X props (so that the recording stage is playable
	 */
	[RPC] void EnsureMinimumProps() {
		// TODO
	}

	[RPC] public void PurchaseProp(string pPropID) {
		Prop prop = mGame.uProps[pPropID];
		print("PURCHASING");
		// Check that there is a prop.uID available
		bool propAvailable = false;
		foreach(Prop p in uUnpurchasedProps) {
			if (p.uID == prop.uID) {
				propAvailable = true;
				break;
			}
		}

		print (propAvailable);

		if (!propAvailable) {
			return;
		}

		// Check that we have enough money
		if (uBudget < prop.uPrice) {
			return;
		}
		print ("Have budget");

		// Add the prop to our props, and take away the money
		PurchasedProp purchasedProp = new PurchasedProp(prop);
		uPurchasedProps.Add(purchasedProp.uID, purchasedProp);
		uBudget -= prop.uPrice;

		mSceneManager.PropPurchased(this, purchasedProp);

		print("DONE!");
		if (networkView.isMine) {
			networkView.RPC ("PurchaseProp", RPCMode.Others, prop.uID);
		}
	}

	[RPC] public void SellProp(string pPurchasedPropID) {
		if (!uPurchasedProps.ContainsKey(pPurchasedPropID)) {
			return;
		}

		// Remove the prop from purchases and add to the budget
		PurchasedProp p = uPurchasedProps[pPurchasedPropID];
		uPurchasedProps.Remove (pPurchasedPropID);
		uBudget += p.uProp.uPrice;

		mSceneManager.PropSold(this, p);

		if (networkView.isMine) {
			networkView.RPC ("SellProp", RPCMode.Others, p.uID);
		}
	}

	[RPC] public void RecordAction(string pActionType, string[] pParameters) {
		Type t = Type.GetType (pActionType);
		RecordingChange c = (RecordingChange)t.GetConstructors()[0].Invoke (pParameters);
		uRecordingChanges.Add (c);
	}
}
