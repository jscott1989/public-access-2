using UnityEngine;
using System.Collections;

public class EveningManager : SceneManager {

	NetworkManager mNetworkManager;
	RecordingPlayer mRecordingPlayer;
	GameObject mScreen;

	int watchingPlayerNumber = 0;

	void Awake() {
		mNetworkManager = (NetworkManager) FindObjectOfType(typeof(NetworkManager));
		mRecordingPlayer = (RecordingPlayer) FindObjectOfType(typeof(RecordingPlayer));
		mScreen = GameObject.FindGameObjectWithTag("Screen");
	}
	void Start () {
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
