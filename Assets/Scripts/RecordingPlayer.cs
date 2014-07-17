using UnityEngine;
using System.Collections;

public class RecordingPlayer : MonoBehaviour {
	Player mPlayingPlayer;
	GameObject mPlayingScreen;

	double mTime;
	double mLastPlayedTime;

	bool mLoop = false;

	public double uTime {
		get {
			return mTime;
		}
	}

	/**
	 * Start playing back pPlayer's latest episode on pScreen
	 */
	public void Play(Player pPlayer, GameObject pScreen, bool uLoop = false) {
		mPlayingScreen = pScreen;
		Reset();
		mLoop = uLoop;
		mPlayingPlayer = pPlayer;
	}

	/**
	 * Clear the screen
	 */
	public void Reset() {
		// Destroy all objects
		foreach(PlayingProp p in mPlayingScreen.GetComponentsInChildren(typeof(PlayingProp))) {
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
		Reset ();
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
			if (mTime > Game.RECORDING_COUNTDOWN) {
				// We're finished
				if (mLoop) {
					// Start again
					Reset();
				} else {
					// Stop playing
					mPlayingPlayer = null;
				}
			}

			PlayToCurrentTime();
		}
	}
}
