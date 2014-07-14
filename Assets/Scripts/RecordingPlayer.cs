using UnityEngine;
using System.Collections;

public class RecordingPlayer : MonoBehaviour {
	Player mPlayingPlayer;
	GameObject mPlayingScreen;

	double mTime;
	double mLastPlayedTime;

	/**
	 * Start playing back pPlayer's latest episode on pScreen
	 */
	public void Play(Player pPlayer, GameObject pScreen) {
		mTime = 0;
		mLastPlayedTime = -1;
		mPlayingPlayer = pPlayer;
		mPlayingScreen = pScreen;
	}
	
	/**
	 * Jump the current playback to pTime
	 */
	public void Jump(float pTime) {
		
	}

	void Update() {
		if (mPlayingPlayer != null) {
			// We're playing
			mTime += Time.deltaTime;

			foreach(RecordingChange rc in mPlayingPlayer.uRecordingChanges) {
				if (rc.uTime > mTime) {
					mLastPlayedTime = mTime;
					return;
				} else if (rc.uTime > mLastPlayedTime) {
					// Needs activated
					rc.run (mPlayingScreen);
				}
			}
		}
	}
}
