using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
 * This class just contains the basic logic to start the afternoon phase running
 */
public class AfternoonManager : MonoBehaviour {

	const float TIME_TO_CHANGE_CHANNEL = 0.3f;

	public int currentChannel = 0;

	private GameObject[] channels;
	public GameObject staticChannel;

	// This is used for the "static" effect between channel changes
	private float changingChannelTime = 0;
	private int nextChannel = 0;

	// This is used to record which channels were watched and when
	private float timePassed = 0;
	private List<ChannelWatchingAction> channelWatchingActions = new List<ChannelWatchingAction>();


	// Use this for initialization
	void Start () {
		// TODO: Have an instructions phase on Day 1 afternoon

		// TODO: Have a bit which introduces any additional "wants" prior to the playback starting

		// We put the channels into a fixed order
		channels = GameObject.FindGameObjectsWithTag ("channel");
		StartWatching (1);
	}

	public void StopWatching() {
		channelWatchingActions [channelWatchingActions.Count - 1].endTime = timePassed;

		// Stop watching all channels - turn to static
		foreach (GameObject channel in channels) {
			foreach (SpriteRenderer r in channel.GetComponentsInChildren(typeof(SpriteRenderer))) {
				r.enabled = false;
			}
		}
		
		// Then show static
		foreach (SpriteRenderer r in staticChannel.GetComponentsInChildren(typeof(SpriteRenderer))) {
			r.enabled = true;
		}
	}

	public void StartWatching(int channelNumber) {
		channelWatchingActions.Add (new ChannelWatchingAction (timePassed, channelNumber));
		nextChannel = 0;
//		 Then turn on this channel
		foreach (SpriteRenderer r in channels[channelNumber-1].GetComponentsInChildren(typeof(SpriteRenderer))) {
			r.enabled = true;
		}

		foreach (SpriteRenderer r in staticChannel.GetComponentsInChildren(typeof(SpriteRenderer))) {
			r.enabled = false;
		}

		// Then show the channel information for 5 seconds
	}

	public void ShowChannel(int channelNumber) {
		currentChannel = channelNumber;

		// We create a GameObject for each channel that contains all props for that channel
		// this allows us to set the visibility of the whole lot easily

		// First turn off all other channels
		StopWatching ();

		changingChannelTime = TIME_TO_CHANGE_CHANNEL;
		nextChannel = channelNumber;
	}

	public void ChannelUp() {
		if (currentChannel >= channels.Length) {
			ShowChannel (1);
		} else {
			ShowChannel(currentChannel + 1);
		}
	}

	public void ChannelDown() {
		if (currentChannel <= 1) {
			ShowChannel (channels.Length);
		} else {
			ShowChannel(currentChannel - 1);
		}
	}

	void Update() {
		timePassed += Time.deltaTime;

		if (nextChannel > 0) {
			changingChannelTime -= Time.deltaTime;
			if (changingChannelTime <= 0) {
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
