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
		Reset(pScreen);
		mPlayingPlayer = pPlayer;
		mPlayingScreen = pScreen;
	}

	/**
	 * Clear the screen
	 */
	public void Reset(GameObject mScreen) {
		// Destroy all objects
		foreach(PlayingProp p in mScreen.GetComponentsInChildren(typeof(PlayingProp))) {
			Destroy (p.gameObject);
		}
		// TODO: Stop all sounds, etc.

		// Reset the time
		mTime = 0;
		mLastPlayedTime = -1;
	}
	
	/**
	 * Jump the current playback to pTime
	 */
	public void Jump(float pTime) {
		Reset (mPlayingScreen);
//		mTime = pTime;
//		PlayToCurrentTime();
	}

	void PlayToCurrentTime() {
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

	void Update() {
		if (mPlayingPlayer != null) {
			// We're playing
			mTime += Time.deltaTime;

			PlayToCurrentTime();
		}
	}
}
