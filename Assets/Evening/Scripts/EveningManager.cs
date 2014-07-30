using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class EveningManager : SceneManager {

	const int SCREEN_WIDTH = 396;
	const int SCREEN_HEIGHT = 401;

	NetworkManager mNetworkManager;
	RecordingPlayer mRecordingPlayer;
	GameObject mScreen;
	dfControl mScreenControl;
	Countdown mCountdown;
	Game mGame;

	GameObject mDad;
	GameObject mMam;
	GameObject mSon;
	GameObject mDaughter;
	GameObject mGrandma;

	GameObject mUIRoot;

	GameObject mPointGainedIndicatorPrefab;

	DialogueManager mDialogueManager;

	// The player who's station we are watching
	int mWatchingPlayer = 0;
	float mInformationCountdown = -1;
	float mTime = 0;
	int mMyChannel = 0;

	const int PREPARING = 0;
	const int PLAYING = 1;

	int stage = 0;

	public bool uIsPreparing {
		get {
			return stage == PREPARING;
		}
	}

	public bool uShouldShowBadImage {
		get {
			return (uIsPreparing || mNetworkManager.playersOrderedByStation[mWatchingPlayer].isDisconnected);
		}
	}

	public string uStageText {
		get {
			if (uIsPreparing) {
				return "Preparing";
			}
			return "Watching";
		}
	}

	public bool uDay1HasPassed {
		get {
			if (mNetworkManager == null || mNetworkManager.myPlayer == null) {
				return false;
			}
			return mNetworkManager.myPlayer.uDay > 1;
		}
	}
	public bool uDay2HasPassed {
		get {
			if (mNetworkManager == null || mNetworkManager.myPlayer == null) {
				return false;
			}
			return mNetworkManager.myPlayer.uDay > 2;
		}
	}
	public bool uDay3HasPassed {
		get {
			if (mNetworkManager == null || mNetworkManager.myPlayer == null) {
				return false;
			}
			return mNetworkManager.myPlayer.uDay > 3;
		}
	}
	public bool uDay4HasPassed {
		get {
			if (mNetworkManager == null || mNetworkManager.myPlayer == null) {
				return false;
			}
			return mNetworkManager.myPlayer.uDay > 4;
		}
	}

	public int uTodaysScore {
		get {
			if (mNetworkManager == null) {
				return 0;
			} else if (mNetworkManager.myPlayer.uDailyWatchingScore.Count == 0) {
				return 0;
			}
			return mNetworkManager.myPlayer.uDailyWatchingScore.Last ();
		}
	}

	public string[] uTodaysLikes {
		get {
			if (mNetworkManager == null) {
				return new string[]{};
			}
			return mNetworkManager.myPlayer.uNeeds.Take (mNetworkManager.myPlayer.uDay).Where (s => !s.StartsWith("-")).ToArray();
		}
	}

	public string[] uTodaysDislikes {
		get {
			if (mNetworkManager == null) {
				return new string[]{};
			}
			return mNetworkManager.myPlayer.uNeeds.Take (mNetworkManager.myPlayer.uDay).Where (s => s.StartsWith("-")).Select (s => s.Substring(1)).ToArray();
		}
	}

	public string uDay1Like {
		get {
			if (mNetworkManager == null) {
				return "";
			}
			return mGame.uTagHumanReadable[mNetworkManager.myPlayer.uNeeds.Where (s => !s.StartsWith("-")).First()];
		}
	}

	public string uDay2Dislike {
		get {
			if (mNetworkManager == null) {
				return "";
			}
			return mGame.uTagHumanReadable[mNetworkManager.myPlayer.uNeeds.Where (s => s.StartsWith("-")).Select (s => s.Substring(1)).First()];
		}
	}

	public string uDay3Like {
		get {
			if (mNetworkManager == null) {
				return "";
			}
			return mGame.uTagHumanReadable[mNetworkManager.myPlayer.uNeeds.Where (s => !s.StartsWith("-")).ToArray()[1]];
		}
	}

	public string uDay4Dislike {
		get {
			if (mNetworkManager == null) {
				return "";
			}
			return mGame.uTagHumanReadable[mNetworkManager.myPlayer.uNeeds.Where (s => s.StartsWith("-")).Select (s => s.Substring(1)).ToArray()[1]];
		}
	}

	public string uDay5Like {
		get {
			if (mNetworkManager == null) {
				return "";
			}
			return mGame.uTagHumanReadable[mNetworkManager.myPlayer.uNeeds.Where (s => !s.StartsWith("-")).ToArray()[2]];
		}
	}

	void Awake() {
		mNetworkManager = (NetworkManager) FindObjectOfType(typeof(NetworkManager));
		mRecordingPlayer = (RecordingPlayer) FindObjectOfType(typeof(RecordingPlayer));
		mCountdown = (Countdown) FindObjectOfType(typeof(Countdown));
		mScreen = GameObject.FindGameObjectWithTag("Screen");
		mScreenControl = mScreen.GetComponent<dfPanel>();
		mGame = (Game) FindObjectOfType (typeof(Game));
		mDialogueManager = FindObjectOfType<DialogueManager>();

		mDad = GameObject.FindGameObjectWithTag("Dad");
		mMam = GameObject.FindGameObjectWithTag("Mam");
		mSon = GameObject.FindGameObjectWithTag("Son");
		mDaughter = GameObject.FindGameObjectWithTag("Daughter");
		mGrandma = GameObject.FindGameObjectWithTag("Grandma");

		mUIRoot = GameObject.FindGameObjectWithTag("UIRoot");

		mPointGainedIndicatorPrefab = (GameObject)Resources.Load ("Evening/Prefabs/PointGainedIndicator");

	}
	void Start () {
		mNetworkManager.myPlayer.uDailyWatchingScore.Add (0);
		if (mNetworkManager.myPlayer.uDay == 1) {
			StartDay1();
		} else if (mNetworkManager.myPlayer.uDay == 2) {
			StartDay2();
		} else if (mNetworkManager.myPlayer.uDay == 3) {
			StartDay3();
		} else if (mNetworkManager.myPlayer.uDay == 4) {
			StartDay4();
		} else if (mNetworkManager.myPlayer.uDay == 5) {
			StartDay5();
		}
	}

	void StartDay1() {
		string[] day1Dialogue = new string[]{
			"You arrive home after a hard day's work.",
			"As usual, you're going to spend your evening watching television.",
			"This evening, it seems you have the TV all to yourself",
			"In that case, you decide you're going to watch your favourite thing: " + uDay1Like,
			"You will get points while " + uDay1Like + " are on the screen",
			"Flick through the channels using the UP and DOWN arrows to look for things that you like."
		};

		Action day1DialogueComplete =
			() => {
			// This is our first time in Evening, so we need to decide which channel to start with
			mMyChannel = Array.IndexOf (mNetworkManager.playersOrderedByStation, mNetworkManager.myPlayer);
			if (mMyChannel == 0) {
				// If I'm the first channel then we should start on the last channel
				StartPreparing(mNetworkManager.players.Length - 1);
			} else {
				// Otherwise start on myChannel - 1
				StartPreparing(mMyChannel - 1);
			}
		};

		mDialogueManager.StartDialogue(day1Dialogue, day1DialogueComplete);

	}
	void StartDay2() {
		string[] day2Dialogue = new string[]{
			"Today, your wife " + mNetworkManager.myPlayer.uWifesName + " has joined you.",
			"She isn't particularly picky when it comes to watching TV, but she hates: " + uDay2Dislike,
			"You'll still gain points when " + uDay1Like + " are on the screen, but will lose points whenever " + uDay2Dislike + " are on the screen."
		};
		
		Action day2DialogueComplete =
		() => {
			StartPreparing(mNetworkManager.myPlayer.uLastWatchedChannel);
		};
		
		mDialogueManager.StartDialogue(day2Dialogue, day2DialogueComplete);
	}
	void StartDay3() {
		string[] day3Dialogue = new string[]{
			"Today, your son " + mNetworkManager.myPlayer.uSonsName + " has joined you.",
			"Like any child his age, he's quite obsessed with " + uDay3Like,
			"Now you need to try to ensure that you and your son are watching things you enjoy, while avoiding things your wife dislikes."
		};
		
		Action day3DialogueComplete =
		() => {
			StartPreparing(mNetworkManager.myPlayer.uLastWatchedChannel);
		};
		
		mDialogueManager.StartDialogue(day3Dialogue, day3DialogueComplete);
	}
	void StartDay4() {
		string[] day4Dialogue = new string[]{
			mNetworkManager.myPlayer.uDaughtersName + ", your daughter, has seen the fun that " + mNetworkManager.myPlayer.uSonsName + " is having, and has opted to join you this evening.",
			"She's quite a miserable child, and really dislikes " + uDay4Dislike,
			"When watching TV, try to take this into account along with your preference, and your wife and son's preferences."
		};
		
		Action day4DialogueComplete =
		() => {
			StartPreparing(mNetworkManager.myPlayer.uLastWatchedChannel);
		};
		
		mDialogueManager.StartDialogue(day4Dialogue, day4DialogueComplete);
	}
	void StartDay5() {
		string[] day5Dialogue = new string[]{
			"Great... your mother in law, " + mNetworkManager.myPlayer.uGrandmothersName + ", has turned up unannounced. Right when you were settling down to watch some " + uDay1Like,
			"She's really into " + uDay5Like + " these days.",
			"Try to keep her happy. It's worth points."
		};
		
		Action day5DialogueComplete =
		() => {
			StartPreparing(mNetworkManager.myPlayer.uLastWatchedChannel);
		};
		
		mDialogueManager.StartDialogue(day5Dialogue, day5DialogueComplete);
	}

	/**
	 * Start preparing, with the channel on pChannelNumber
	 */
	void StartPreparing(int pChannelNumber) {
		// Here we should have 5 seconds of static just so people can get to the station they want
		stage = PREPARING;
		Action preparingFinished = 
		() => {
			StartPlaying();
		};
		mCountdown.StartCountdown(Game.EVENING_PREPARING_COUNTDOWN, preparingFinished);
		ShowChannel(pChannelNumber);
	}

	void StartPlaying() {
		mTime = 0;
		stage = PLAYING;
		mNetworkManager.myPlayer.StartWatchingStation(mNetworkManager.playersOrderedByStation[mWatchingPlayer].uID.ToString(), "0");
		mRecordingPlayer.Play(mNetworkManager.playersOrderedByStation[mWatchingPlayer], mScreen);
		Action eveningFinished = 
		() => {
			mNetworkManager.myPlayer.networkView.RPC ("NextDay", RPCMode.All);
			mNetworkManager.LoadLevel ("Morning");
		};
		mCountdown.StartCountdown(Game.RECORDING_COUNTDOWN, eveningFinished);
	}

	void ShowChannel(int pChannelNumber) {
		mNetworkManager.myPlayer.uLastWatchedChannel = pChannelNumber;
		uStationInformationIsVisible = true;
		mWatchingPlayer = pChannelNumber;
		mInformationCountdown = Game.CHANNEL_INFORMATION_COUNTDOWN;
		if (!uIsPreparing) {
			mRecordingPlayer.Play(mNetworkManager.playersOrderedByStation[mWatchingPlayer], mScreen);
			mRecordingPlayer.Jump(mTime);
			mNetworkManager.myPlayer.StartWatchingStation(mNetworkManager.playersOrderedByStation[mWatchingPlayer].uID.ToString(), mTime.ToString ());
		}
	}

	public int GetNextChannelUp(int pCurrentChannel) {
		if (pCurrentChannel >= (mNetworkManager.players.Length - 1)) {
			return 0;
		} else {
			return pCurrentChannel + 1;
		}
	}

	public int GetNextChannelDown(int pCurrentChannel) {
		if (pCurrentChannel <= 0) {
			return (mNetworkManager.players.Length - 1);
		} else {
			return pCurrentChannel - 1;
		}
	}

	public void ChannelUp() {
		if (mNetworkManager.players.Length == 1) {
			ShowChannel (0);
			return;
		}
		int pNextChannel = GetNextChannelUp(mWatchingPlayer);
		while (pNextChannel == mMyChannel) {
			pNextChannel = GetNextChannelUp(pNextChannel);
		}
		ShowChannel(pNextChannel);
	}

	public void ChannelDown() {
		if (mNetworkManager.players.Length == 1) {
			ShowChannel (0);
			return;
		}
		int pNextChannel = GetNextChannelDown(mWatchingPlayer);
		while (pNextChannel == mMyChannel) {
			pNextChannel = GetNextChannelDown(pNextChannel);
		}
		ShowChannel(pNextChannel);
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			ChannelUp();
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			ChannelDown();
		}

		if (mInformationCountdown != -1) {
			mInformationCountdown -= Time.deltaTime;

			if (mInformationCountdown <= 0) {
				mInformationCountdown = -1;
				uStationInformationIsVisible = false;
			}
		}

		if (stage == PLAYING) {
			mTime += Time.deltaTime;

			CheckScore();
		}
	}

	float mCheckScoreTimeout = 0;

	/**
	 * This will check what "needs" are currently active, and add appropriate scores for those present
	 */
	void CheckScore() {
		if (stage == PLAYING) {
			mCheckScoreTimeout += Time.deltaTime;

			if (mCheckScoreTimeout >= Game.CHECKSCORE_TIMEOUT) {
				mCheckScoreTimeout = 0;

				// Check what we can give score for
				foreach(PlayingProp p in mScreen.GetComponentsInChildren<PlayingProp>()) {
					dfControl sprite = p.gameObject.GetComponent<dfTextureSprite>();
					if (sprite == null) {
						sprite = p.gameObject.GetComponent<dfSlicedSprite>();
					}

					Vector3 topLeftPosition = new Vector3(sprite.Position.x, 0-sprite.Position.y);
					Vector3 bottomRightPosition = (Vector3)topLeftPosition + (Vector3)sprite.Size;

					// this checks is it visible on screen at all
					if (bottomRightPosition.x > 0 && bottomRightPosition.y > 0 && topLeftPosition.x < SCREEN_WIDTH && topLeftPosition.y < SCREEN_HEIGHT) {
						for(int i = 0; i < uTodaysLikes.Length; i++) {
							if (p.uTags.Contains(uTodaysLikes[i])) {
								AddScore(i, uTodaysLikes[i], sprite);
							}
						}

						for(int i = 0; i < uTodaysDislikes.Length; i++) {
							if (p.uTags.Contains(uTodaysDislikes[i])) {
								LoseScore(i, uTodaysDislikes[i], sprite);
							}
						}
					}
				}
			}
		}
	}

	/**
	 * Add to our viewer score for the given need
	 * pDay = the need's day NOT the current day
	 */
	void AddScore(int pDay, string pNeed, dfControl source, int pNumber = 1) {
		dfTextureSprite target = mDad.GetComponent<dfTextureSprite>();
		if (pDay == 1) {
			target = mSon.GetComponent<dfTextureSprite>();
		}
		if (pDay == 2) {
			target = mGrandma.GetComponent<dfTextureSprite>();
		}

		for(int i = 0; i < pNumber; i++) {
			GameObject g = (GameObject)Instantiate(mPointGainedIndicatorPrefab,Vector3.zero,Quaternion.identity);
			PointGainedIndicator p = g.GetComponent<PointGainedIndicator>();
			p.transform.parent = mUIRoot.transform;
			if (source == null) {
				g.GetComponent<dfTextureSprite>().Position = new Vector3(mScreenControl.Position.x + (mScreenControl.Width/2), mScreenControl.Position.y - (mScreenControl.Height/2));
			} else {
				g.GetComponent<dfTextureSprite>().Position = new Vector3(mScreenControl.Position.x + source.Position.x + (source.Width/2), mScreenControl.Position.y + source.Position.y - (source.Height/2));
			}
			p.MoveTo(target);
		}

		mNetworkManager.myPlayer.networkView.RPC ("AddWatchingScore", RPCMode.All, pNeed, pNumber);
	}

	/**
	 * Subtract to our viewer score for the given need
	 * pDay = the need's day NOT the current day
	 */
	void LoseScore(int pDay, string pNeed, dfControl source, int pNumber = 1) {
		dfTextureSprite target = mMam.GetComponent<dfTextureSprite>();
		if (pDay == 1) {
			target = mDaughter.GetComponent<dfTextureSprite>();
		}

		for(int i = 0; i < pNumber; i++) {
			GameObject g = (GameObject)Instantiate(mPointGainedIndicatorPrefab,Vector3.zero,Quaternion.identity);
			PointGainedIndicator p = g.GetComponent<PointGainedIndicator>();
			p.transform.parent = mUIRoot.transform;
			if (source == null) {
				g.GetComponent<dfTextureSprite>().Position = new Vector3(mScreenControl.Position.x + (mScreenControl.Width/2), mScreenControl.Position.y - (mScreenControl.Height/2));
			} else {
				g.GetComponent<dfTextureSprite>().Position = new Vector3(mScreenControl.Position.x + source.Position.x + (source.Width/2), mScreenControl.Position.y + source.Position.y - (source.Height/2));
			}
			p.SetNegative();
			p.MoveTo(target);
		}

		mNetworkManager.myPlayer.networkView.RPC ("LoseWatchingScore", RPCMode.All, pNeed, pNumber);
	}

	public bool uStationInformationIsVisible;
	public string uStationName {
		get {
			if (mNetworkManager == null) {
				return "";
			}
			return mNetworkManager.playersOrderedByStation[mWatchingPlayer].uStationName;
		}
	}

	public Texture uStationLogo {
		get {
			if (mNetworkManager == null) {
				return null;
			}
			return mNetworkManager.playersOrderedByStation[mWatchingPlayer].uStationLogo;
		}
	}

	public string uShowName {
		get {
			if (mNetworkManager == null) {
				return "";
			}
			return mNetworkManager.playersOrderedByStation[mWatchingPlayer].uShowName;
		}
	}

	public string uTheme {
		get {
			if (mNetworkManager == null) {
				return "";
			}
			return mNetworkManager.playersOrderedByStation[mWatchingPlayer].uTheme;
		}
	}

	public override void AudioPlayed(Audio pAudio) {
		foreach (string tag in pAudio.uTags) {
			for (int i = 0; i < uTodaysLikes.Length; i++) {
				if (uTodaysLikes[i] == tag) {
					AddScore (i, tag, null, 10);
				}
			}
			for (int i = 0; i < uTodaysDislikes.Length; i++) {
				if (uTodaysDislikes[i] == tag) {
					LoseScore (i, tag, null, 10);
				}
			}
		}
	}
}
