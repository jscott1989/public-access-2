using UnityEngine;
using System;
using System.Collections;

public class MorningManager : SceneManager {
	NetworkManager mNetworkManager;
	DialogueManager mDialogueManager;
	QuestionPanel mQuestionPanel;
	Game mGame;

	string[] mGoodDialogues = new string[]{
		"Wow! That was one of our best ever nights. Keep it up!",
		// TODO: More good dialogues
	};

	string[] mBadDialogues = new string[]{
		"That was bad",
		// TODO: Bad dialogues
	};

	string[] mMiddleDialogues = new string[]{
		"That was okay",
		// TODO: Middle dialogues
	};

	void Awake() {
		mNetworkManager = FindObjectOfType<NetworkManager>();
		mDialogueManager = FindObjectOfType<DialogueManager>();
		mQuestionPanel = FindObjectOfType<QuestionPanel>();
		mGame = FindObjectOfType<Game>();
	}

	void Start () {
		// First we need to set everyone to "Not Ready"
		if (Network.isServer) {
			foreach (Player player in mNetworkManager.players) {
				player.networkView.RPC ("SetReady", RPCMode.All, false);
			}
		}

		if (mNetworkManager.myPlayer.uDay == 1) {
			FirstDay();
		} else if(mNetworkManager.myPlayer.uDay < Game.NUMBER_OF_DAYS) {
			MiddleDay();
		} else {
			LastDay();
		}
	}

	void FirstDay() {
		string[] day1FirstDialogue = new string[]{
			"This is " + mNetworkManager.myPlayer.uSelectedStation.uName + ". The public access TV station you've worked at for several years",
			"Here comes your boss, (boss' name)"
		};

		Action day1FirstDialogueFinished =
		() => {

			Action day1DialogueFinished =
			() => {
				Action<string> showNameSelected =
					(string pShowName) => {
						mNetworkManager.myPlayer.networkView.RPC ("SetShowName", RPCMode.All, pShowName);
						mDialogueManager.StartDialogue("Waiting for other players to continue");
						mNetworkManager.myPlayer.networkView.RPC("SetReady", RPCMode.All, true);
				};

				mQuestionPanel.AskQuestion ("The show will be \"" + mNetworkManager.myPlayer.uTheme + "\"\n\nWhat shall we call the new show?", showNameSelected);

			};

			string[] day1SecondDialogue = new string[]{
				"Good morning " + mNetworkManager.myPlayer.uName + "!",
				"Listen... I've been meaning to talk to you",
				"You see, things aren't going so great at " + mNetworkManager.myPlayer.uSelectedStation.uName,
				"(other station name) are killing us in the ratings",
				"The bosses... they wanted to let you go... (old tv theme) just isn't doing it any more",
				"We're trying to recreate " + mNetworkManager.myPlayer.uSelectedStation.uName + " to be new, exciting, vibrant",
				"You've got one week to turn it around - and we're giving you a new show to run",
				"Our market research suggests that " + mNetworkManager.myPlayer.uTheme + " would really be a big hit with modern audiences",
			};

			
			mDialogueManager.StartDialogue(day1SecondDialogue, day1DialogueFinished);
		};

		if (Game.DEBUG_MODE) {
			mNetworkManager.myPlayer.networkView.RPC ("SetShowName", RPCMode.All, "TempShowName");
			mDialogueManager.StartDialogue ("Waiting for other players to continue");
			mNetworkManager.myPlayer.networkView.RPC ("SetReady", RPCMode.All, true);
		} else {
			mDialogueManager.StartDialogue (day1FirstDialogue, day1FirstDialogueFinished);
		}
	}

	void MiddleDay() {
		// TODO: figure out if we're in the top/middle/bottom 3rd of players
		// for now we just show middle
		Action dialogueFinished =
			() => {
				mDialogueManager.StartDialogue("Waiting for other players to continue");
				mNetworkManager.myPlayer.networkView.RPC("SetReady", RPCMode.All, true);
		};

		if (Game.DEBUG_MODE) {
			mDialogueManager.StartDialogue ("Waiting for other players to continue");
			mNetworkManager.myPlayer.networkView.RPC ("SetReady", RPCMode.All, true);
		} else {
			mDialogueManager.StartDialogue (mMiddleDialogues [0], dialogueFinished);
		}
	}

	void LastDay() {
		// TODO: figure out if we're in the top/middle/bottom 3rd of players
		// for now we just show middle
		Action firstDialogueFinished =
			() => {
				Action dialogueFinished =
					() => {
						mDialogueManager.StartDialogue("Waiting for other players to continue");
						mNetworkManager.myPlayer.networkView.RPC("SetReady", RPCMode.All, true);
				};

				string[] lastDialogue = new string[] {
					"This is the last day......", // TODO
				};

				mDialogueManager.StartDialogue(lastDialogue, dialogueFinished);
		};

		if (Game.DEBUG_MODE) {
			mDialogueManager.StartDialogue ("Waiting for other players to continue");
			mNetworkManager.myPlayer.networkView.RPC ("SetReady", RPCMode.All, true);
		} else {
			mDialogueManager.StartDialogue (mMiddleDialogues [0], firstDialogueFinished);
		}
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

	/**
	 * Move to the prop selection scene
	 */
	[RPC] void MoveToNextScene() {
		mDialogueManager.EndDialogue();
		if (mNetworkManager.myPlayer.uDay == 1) {
			Application.LoadLevel ("PropSelection");
		} else {
			Application.LoadLevel("Feedback");
		}
	}
}
