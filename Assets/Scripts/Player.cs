using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/**
 * This class holds basic information about each player, and methods
 * to interact with the player
 */
public class Player : MonoBehaviour {

	public delegate void AvailableStationsHaveChangedEvent();
	public event AvailableStationsHaveChangedEvent AvailableStationsHaveChanged;

	// The player's ID
	public int uID;

	// This is used anywhere we need to wait for everyone to be ready before continuing
	public bool uReady = false;

	public int uDay = 1;

	public int uBudget = 300;

	public string uShowName = "";

	public int uLastWatchedChannel = -1;

	public Game mGame;
	public Dictionary<string, PurchasedProp> uPurchasedProps = new Dictionary<string, PurchasedProp>();

	Texture2D mReadyTexture;
	Texture2D mNotReadyTexture;

	public List<RecordingChange> uRecordingChanges = new List<RecordingChange>();

	NetworkManager mNetworkManager;

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

	public Station uSelectedStation;

	public string uStationName {
		get {
			return uSelectedStation.uName;
		}
	}

	public Texture2D uStationLogo {
		get {
			return uSelectedStation.uLogo;
		}
	}

	public Station[] uAvailableStations {
		get {
			// First list stations which are already taken
			IEnumerable<string> takenStationIDS = (from player in mNetworkManager.players where (player.uID != mNetworkManager.myPlayer.uID && player.uSelectedStation.uID != Game.RANDOM_STATION_ID) select player.uSelectedStation.uID);
			return (from station in mGame.uStations where !(takenStationIDS.Contains(station.uID)) select station).ToArray ();
		}
	}

	public string[] uAvailableStationNames {
		get {
			return (from station in uAvailableStations select station.uName).ToArray();
		}
	}

	public int uSelectedStationIndex {
		get {
			Station[] s = uAvailableStations;
			for(int i = 0; i < s.Count(); i++) {
				if (s[i].uID == uSelectedStation.uID) {
					return i;
				}
			}
			return 0;
		}
		set {
			uSelectedStation = uAvailableStations[value];
			networkView.RPC ("SetSelectedStation", RPCMode.Others, uSelectedStation.uID);
		}
	}

	// This is called over the network when another player chooses a station
	[RPC] public void SetSelectedStation(string pSelectedStationID) {
		uSelectedStation = mGame.uStationsByID[pSelectedStationID];
		if (mNetworkManager.myPlayer.AvailableStationsHaveChanged != null) {
			mNetworkManager.myPlayer.AvailableStationsHaveChanged();
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

	public string uTheme;
	public string uNeed;

	[RPC] public void SetGameInfo (string pTheme, string pNeed, string pPropsString) {
		uAvailableProps.Clear ();
		string[] propIDs = RPCEncoder.Decode(pPropsString);
		foreach(var pID in propIDs) {
			uAvailableProps.Add (mGame.uProps[pID]);
		}

		uTheme = pTheme;
		uNeed = pNeed;
	}

	
	// The current scene manager
	private SceneManager mSceneManager;

	void Awake() {
		mReadyTexture = (Texture2D)Resources.Load ("Lobby/Images/ready");
		mNotReadyTexture = (Texture2D)Resources.Load ("Lobby/Images/not_ready");
		mGame = (Game)FindObjectOfType(typeof(Game));
		mNetworkManager = FindObjectOfType<NetworkManager>();

		uSelectedStation = mGame.uStationsByID[Game.RANDOM_STATION_ID];

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
		networkView.RPC ("SetSelectedStation", pPlayer, uSelectedStation.uID);
	}

	/**
	 * Ensure that I have at least X props (so that the recording stage is playable
	 */
	[RPC] void EnsureMinimumProps() {
		// TODO
	}

	[RPC] public void PurchaseProp(string pPropID) {
		Prop prop = mGame.uProps[pPropID];
		// Check that there is a prop.uID available
		bool propAvailable = false;
		foreach(Prop p in uUnpurchasedProps) {
			if (p.uID == prop.uID) {
				propAvailable = true;
				break;
			}
		}

		if (!propAvailable) {
			return;
		}

		// Check that we have enough money
		if (uBudget < prop.uPrice) {
			return;
		}

		// Add the prop to our props, and take away the money
		PurchasedProp purchasedProp = new PurchasedProp(prop);
		uPurchasedProps.Add(purchasedProp.uID, purchasedProp);
		uBudget -= prop.uPrice;

		mSceneManager.PropPurchased(this, purchasedProp);

		// I don't think we need to sync buying and selling
//		if (networkView.isMine) {
//			networkView.RPC ("PurchaseProp", RPCMode.Others, prop.uID);
//		}
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

		// I don't think we need to sync buying and selling
//		if (networkView.isMine) {
//			networkView.RPC ("SellProp", RPCMode.Others, p.uID);
//		}
	}

	[RPC] public void RecordAction(string pActionType, string pParametersString) {
		string[] pParameters = pParametersString.Split(',');
		Type t = Type.GetType (pActionType);
		RecordingChange c = (RecordingChange)t.GetConstructors()[0].Invoke (pParameters);
		uRecordingChanges.Add (c);
	}

	[RPC] public void SetShowName(string pShowName) {
		uShowName = pShowName;
	}

	public void NextDay() {
		uDay += 1;
	}
}
