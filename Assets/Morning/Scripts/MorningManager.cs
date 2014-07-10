using UnityEngine;
using System;
using System.Collections;

public class MorningManager : SceneManager {
	NetworkManager mNetworkManager;
	DialogueManager mDialogueManager;

	void Awake() {
		mNetworkManager = (NetworkManager)FindObjectOfType (typeof(NetworkManager));
		mDialogueManager = (DialogueManager)FindObjectOfType (typeof(DialogueManager));
	}

	void Start () {
		// First we need to set everyone to "Not Ready"
		foreach (Player player in mNetworkManager.players) {
			player.networkView.RPC ("SetReady", RPCMode.All, false);
		}

		// TODO: Start the dialogue
		// TODO: We need a way of tracking the day, but for now let's just assume it's day 1
		string[] day1Dialogue = new string[]{
			"A",
			"B",
			"C"
		};

		Action day1DialogueFinished =
			() => {
				mNetworkManager.myPlayer.networkView.RPC("SetReady", RPCMode.All, true);
				mDialogueManager.StartDialogue(new string[]{"Waiting for other players to continue"});
		};

		mDialogueManager.StartDialogue(day1Dialogue, day1DialogueFinished);
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

			// Everyone is ready, let's move to the next scene
			networkView.RPC ("MoveToNextScene", RPCMode.All);
		}
	}

	/**
	 * Move to the prop selection scene
	 */
	[RPC] void MoveToNextScene() {
		mDialogueManager.EndDialogue();
		Application.LoadLevel ("PropSelection");
	}
}
