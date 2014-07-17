using UnityEngine;
using System;
using System.Collections;

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


	void Start () {
		// First we need to set everyone to "Not Ready"
		if (Network.isServer) {
			foreach (Player player in mNetworkManager.players) {
				player.networkView.RPC ("SetReady", RPCMode.All, false);
			}
		}

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

	[RPC] public void MoveToNextScene() {
		mDialogueManager.EndDialogue ();
		Application.LoadLevel ("PropSelection");
	}
}
