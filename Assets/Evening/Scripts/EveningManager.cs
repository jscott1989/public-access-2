using UnityEngine;
using System;
using System.Collections;

public class EveningManager : SceneManager {

	NetworkManager mNetworkManager;
	RecordingPlayer mRecordingPlayer;
	GameObject mScreen;
	Countdown mCountdown;
	Game mGame;

	// The player who's station we are watching
	int mWatchingPlayer = 0;
	float mInformationCountdown = -1;

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
			int myChannel = Array.IndexOf (mNetworkManager.playersOrderedByStation, mNetworkManager.myPlayer);
			if (myChannel == 0) {
				// If I'm the first channel then we should start on the last channel
				StartPreparing(mNetworkManager.players.Length - 1);
			} else {
				// Otherwise start on myChannel - 1
				StartPreparing(myChannel - 1);
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
		stage = PLAYING;
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
	}

	public void ChannelUp() {
		if (mWatchingPlayer == (mNetworkManager.players.Length - 1)) {
			// Move to 0
			ShowChannel(0);
		} else {
			ShowChannel(mWatchingPlayer + 1);
		}
	}

	public void ChannelDown() {
		if (mWatchingPlayer == 0) {
			// Move to max
			ShowChannel((mNetworkManager.players.Length - 1));
		} else {
			ShowChannel(mWatchingPlayer - 1);
		}
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			ChannelUp();
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			ChannelDown();
		}

		print(mInformationCountdown);
		if (mInformationCountdown != -1) {
			mInformationCountdown -= Time.deltaTime;

			if (mInformationCountdown <= 0) {
				mInformationCountdown = -1;
				uStationInformationIsVisible = false;
			}
		}

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
