using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RecordingPlayer : MonoBehaviour {
	Player mPlayingPlayer;
	GameObject mPlayingScreen;

	double mTime;

	bool mLoop = false;
	bool mIsPaused = false;
	List<RecordingChange> mPlayedChanges = new List<RecordingChange>();

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

	public void Pause() {
		mIsPaused = true;
	}

	public void Continue() {
		mIsPaused = false;
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

		mPlayedChanges.Clear ();

		// Reset the time
		mTime = 0;

	}
	
	/**
	 * Jump the current playback to pTime
	 */
	public void Jump(float pTime) {
		Reset ();
		mTime = pTime;
		PlayToCurrentTime();
	}

	void PlayToCurrentTime() {
		foreach(RecordingChange rc in mPlayingPlayer.uRecordingChanges) {
			if (rc.uTime < mTime && !mPlayedChanges.Contains(rc)) {
				// Needs activated
				mPlayedChanges.Add (rc);
				rc.run (mPlayingScreen);
			}
		}
	}

	void Update() {
		if (mPlayingPlayer != null && !mIsPaused) {
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
