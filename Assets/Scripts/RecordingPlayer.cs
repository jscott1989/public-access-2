using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RecordingPlayer : MonoBehaviour {
	Player mPlayingPlayer;
	GameObject mPlayingScreen;
	SceneManager mSceneManager;

	double mTime;

	bool mLoop = false;
	bool mIsPaused = false;
	List<RecordingChange> mPlayedChanges = new List<RecordingChange>();

	Game mGame;
	AudioSource mAudioSource;

	void Awake() {
		mGame = FindObjectOfType<Game>();
		mAudioSource = GetComponent<AudioSource>();
	}

	public void PlayAudio(string pID) {
		// TODO: Add score if possible
		mSceneManager.AudioPlayed(mGame.uAudio[pID]);
		mAudioSource.clip = mGame.uAudio[pID].uClip;
		mAudioSource.Play ();
	}

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

		// Then split it into a separate list per object
		Dictionary<string, List<RecordingChange>> partitionedChanges = new Dictionary<string, List<RecordingChange>>();

		foreach(RecordingChange rc in mPlayingPlayer.uRecordingChanges.Where (rc => rc.uTime < pTime)) {
			mPlayedChanges.Add (rc);
			if (!partitionedChanges.ContainsKey (rc.uID)) {
				partitionedChanges[rc.uID] = new List<RecordingChange>();
			}
			partitionedChanges[rc.uID].Add (rc);
		}

		foreach(KeyValuePair<string, List<RecordingChange>> kvp in partitionedChanges) {
			List<RecordingChange> changes = kvp.Value;

			IEnumerable<RecordingChange> runnableChanges = PerformPropJumpActions(changes);
			if (runnableChanges == null) {
				runnableChanges = PerformDialogueJumpActions(changes);
			}

			// now get the last size/zorder/position change and apply it
			Type[] changesToApply = new Type[]{typeof(SizeChange), typeof(ZOrderChange), typeof(PositionChange), typeof(DialogueTextChange), typeof(DialogueTextScaleChange)};

			foreach(Type t in changesToApply) {
				RecordingChange lastChange = runnableChanges.Where (rc => rc.GetType() == t).OrderByDescending(rc => rc.uTime).FirstOrDefault();
				if (lastChange != null) {
					lastChange.run (mPlayingScreen);
				}
			}
		}

		mTime = pTime;
	}

	IEnumerable<RecordingChange> PerformPropJumpActions(List<RecordingChange> changes) {
		// Now loop through each one, and find the latest "Create" action, and latest "destroy" action
		RecordingChange lastCreate = changes.Where (rc => rc.GetType() == typeof(InstantiationChange)).OrderByDescending(rc => rc.uTime).FirstOrDefault();
		RecordingChange lastDestroy = changes.Where (rc => rc.GetType() == typeof(DestroyChange)).OrderByDescending(rc => rc.uTime).FirstOrDefault();
		
		// if there is no create - or the destroy is after the create, then we don't do anything
		if (lastCreate == null) {
			return null;
		}
		
		if (lastDestroy != null && lastDestroy.uTime >= lastCreate.uTime) {
			return null;
		}
		
		// Run the last create
		lastCreate.run (mPlayingScreen);
		
		// re-filter the changes to remove any which occured before the creation
		return changes.Where (rc => rc.uTime >= lastCreate.uTime);
	}

	IEnumerable<RecordingChange> PerformDialogueJumpActions(List<RecordingChange> changes) {
		// Now loop through each one, and find the latest "Create" action, and latest "destroy" action
		RecordingChange lastCreate = changes.Where (rc => rc.GetType() == typeof(DialogueInstantiationChange)).OrderByDescending(rc => rc.uTime).FirstOrDefault();
		RecordingChange lastDestroy = changes.Where (rc => rc.GetType() == typeof(DestroyChange)).OrderByDescending(rc => rc.uTime).FirstOrDefault();
		
		// if there is no create - or the destroy is after the create, then we don't do anything
		if (lastCreate == null) {
			return null;
		}
		
		if (lastDestroy != null && lastDestroy.uTime >= lastCreate.uTime) {
			return null;
		}
		
		// Run the last create
		lastCreate.run (mPlayingScreen);
		
		// re-filter the changes to remove any which occured before the creation
		return changes.Where (rc => rc.uTime >= lastCreate.uTime);
	}

	void PlayToCurrentTime() {
		if (mPlayingPlayer != null && mPlayingPlayer.uRecordingChanges != null && mPlayedChanges != null) {
			foreach(RecordingChange rc in mPlayingPlayer.uRecordingChanges.Where (rc => rc.uTime < mTime && !mPlayedChanges.Contains (rc)).OrderBy(rc => rc.uTime)) {
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
