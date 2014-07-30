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
	bool _ready = false;

	public bool isDisconnected = false;

	public bool uReady {
		get {
			return (isDisconnected || _ready);
		}

		set {
			_ready = value;
		}
	}

	public int uDay = 1;

	public int uBudget = 300;

	public string uShowName = "";

	public int uLastWatchedChannel = -1;

	public List<int> uDailyCreatorScore = new List<int>();
	public List<int> uDailyWatchingScore = new List<int>();

	public Dictionary<string, int> uScoreFromWatching = new Dictionary<string, int>();
	public Dictionary<string, int> uScoreLostFromWatching = new Dictionary<string, int>();

	public string uFormattedDailyNeed(int day) {
		if (day == 0) {
			return uName + " likes " + uNeeds[0];
		}
		if (day == 1) {
			return uWifesName + " dislikes " + uNeeds[1].Substring(1);
		}
		if (day == 2) {
			return uSonsName + " likes " + uNeeds[2];
		}
		if (day == 3) {
			return uDaughtersName + " dislikes " + uNeeds[3].Substring(1);
		}
		if (day == 4) {
			return uGrandmothersName + " likes " + uNeeds[4];
		}
		return "";
	}
	/**
	 * Record that a point has been gained from watching TV
	 */
	[RPC] public void AddWatchingScore(string pNeed, int pNumber) {
		while (uDailyWatchingScore.Count < uDay) {
			uDailyWatchingScore.Add (0);
		}

		uDailyWatchingScore[uDailyWatchingScore.Count - 1] += pNumber;

		if (!uScoreFromWatching.ContainsKey(pNeed)) {
			uScoreFromWatching[pNeed] = pNumber;
		} else {
			uScoreFromWatching[pNeed] += pNumber;
		}
	}

	/**
	 * Record that a point has been gained from watching TV
	 */
	[RPC] public void LoseWatchingScore(string pNeed, int pNumber) {
		while (uDailyWatchingScore.Count < uDay) {
			uDailyWatchingScore.Add (0);
		}
		
		uDailyWatchingScore[uDailyWatchingScore.Count - 1] -= pNumber;
		
		if (!uScoreLostFromWatching.ContainsKey(pNeed)) {
			uScoreLostFromWatching[pNeed] = pNumber;
		} else {
			uScoreLostFromWatching[pNeed] += pNumber;
		}
	}

	[RPC] public void HasDisconnected() {
		int playersRemaining = mNetworkManager.players.Where(p => !p.isDisconnected).Count ();
		if (playersRemaining < Game.MINIMUM_PLAYERS) {
			Action c = () => {
				mNetworkManager.ReturnToMainMenu();
			};
			FindObjectOfType<ErrorPanel>().ShowError(uName + " has disconnected. There are too few players to continue. The game will now quit.", c);
		} else {
			Action c = () => {};
			FindObjectOfType<ErrorPanel>().ShowError(uName + " has disconnected", c);
			isDisconnected = true;
		}
	}

	/**
	 * Record the total points gained from views at the end of a day
	 */
	[RPC] public void AddDailyCreatorScore(string pScore) {
		int score = int.Parse(pScore);
		uDailyCreatorScore.Add (score);
	}

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

	public int uOverallScore {
		get {
			return uCreatorScore + uWatcherScore;
		}
	}

	public int uCreatorScore {
		get {
			return uDailyCreatorScore.Sum ();
		}
	}

	public int uWatcherScore {
		get {
			return uDailyWatchingScore.Sum ();
		}
	}

	public Station uSelectedStation;

	public string uStationName {
		get {
			if (uSelectedStation == null) {
				return null;
			}
			return uSelectedStation.uName;
		}
	}

	public Texture2D uStationLogo {
		get {
			if (uSelectedStation == null) {
				return null;
			}
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

	public Backdrop[] uUnpurchasedBackdrops {
		get {
			List<Backdrop> backdrops = new List<Backdrop>();
			backdrops.AddRange(uAvailableBackdrops);
			foreach(KeyValuePair<string, PurchasedProp> p in uPurchasedProps) {
				Backdrop backdrop = p.Value.uProp as Backdrop;
				if (backdrop != null) {
					backdrops.Remove (backdrop);
				}
			}
			return backdrops.ToArray();
		}
	}

	public Audio[] uUnpurchasedAudio {
		get {
			List<Audio> audios = new List<Audio>();
			audios.AddRange(uAvailableAudio);
			foreach(KeyValuePair<string, PurchasedProp> p in uPurchasedProps) {
				Audio a = p.Value.uProp as Audio;
				if (a != null) {
					audios.Remove (a);
				}
			}
			return audios.ToArray();
		}
	}
	
	[RPC] public void SetReady(bool pReady) {
		uReady = pReady;
		if (Network.isServer) {
			mSceneManager.ReadyStatusChanged(this);
		}
	}

	public List<Prop> uAvailableProps = new List<Prop>();
	public List<Backdrop> uAvailableBackdrops = new List<Backdrop>();
	public List<Audio> uAvailableAudio = new List<Audio>();

	public string uBossName;
	public string uOldTheme;
	public string uTheme;
	public string[] uNeeds;
	public string uSonsName;
	public string uDaughtersName;
	public string uWifesName;
	public string uGrandmothersName;

	[RPC] public void ClearRecording() {
		uRecordingChanges.Clear ();
	}

	[RPC] public void SetGameInfo (string pOldTheme, string pBossName, string pSonsName, string pFemaleNamesString, string pTheme, string pNeeds, string pPropsString, string pBackdropsString, string pAudioString) {
		uAvailableProps.Clear ();
		uAvailableBackdrops.Clear ();
		string[] propIDs = RPCEncoder.Decode(pPropsString);
		string[] backdropIDs = RPCEncoder.Decode(pBackdropsString);
		string[] audioIDs = RPCEncoder.Decode(pAudioString);
		string[] needs = RPCEncoder.Decode(pNeeds);
		string[] femaleNames = RPCEncoder.Decode(pFemaleNamesString);
		foreach(var pID in propIDs) {
			uAvailableProps.Add (mGame.uProps[pID]);
		}
		foreach(var bID in backdropIDs) {
			uAvailableBackdrops.Add (mGame.uBackdrops[bID]);
		}

		foreach(var aID in audioIDs) {
			uAvailableAudio.Add (mGame.uAudio[aID]);
		}
		uOldTheme = pOldTheme;
		uBossName = pBossName;
		uSonsName = pSonsName;
		uDaughtersName = femaleNames[0];
		uWifesName = femaleNames[1];
		uGrandmothersName = femaleNames[2];
		uTheme = pTheme;
		uNeeds = needs;
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

	[RPC] public void PurchaseBackdrop(string pBackdropID) {
		Backdrop prop = mGame.uBackdrops[pBackdropID];
		// Check that there is a prop.uID available
		bool propAvailable = false;
		foreach(Prop p in uUnpurchasedBackdrops) {
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
		PurchasedBackdrop purchasedProp = new PurchasedBackdrop(prop);
		uPurchasedProps.Add(purchasedProp.uID, purchasedProp);
		uBudget -= prop.uPrice;
		
		mSceneManager.PropPurchased(this, purchasedProp);
		
		// I don't think we need to sync buying and selling
		//		if (networkView.isMine) {
		//			networkView.RPC ("PurchaseProp", RPCMode.Others, prop.uID);
		//		}
	}

	[RPC] public void PurchaseAudio(string pAudioID) {
		Audio prop = mGame.uAudio[pAudioID];
		// Check that there is a prop.uID available
		bool propAvailable = false;
		foreach(Prop p in uUnpurchasedAudio) {
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
		PurchasedAudio purchasedProp = new PurchasedAudio(prop);
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
		uBudget += mGame.uCashPerDay[uDay];
		uDay += 1;
	}

	public int[] GenerateLatestViewerData() {
		int[] viewerData = new int[]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
		foreach(Player p in mNetworkManager.players.Where (p => p != mNetworkManager.myPlayer)) {
			foreach(WatchedStationAction a in p.uWatchedStationActions.Where(wsa => wsa.uPlayer == mNetworkManager.myPlayer)) {
				float endTime = a.uEndTime;
				if (endTime == -1) {
					endTime = 31;
				}
				// Check each second if it falls inside the watched time
				for(int i = 0; i < 30; i++) {
					if ((i >= a.uStartTime && i <= endTime) && (i + 1 >= a.uStartTime && i <= endTime)) {
						// This will show everyone who watched for at least one second
						viewerData[i] += 1;
					}
				}
			}
		}
		
		return viewerData;
	}

	public List<WatchedStationAction> uWatchedStationActions = new List<WatchedStationAction>();

	[RPC] public void StartWatchingStation(string pPlayerID, string pStartTime) {
		// TODO: Clear on day 2

		if (uWatchedStationActions.Count > 0) {
			uWatchedStationActions.Last().uEndTime = float.Parse(pStartTime);
		}

		Player player = mNetworkManager.GetPlayerWithID(int.Parse(pPlayerID));
		float time = float.Parse(pStartTime);
		WatchedStationAction wsa = new WatchedStationAction(player, time);
		uWatchedStationActions.Add (wsa);
		if (networkView.isMine) {
			networkView.RPC ("StartWatchingStation", RPCMode.Others, pPlayerID, pStartTime);
		}
	}
}
