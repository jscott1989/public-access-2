using UnityEngine;
using System;
using System.Collections;

public class EveningManager : SceneManager {

	NetworkManager mNetworkManager;
	RecordingPlayer mRecordingPlayer;
	GameObject mScreen;
	Countdown mCountdown;
	Game mGame;

	int watchingPlayerNumber = 0;

	void Awake() {
		mNetworkManager = (NetworkManager) FindObjectOfType(typeof(NetworkManager));
		mRecordingPlayer = (RecordingPlayer) FindObjectOfType(typeof(RecordingPlayer));
		mCountdown = (Countdown) FindObjectOfType(typeof(Countdown));
		mScreen = GameObject.FindGameObjectWithTag("Screen");
		mGame = (Game) FindObjectOfType (typeof(Game));
	}
	void Start () {
		Action eveningFinished = 
			() => {
			mNetworkManager.myPlayer.NextDay ();
			Application.LoadLevel ("Morning");
		};
		mCountdown.StartCountdown(mGame.RECORDING_COUNTDOWN, eveningFinished);
		mRecordingPlayer.Play(mNetworkManager.players[watchingPlayerNumber], mScreen);
	}

	public void Jump() {
		mRecordingPlayer.Jump (10);
	}

	public void ChannelUp() {
		watchingPlayerNumber += 1;
		if (watchingPlayerNumber >= mNetworkManager.players.Length) {
			watchingPlayerNumber = 0;
		}
		mRecordingPlayer.Play(mNetworkManager.players[watchingPlayerNumber], mScreen);
	}

	public void ChannelDown() {
		watchingPlayerNumber -= 1;
		if (watchingPlayerNumber < 0) {
			watchingPlayerNumber = mNetworkManager.players.Length - 1;
		}
		mRecordingPlayer.Play(mNetworkManager.players[watchingPlayerNumber], mScreen);
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			ChannelUp();
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			ChannelDown();
		}
	}
}
