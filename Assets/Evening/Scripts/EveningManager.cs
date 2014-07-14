using UnityEngine;
using System.Collections;

public class EveningManager : SceneManager {

	NetworkManager mNetworkManager;
	RecordingPlayer mRecordingPlayer;
	GameObject mScreen;

	void Awake() {
		mNetworkManager = (NetworkManager) FindObjectOfType(typeof(NetworkManager));
		mRecordingPlayer = (RecordingPlayer) FindObjectOfType(typeof(RecordingPlayer));
		mScreen = GameObject.FindGameObjectWithTag("Screen");
	}
	void Start () {
		mRecordingPlayer.Play(mNetworkManager.myPlayer, mScreen);
	}
}
