using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * A ChannelWatchingAction ecapsulates a single viewing of a channel
 * it has a channel, and a start and end time for the watching
 */
class ChannelWatchingAction {
	public float startTime;
	public float endTime;
	public int channelNumber;
	
	public ChannelWatchingAction(float start_time, int channel_number) {
		startTime = start_time;
		channelNumber = channel_number;
	}
}

/**
 * This class contains logic for the evening phase
 * It should be added to some object that will remain in the scene for the full evening phase
 * 
 * TODO: Some of the methods are a bit awkwardly named - Tidy it up
 */
public class EveningManager : MonoBehaviour {

	// The amount of time the static channel should be shown between channel changes
	const float TIME_TO_CHANGE_CHANNEL = 0.3f;

	// The amount of time the "channel information" is on screen
	const float TIME_TO_REMOVE_INFORMATION = 2.0f;

	// The current channel being watched
	public int currentChannel = 0;

	// The GameObject containing all elements for the channel
	// these must be in the correct order so that channels[0] is the first channel, channel[1] is the second, etc.
	public GameObject[] channels;

	// Set this is the unity UI to be the container for the "static" channel
	public GameObject staticChannel;

	// These are used to track when we are between channels
	private float changingChannelTime = 0;
	private int nextChannel = 0;

	// This is used to time out the removal of the channel information
	private float removingChannelInformationTime = 0;

	// This is used to record which channels were watched and when
	private float timePassed = 0;
	private List<ChannelWatchingAction> channelWatchingActions = new List<ChannelWatchingAction>();

	public Countdown countdown;


	void Start () {
		// TODO: Have an instructions phase on Day 1 evening

		// TODO: Have a bit which introduces "wants" prior to the playback starting

		// We put the channels into a fixed order
		// TODO: Replace this with the actual channels from the network
		// This is commented out so we can fix the order from the UI for testing - this will be removed properly
		// when networking is working
//		channels = GameObject.FindGameObjectsWithTag ("channel");

		// Start with channel 1
		// TODO: Make this something like (mychannelnumber - 1) so that no channel is given an unfair advantage
		StartWatching (1);

		Action eveningFinished = 
			() => Application.LoadLevel ("Morning");
		countdown.StartCountdown (30, eveningFinished);
	}

	void RemoveChannelInformation() {
		GameObject channel = channels [channelWatchingActions [channelWatchingActions.Count - 1].channelNumber - 1];

		foreach (GUIText r in channel.GetComponentsInChildren(typeof(GUIText))) {
			r.enabled = false;
		}
	}

	public void StopChannel(GameObject channel) {
		// Hide props
		foreach (SpriteRenderer r in channel.GetComponentsInChildren(typeof(SpriteRenderer))) {
			r.enabled = false;
		}

		// Hide descriptions
		foreach (GUIText r in channel.GetComponentsInChildren(typeof(GUIText))) {
			r.enabled = false;
		}

		// Hide 3D Text
		foreach (MeshRenderer r in channel.GetComponentsInChildren(typeof(MeshRenderer))) {
//			r.enabled = false;
		}

		// Silence audio
		foreach (AudioSource r in channel.GetComponentsInChildren(typeof(AudioSource))) {
			r.volume = 0;
		}
	}

	public void StartChannel(GameObject channel) {
		// Show props
		foreach (SpriteRenderer r in channel.GetComponentsInChildren(typeof(SpriteRenderer))) {
			r.enabled = true;
		}

		// Start audio
		foreach (AudioSource r in channel.GetComponentsInChildren(typeof(AudioSource))) {
			r.volume = 1;
		}

		// Show 3D Text
		foreach (MeshRenderer r in channel.GetComponentsInChildren(typeof(MeshRenderer))) {
			r.enabled = true;
		}


		// Show channel information
		foreach (GUIText r in channel.GetComponentsInChildren(typeof(GUIText))) {
			r.enabled = true;
		}
		removingChannelInformationTime = TIME_TO_REMOVE_INFORMATION;
	}

	public void StopWatchingCurrentChannel() {
		// Set the most recent ChannelWatchingAction end time
		channelWatchingActions [channelWatchingActions.Count - 1].endTime = timePassed;

		// Stop watching current channel
		StopChannel(channels[channelWatchingActions [channelWatchingActions.Count - 1].channelNumber - 1]);
		
		// Then show static
		StartChannel(staticChannel);
	}

	public void StartWatching(int channelNumber) {
		channelWatchingActions.Add (new ChannelWatchingAction (timePassed, channelNumber));
		currentChannel = channelNumber;
		nextChannel = 0;

		StartChannel (channels [channelNumber - 1]);
		StopChannel (staticChannel);
	}

	public void TurnToChannel(int channelNumber) {
		// First turn off current channel
		StopWatchingCurrentChannel ();

		// Then start the countdown until the next channel will start showing
		changingChannelTime = TIME_TO_CHANGE_CHANNEL;
		nextChannel = channelNumber;
	}

	public void ChannelUp() {
		if (currentChannel >= channels.Length) {
			TurnToChannel (1);
		} else {
			TurnToChannel(currentChannel + 1);
		}
	}

	public void ChannelDown() {
		if (currentChannel <= 1) {
			TurnToChannel (channels.Length);
		} else {
			TurnToChannel(currentChannel - 1);
		}
	}

	void Update() {
		// Count time just so we can track when channels are changed
		timePassed += Time.deltaTime;

		// Remove the "channel information" after a second
		if (removingChannelInformationTime > 0) {
			removingChannelInformationTime -= Time.deltaTime;

			if (removingChannelInformationTime <= 0) {
				RemoveChannelInformation();
			}
		}

		if (nextChannel > 0) { // If we are in the middle of changing the channel
			// Count down until we finish the channel change
			changingChannelTime -= Time.deltaTime;
			if (changingChannelTime <= 0) {
				// If the countdown is complete - turn on the next channel
				StartWatching(nextChannel);
			}
		} else {
			// Just check for up arrow/down arrow to change channel
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				ChannelUp ();
			} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
				ChannelDown ();
			}
		}
	}
}
