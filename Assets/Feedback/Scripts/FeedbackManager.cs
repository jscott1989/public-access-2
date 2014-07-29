using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class FeedbackManager : SceneManager {
	Countdown mCountdown;
	RecordingPlayer mRecordingPlayer;
	DialogueManager mDialogueManager;
	NetworkManager mNetworkManager;
	GameObject mScreen;
	ViewerChart mViewerChart;

	void Awake() {
		mCountdown = FindObjectOfType<Countdown> ();
		mRecordingPlayer = FindObjectOfType<RecordingPlayer> ();
		mDialogueManager = FindObjectOfType<DialogueManager> ();
		mNetworkManager = FindObjectOfType<NetworkManager> ();
		mScreen = GameObject.FindGameObjectWithTag ("Screen");
		mViewerChart = FindObjectOfType<ViewerChart> ();
	}

	int[] GenerateViewerData() {
		int[] viewerData = new int[]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
		foreach(Player p in mNetworkManager.players.Where (p => p != mNetworkManager.myPlayer)) {
			foreach(WatchedStationAction a in p.uWatchedStationActions.Where(wsa => wsa.uPlayer == mNetworkManager.myPlayer)) {
				float endTime = a.uEndTime;
				if (endTime == -1) {
					endTime = 31;
				}
				// Check each second if it falls inside the watched time
				for(int i = 0; i < 30; i++) {
					if ((i >= a.uStartTime && i <= endTime) && (i + 1 >= a.uStartTime && i <= endTime)) {
						// This will show everyone who watched for at least one second
						viewerData[i] += 1;
					}
				}
			}
		}
		
		return viewerData;
	}

	void Start () {
		// First we need to set everyone to "Not Ready"
		if (Network.isServer) {
			foreach (Player player in mNetworkManager.players) {
				player.networkView.RPC ("SetReady", RPCMode.All, false);
			}
		}

		int[] data = GenerateViewerData();

		// Add today's total viewer seconds to the score record
		// TODO: This isn't a very good scoring mechanism - replace this with something more accurate
		mNetworkManager.myPlayer.networkView.RPC ("AddDailyCreatorScore", RPCMode.All, (data.Sum () * Game.CREATOR_SCORE_MULTIPLIER) .ToString ());

		// First push the calculated viewers onto the chart
		mViewerChart.UpdateChart (mNetworkManager.players.Length - 1, data);

		// If it's day 1 show an introduction
		if (mNetworkManager.myPlayer.uDay == 2) {
			StartFirstDay();
		} else  if (mNetworkManager.myPlayer.uDay == Game.NUMBER_OF_DAYS){
			StartLastDay();
		} else {
			StartMiddleDay();
		}
	}

	void StartFirstDay() {
		string[] dialogue = new string[] {
			"Here are last night's viewing figures",
			"Use them to try to figure out what the audience are looking for. So future shows can better match their tastes",
			"You can jump to any part of your show by clicking on the chart to see what was on screen at that time",
			"You have " + Game.FEEDBACK_COUNTDOWN + " seconds."
		};
		
		Action dialogueFinished =
		() => {
			StartFeedback();
		};
		mDialogueManager.StartDialogue(dialogue, dialogueFinished);
	}

	void StartMiddleDay() {
		string[] dialogue = new string[] {
			"Here are last night's viewing figures",
			"Use them to try to figure out what the audience are looking for. So future shows can better match their tastes",
		};
		
		Action dialogueFinished =
		() => {
			StartFeedback();
		};
		mDialogueManager.StartDialogue(dialogue, dialogueFinished);
	}

	void StartLastDay() {
		string[] dialogue = new string[] {
			"Here are last night's viewing figures",
			"It's too late to make changes to the show based on them. But it might be interesting to see"
		};
		
		Action dialogueFinished =
		() => {
			StartFeedback();
		};
		mDialogueManager.StartDialogue(dialogue, dialogueFinished);
	}

	void StartFeedback() {
		// Start the countdown
		Action countdownFinished =
			() => {
			mDialogueManager.StartDialogue("Waiting for other players to continue");
			mNetworkManager.myPlayer.networkView.RPC("SetReady", RPCMode.All, true);
		};
		mCountdown.StartCountdown (Game.FEEDBACK_COUNTDOWN, countdownFinished);
		
		// Start the playback
		mViewerChart.StartViewer (mRecordingPlayer);
		mRecordingPlayer.Play (mNetworkManager.myPlayer, mScreen, true);
	}

	/**
	 * This is called on the server when any player changes their ready status
	 */
	public override void ReadyStatusChanged(Player pPlayer) {
		if (pPlayer.uReady) {
			if (!Game.DEBUG_MODE) {
				// Check if all players are ready - if so we can start
				foreach (Player p in mNetworkManager.players) {
					if (!p.uReady) {
						return;
					}
				}
			}
			
			// Everyone is ready, let's move to the next scene
			networkView.RPC ("MoveToNextScene", RPCMode.All);
		}
	}

	public void ReadyButtonPressed() {
		// Once ready is pressed we need to block the rest of the scene, so we'll show a cancellable dialogue
		Action readyCancelled =
		() => {
			mNetworkManager.myPlayer.networkView.RPC("SetReady", RPCMode.All, false);
		};
		mDialogueManager.StartDialogue ("Waiting for other players...", readyCancelled, "Cancel");
		mNetworkManager.myPlayer.networkView.RPC("SetReady", RPCMode.All, true);
	}

	[RPC] public void MoveToNextScene() {
		mDialogueManager.EndDialogue ();
		mNetworkManager.LoadLevel ("PropSelection");
	}
}
