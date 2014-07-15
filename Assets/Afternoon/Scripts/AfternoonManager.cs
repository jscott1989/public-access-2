using UnityEngine;
using System;
using System.Collections;

public class AfternoonManager : SceneManager {
	NetworkManager mNetworkManager;
	DialogueManager mDialogueManager;
	Countdown mCountdown;
	Game mGame;
	Recorder mRecorder;
	GameObject mScreen;

	public bool uRecording = false;

	public string uStateText {
		get {
			if (uRecording) {
				return "Recording";
			} else {
				return "Preparing";
			}
		}
	}

	void Awake() {
		mNetworkManager = (NetworkManager) FindObjectOfType(typeof(NetworkManager));
		mDialogueManager = (DialogueManager) FindObjectOfType(typeof(DialogueManager));
		mCountdown = (Countdown) FindObjectOfType(typeof(Countdown));
		mGame = (Game) FindObjectOfType (typeof(Game));
		mRecorder = (Recorder)FindObjectOfType(typeof(Recorder));
		mScreen = GameObject.FindGameObjectWithTag("Screen");
	}

	void Start () {
		// First we need to set everyone to "Not Ready"
		if (Network.isServer) {
			foreach (Player player in mNetworkManager.players) {
				player.networkView.RPC ("SetReady", RPCMode.All, false);
			}
		}

		string[] afternoon1Dialogue = new string[] {
			"Some stuff about afternoon...",
			"introduce it..."
		};

		Action afternoon1DialogueComplete =
			() => {
				mDialogueManager.StartDialogue("Waiting for other players to continue");
				mNetworkManager.myPlayer.networkView.RPC("SetReady", RPCMode.All, true);
		};

		mDialogueManager.StartDialogue(afternoon1Dialogue, afternoon1DialogueComplete);
	}

	/**
	 * This is called on the server when any player changes their ready status
	 */
	public override void ReadyStatusChanged(Player pPlayer) {
		if (pPlayer.uReady) {
			// Check if all players are ready
			if (!mGame.DEBUG_MODE) {
				foreach (Player p in mNetworkManager.players) {
					if (!p.uReady) {
						return;
					}
				}
			}
			
			// Everyone is ready, let's move on
			if (!uRecording) {
				// This must be the dialogue
				networkView.RPC ("StartPreparing", RPCMode.All);
			} else {
				// This must indicate everyone is finished recording - move on to the evening stage
				networkView.RPC ("MoveToNextScene", RPCMode.All);
			}
		}
	}

	[RPC] void StartPreparing() {
		if (Network.isServer) {
			// Set everyone to "not ready" so we can wait again after the recording stage
			foreach (Player player in mNetworkManager.players) {
				player.networkView.RPC ("SetReady", RPCMode.All, false);
			}
		}

		mDialogueManager.EndDialogue();

		uRecording = false;
		Action finishedPreparing =
			() => {
			StartRecording();
		};

		mCountdown.StartCountdown (mGame.PREPARING_COUNTDOWN, finishedPreparing);
	}

	void StartRecording() {
		mRecorder.StartRecording (mNetworkManager.myPlayer, mScreen);
		uRecording = true;
		Action finishedRecording =
			() => {
			mRecorder.StopRecording ();
			mDialogueManager.StartDialogue("Waiting for other players to finish recording");
			mNetworkManager.myPlayer.networkView.RPC("SetReady", RPCMode.All, true);
		};

		mCountdown.StartCountdown (mGame.RECORDING_COUNTDOWN, finishedRecording);
	}

	/**
	 * Move to the prop selection scene
	 */
	[RPC] void MoveToNextScene() {
		mDialogueManager.EndDialogue();
		Application.LoadLevel ("Evening");
	}
}
