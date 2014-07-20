using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class FeedbackManager : SceneManager {

	Game mGame;
	Countdown mCountdown;
	RecordingPlayer mRecordingPlayer;
	DialogueManager mDialogueManager;
	NetworkManager mNetworkManager;
	GameObject mScreen;
	ViewerChart mViewerChart;

	void Awake() {
		mGame = FindObjectOfType<Game> ();
		mCountdown = FindObjectOfType<Countdown> ();
		mRecordingPlayer = FindObjectOfType<RecordingPlayer> ();
		mDialogueManager = FindObjectOfType<DialogueManager> ();
		mNetworkManager = FindObjectOfType<NetworkManager> ();
		mScreen = GameObject.FindGameObjectWithTag ("Screen");
		mViewerChart = FindObjectOfType<ViewerChart> ();
	}

	int[] GenerateViewerData() {
		int[] viewerData = new int[]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
		foreach(Player p in mNetworkManager.players) {
			// TODO: We can ignore our own player
			foreach(WatchedStationAction a in p.uWatchedStationActions) {
				if (a.uPlayer == mNetworkManager.myPlayer) {
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
		mNetworkManager.myPlayer.uDailyCreatorScore.Add (data.Sum());

		// First push the calculated viewers onto the chart
		mViewerChart.UpdateChart (mNetworkManager.players.Length, data); // TODO: put Length - 1 when we ban people watching their own show

		// If it's day 1 show an introduction
		if (mNetworkManager.myPlayer.uDay == 1) {
			string[] dialogue = new string[] {
				"Introduction to how feedback works...",
				"and what the user is looking for"
			};

			Action dialogueFinished =
				() => {
				StartFeedback();
			};
			mDialogueManager.StartDialogue(dialogue, dialogueFinished);
		} else {
			StartFeedback();
		}
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
		Application.LoadLevel ("PropSelection");
	}
}
