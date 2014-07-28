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
	Countdown mCountdown;
	Game mGame;

	GameObject mMam;
	GameObject mSon;
	GameObject mDaughter;
	GameObject mGrandma;

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
			return mNetworkManager.myPlayer.uNeeds.Where (s => !s.StartsWith("-")).First();
		}
	}

	public string uDay2Dislike {
		get {
			if (mNetworkManager == null) {
				return "";
			}
			return mNetworkManager.myPlayer.uNeeds.Where (s => s.StartsWith("-")).Select (s => s.Substring(1)).First();
		}
	}

	public string uDay3Like {
		get {
			if (mNetworkManager == null) {
				return "";
			}
			return mNetworkManager.myPlayer.uNeeds.Where (s => !s.StartsWith("-")).ToArray()[1];
		}
	}

	public string uDay4Dislike {
		get {
			if (mNetworkManager == null) {
				return "";
			}
			return mNetworkManager.myPlayer.uNeeds.Where (s => s.StartsWith("-")).Select (s => s.Substring(1)).ToArray()[1];
		}
	}

	public string uDay5Like {
		get {
			if (mNetworkManager == null) {
				return "";
			}
			return mNetworkManager.myPlayer.uNeeds.Where (s => !s.StartsWith("-")).ToArray()[2];
		}
	}

	public string uTodaysLikesString {
		get {
			return string.Join("\n", uTodaysLikes);
		}
	}

	public string uTodaysDislikesString {
		get {
			return string.Join("\n", uTodaysDislikes);
		}
	}

	void Awake() {
		mNetworkManager = (NetworkManager) FindObjectOfType(typeof(NetworkManager));
		mRecordingPlayer = (RecordingPlayer) FindObjectOfType(typeof(RecordingPlayer));
		mCountdown = (Countdown) FindObjectOfType(typeof(Countdown));
		mScreen = GameObject.FindGameObjectWithTag("Screen");
		mGame = (Game) FindObjectOfType (typeof(Game));
		mDialogueManager = FindObjectOfType<DialogueManager>();

		mMam = GameObject.FindGameObjectWithTag("Mam");
		mSon = GameObject.FindGameObjectWithTag("Son");
		mDaughter = GameObject.FindGameObjectWithTag("Daughter");
		mGrandma = GameObject.FindGameObjectWithTag("Grandma");

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
			"In that case, you decide you're going to watch your favourite thing:",
			uDay1Like,
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
			"Today, your wife (wife's name) has joined you.",
			"She isn't particularly picky when it comes to watching TV, but she hates:",
			uDay2Dislike,
			"You'll still gain points when " + uDay1Like + " are on the screen, but will lose points whenever" + uDay2Dislike + " are on the screen."
		};
		
		Action day2DialogueComplete =
		() => {
			StartPreparing(mNetworkManager.myPlayer.uLastWatchedChannel);
		};
		
		mDialogueManager.StartDialogue(day2Dialogue, day2DialogueComplete);
	}
	void StartDay3() {
		string[] day3Dialogue = new string[]{
			"Today, your son (son's name) has joined you.",
			"Like any child his age, he's quite obsessed with:",
			uDay2Dislike,
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
			"(daughter's name), your daughter, has seen the fun that (son's name) is having, and has opted to join you this evening.",
			"She's quite a miserable child, and really dislikes:",
			uDay4Dislike,
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
			"Great... your mother in law has turned up unannounced. Right when you were settling down to watch some " + uDay1Like,
			"She's really into " + uDay5Like + " these days.",
			"Try to keep her happpy. It's worth points."
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
			mNetworkManager.myPlayer.NextDay ();
			if (mNetworkManager.myPlayer.uDay > Game.NUMBER_OF_DAYS) {
				// Move to the end of game
				mNetworkManager.LoadLevel ("EndOfGame");
			} else {
				mNetworkManager.LoadLevel ("Morning");
			}
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

	public void ChannelUp() {
		int nextChannel;
		if (mWatchingPlayer == (mNetworkManager.players.Length - 1)) {
			// Move to 0
			nextChannel = 0;
		} else {
			nextChannel = mWatchingPlayer + 1;
		}
//		if (nextChannel == mMyChannel) {
//			nextChannel += 1;
//		}
		ShowChannel(nextChannel);
	}

	public void ChannelDown() {
		int nextChannel;
		if (mWatchingPlayer == 0) {
			// Move to max
			nextChannel = mNetworkManager.players.Length - 1;
		} else {
			nextChannel = mWatchingPlayer - 1;
		}
//		if (nextChannel == mMyChannel) {
//			nextChannel -= 1;
//		}
		ShowChannel(nextChannel);
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

					Vector2 topLeftPosition = sprite.Position;
					Vector2 bottomRightPosition = new Vector2(topLeftPosition.x + sprite.Width, topLeftPosition.y + sprite.Height);


					// this checks is it visible on screen at all
					if (bottomRightPosition.x > 0 && bottomRightPosition.y > 0 && topLeftPosition.x < SCREEN_WIDTH && topLeftPosition.y < SCREEN_HEIGHT) {
						foreach(string need in uTodaysLikes) {
							if (p.uTags.Contains(need)) {
								AddScore(need);
							}
						}

						foreach(string need in uTodaysDislikes) {
							if (p.uTags.Contains(need)) {
								LoseScore(need);
							}
						}
					}
				}
			}
		}
	}

	/**
	 * Add to our viewer score for the given need
	 */
	void AddScore(string pNeed, int pNumber = 1) {
		// TODO: Show some output that the need has been met
		mNetworkManager.myPlayer.networkView.RPC ("AddWatchingScore", RPCMode.All, pNeed, pNumber);
	}

	/**
	 * Subtract to our viewer score for the given need
	 */
	void LoseScore(string pNeed, int pNumber = 1) {
		// TODO: Show some output that the need has been lost
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
			if (uTodaysLikes.Contains(tag)) {
				AddScore (tag, 10);
			}
			if (uTodaysDislikes.Contains(tag)) {
				LoseScore (tag, 10);
			}
		}
	}
}
