using UnityEngine;
using System.Collections;

/**
 * This script manages playback and recording of scenes
 */
public class SceneRecorder : MonoBehaviour {
	private float time;
	private Player recordingPlayer;
	private Player playingPlayer;
	private bool recording = false;
	private bool playing = false;

	/**
	 * Start recording to Player
	 */
	public void StartRecording(Player player) {
		Stop ();
		recordingPlayer = player;
		recording = true;

		// TODO: Record original position of props
	}

	/**
	 * Start playback of Player
	 */
	public void StartPlayback(Player player) {
		Stop ();
		playingPlayer = player;
		playing = true;

		// TODO: Destroy props

		// TODO: Create props in original positions
	}

	/**
	 * Jump to a particular time in the playback
	 */
	public void Jump(float time) {
		Stop ();

		// TODO: Destroy the props

		// TODO: Create the props in original positions

		// TODO: Run through all animations up to the current point

		// TODO: Figure out how to make sounds work
	}

	void Update() {
		time += Time.deltaTime;
		if (recording) {
			// Check if anything has changed, if so record them
		} else if (playing) {
			// Check if any new animations are available, if so activate them
		}
	}

	public void Stop() {
		// TODO: Ensure that the recordings are "tied up"
		recording = false;
		playing = false;
		time = 0;
		recordingPlayer = null;
		playingPlayer = null;
	}
}
