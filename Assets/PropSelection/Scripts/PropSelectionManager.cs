using UnityEngine;
using System;
using System.Collections;

public class PropSelectionManager : SceneManager {
	NetworkManager mNetworkManager;
	DialogueManager mDialogueManager;
	Countdown mCountdown;

	// This is just to track where we are in the scene
	// 0 = the intial dialogue
	// 1 = the prop selection
	int state = 0;

	void Awake() {
		mNetworkManager = (NetworkManager) FindObjectOfType(typeof(NetworkManager));
		mDialogueManager = (DialogueManager) FindObjectOfType(typeof(DialogueManager));
		mCountdown = (Countdown) FindObjectOfType(typeof(Countdown));
	}

	void Start () {
		// TODO: Check what day it is - for now assume day 1 so give an introduction to prop selection

		// First we need to set everyone to "Not Ready"
		if (Network.isServer) {
			foreach (Player player in mNetworkManager.players) {
				player.networkView.RPC ("SetReady", RPCMode.All, false);
			}
		}

		print("Setting up");

		string[] propSelectionDialogue = new string[]{
			"text about prop selection..",
			"little bit of an overview..."
		};

		Action propSelectionDialogueComplete =
			() => {
				print ("COMPLETE");
				mNetworkManager.myPlayer.networkView.RPC("SetReady", RPCMode.All, true);
				mDialogueManager.StartDialogue("Waiting for other players to continue");
		};

		print("Showing Dialogue");
		mDialogueManager.StartDialogue(propSelectionDialogue, propSelectionDialogueComplete);
		print("Shown");
	}

	/**
	 * This is called on the server when any player changes their ready status
	 */
	public override void ReadyStatusChanged(Player pPlayer) {
		if (pPlayer.uReady) {
			// Check if all players are ready - if so we can start
			foreach (Player p in mNetworkManager.players) {
				if (!p.uReady) {
					return;
				}
			}
			
			// Everyone is ready, let's move on
			if (state == 0) {
				networkView.RPC ("StartPropSelection", RPCMode.All);
			} else {
				EndPropSelection();
			}
		}
	}
	
	/**
	 * Move to prop selection proper
	 */
	[RPC] void StartPropSelection() {
		mDialogueManager.EndDialogue();
		state = 1;

		Action countdownFinished =
			() => {
			if (Network.isServer) {
				// Only one person should push us to the next scene, so let it be the server
				EndPropSelection();
			}
		};

		mCountdown.StartCountdown (60, countdownFinished);
	}

	void EndPropSelection() {
		// TODO: Force everyone to have at least X props (ensure that they aren't stuck with nothing on the next scene)
		networkView.RPC ("MoveToNextScene", RPCMode.All);
	}

	/**
	 * Move to afternoon scene
	 */
	[RPC] void MoveToNextScene() {
		Application.LoadLevel ("Afternoon");
	}
}
