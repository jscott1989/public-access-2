using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class EveningManager : SceneManager {

	NetworkManager mNetworkManager;
	RecordingPlayer mRecordingPlayer;
	GameObject mScreen;
	Countdown mCountdown;
	Game mGame;

	// The player who's station we are watching
	int mWatchingPlayer = 0;
	float mInformationCountdown = -1;
	float mTime = 0;
	int mMyChannel = 0;

	const int PREPARING = 0;
	const int PLAYING = 1;

	int stage = 0;

	void Awake() {
		mNetworkManager = (NetworkManager) FindObjectOfType(typeof(NetworkManager));
		mRecordingPlayer = (RecordingPlayer) FindObjectOfType(typeof(RecordingPlayer));
		mCountdown = (Countdown) FindObjectOfType(typeof(Countdown));
		mScreen = GameObject.FindGameObjectWithTag("Screen");
		mGame = (Game) FindObjectOfType (typeof(Game));
	}
	void Start () {

		// TODO: Add introductory text, to explain how to play this stage and importantly to tell the player what they are looking for

		if (mNetworkManager.myPlayer.uLastWatchedChannel < 0) {
			// This is our first time in Evening, so we need to decide which channel to start with
			mMyChannel = Array.IndexOf (mNetworkManager.playersOrderedByStation, mNetworkManager.myPlayer);
			if (mMyChannel == 0) {
				// If I'm the first channel then we should start on the last channel
				StartPreparing(mNetworkManager.players.Length - 1);
			} else {
				// Otherwise start on myChannel - 1
				StartPreparing(mMyChannel - 1);
			}
		}
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
		mNetworkManager.myPlayer.uDailyWatchingScore.Add(0);
		mTime = 0;
		stage = PLAYING;
		mRecordingPlayer.Play(mNetworkManager.playersOrderedByStation[mWatchingPlayer], mScreen);
		Action eveningFinished = 
		() => {
			mNetworkManager.myPlayer.NextDay ();
			if (mNetworkManager.myPlayer.uDay > Game.NUMBER_OF_DAYS) {
				// Move to the end of game
				Application.LoadLevel ("EndOfGame");
			} else {
				Application.LoadLevel ("Morning");
			}
		};
		mCountdown.StartCountdown(Game.RECORDING_COUNTDOWN, eveningFinished);
	}

	void ShowChannel(int pChannelNumber) {
		uStationInformationIsVisible = true;
		mWatchingPlayer = pChannelNumber;
		mInformationCountdown = Game.CHANNEL_INFORMATION_COUNTDOWN;
		mRecordingPlayer.Play(mNetworkManager.playersOrderedByStation[mWatchingPlayer], mScreen);
		mRecordingPlayer.Jump(mTime);
		mNetworkManager.myPlayer.StartWatchingStation(mNetworkManager.playersOrderedByStation[mWatchingPlayer].uID.ToString(), mTime.ToString ());
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
				foreach(Prop p in mScreen.GetComponentsInChildren<PlayingProp>()) {
					// TODO: First check that they are actually visible on screen at all
					// but for now let's just give points for all of them
					if (p.uTags.Contains(mNetworkManager.myPlayer.uNeed)) {
						AddScore(mNetworkManager.myPlayer.uNeed);
					}
				}
			}
		}
	}

	/**
	 * Add to our viewer score for the given need
	 */
	void AddScore(string need) {
		// TODO: Show some output that the need has been met
		mNetworkManager.myPlayer.uDailyWatchingScore.Last () += 1;
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
}
