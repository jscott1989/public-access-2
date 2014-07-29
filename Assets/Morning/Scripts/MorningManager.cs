using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class MorningManager : SceneManager {
	NetworkManager mNetworkManager;
	DialogueManager mDialogueManager;
	QuestionPanel mQuestionPanel;
	Manager mManager;
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

	public string uCurrentDay {
		get {
			if (mNetworkManager == null || mNetworkManager.myPlayer == null) {
				return "";
			}
			return "Day " + mNetworkManager.myPlayer.uDay.ToString () + " of " + Game.NUMBER_OF_DAYS.ToString ();
		}
	}

	public Texture uStationLogo {
		get {
			if (mNetworkManager == null || mNetworkManager.myPlayer == null) {
				return null;
			}
			return mNetworkManager.myPlayer.uStationLogo;
		}
	}

	public string uWelcomeMessage {
		get {
			if (mNetworkManager == null || mNetworkManager.myPlayer == null) {
				return "";
			}
			return "Welcome to " + mNetworkManager.myPlayer.uStationName;
		}
	}

	void Awake() {
		mNetworkManager = FindObjectOfType<NetworkManager>();
		mDialogueManager = FindObjectOfType<DialogueManager>();
		mQuestionPanel = FindObjectOfType<QuestionPanel>();
		mGame = FindObjectOfType<Game>();
		mManager = FindObjectOfType<Manager>();
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
			"Here comes your boss, " + mNetworkManager.myPlayer.uBossName
		};

		Action day1FirstDialogueFinished =
		() => {

			Action managerArrived =
				() => {
					Action day1DialogueFinished =
					() => {
						Action<string> showNameSelected =
						(string pShowName) => {
							mNetworkManager.myPlayer.networkView.RPC ("SetShowName", RPCMode.All, pShowName);
							
							string[] dialogue = new string[] {
								"Great. " + pShowName + "! I like it!",
								"You have " + Game.NUMBER_OF_DAYS.ToString () + " days to save your job and win the ratings war. Don't let me down!"
							};
							
							
							Action dialogueFinished =
							() => {
								mDialogueManager.StartDialogue("Waiting for other players to continue");
								mNetworkManager.myPlayer.networkView.RPC("SetReady", RPCMode.All, true);
							};
							
							mDialogueManager.StartDialogue(dialogue, dialogueFinished);
						};
						
						mQuestionPanel.AskQuestion ("The show will be \"" + mNetworkManager.myPlayer.uTheme + "\"\n\nWhat shall we call the new show?", showNameSelected);
						
					};
					
					System.Random rnd = new System.Random();
					
					string otherStation = mNetworkManager.players.Where (p => p.uID != mNetworkManager.mMyClientID).Select (p => p.uStationName).OrderBy(x => rnd.Next()).FirstOrDefault();
					if (otherStation == null) {
						// Should only happen when debugging with one player
						otherStation = "The other stations";
					}
					
					string[] day1SecondDialogue = new string[]{
						"Good morning, " + mNetworkManager.myPlayer.uName + "!",
						"Listen... I've been meaning to talk to you",
						"You see, things aren't going so great at " + mNetworkManager.myPlayer.uSelectedStation.uName,
						otherStation + " are killing us in the ratings",
						"The bosses... they wanted to let you go... " + mNetworkManager.myPlayer.uOldTheme + " just isn't doing it any more",
						"We're trying to recreate " + mNetworkManager.myPlayer.uSelectedStation.uName + " to be new, exciting, vibrant",
						"You've got one week to turn it around - and we're giving you a new show to run",
						"Our market research suggests that " + mNetworkManager.myPlayer.uTheme + " would really be a big hit with modern audiences",
					};
					
					mDialogueManager.StartDialogue(day1SecondDialogue, day1DialogueFinished);
			};
			mManager.Show (managerArrived);
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
		Action dialogueFinished =
			() => {
				mDialogueManager.StartDialogue("Waiting for other players to continue");
				mNetworkManager.myPlayer.networkView.RPC("SetReady", RPCMode.All, true);
		};

		if (Game.DEBUG_MODE) {
			mDialogueManager.StartDialogue ("Waiting for other players to continue");
			mNetworkManager.myPlayer.networkView.RPC ("SetReady", RPCMode.All, true);
		} else {
			Action s =
				() => {
					string[] dialogue = new string[] {
						"That was okay. Really, I think that's an improvement",
						"That's " + (mNetworkManager.myPlayer.uDay - 1).ToString () + " days gone. " + (Game.NUMBER_OF_DAYS - (mNetworkManager.myPlayer.uDay - 1)).ToString() + " to go."
					};

					mDialogueManager.StartDialogue (dialogue, dialogueFinished);
			};
			mManager.Show (s);
		}
	}

	void LastDay() {
		Action s =
		() => {
			Action firstDialogueFinished =
				() => {
					Action dialogueFinished =
						() => {
							mDialogueManager.StartDialogue("Waiting for other players to continue");
							mNetworkManager.myPlayer.networkView.RPC("SetReady", RPCMode.All, true);
					};

					string[] lastDialogue = new string[] {
						"This is the last day. One last change to prove to " + mNetworkManager.myPlayer.uStationName + " that you can make shows that the public want to see.",
						"Don't let us down."
					};

					mDialogueManager.StartDialogue(lastDialogue, dialogueFinished);
			};

			if (Game.DEBUG_MODE) {
				mDialogueManager.StartDialogue ("Waiting for other players to continue");
				mNetworkManager.myPlayer.networkView.RPC ("SetReady", RPCMode.All, true);
			} else {
				mDialogueManager.StartDialogue (mMiddleDialogues [0], firstDialogueFinished);
			}
		};
		mManager.Show (s);
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
			mNetworkManager.LoadLevel ("PropSelection");
		} else {
			mNetworkManager.LoadLevel("Feedback");
		}
	}
}
