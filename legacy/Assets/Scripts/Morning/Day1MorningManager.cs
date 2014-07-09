using UnityEngine;
using System;
using System.Collections;

public class Day1MorningManager : MonoBehaviour {

	public DialogueController dialogueController;

	// Use this for initialization
	void Start () {
		Action firstDialogueCompleted =
			() => FirstDialogueCompleted();
		dialogueController.SetText (new string[]{"Hello. This is message 1", "Hello. This is message 2"}, firstDialogueCompleted);
	}
	
	void FirstDialogueCompleted() {
		// TODO: Now we can introduce the station manager, then set some more dialogue
		// including generating the theme
		Application.LoadLevel ("Prop Selection"); // This is temporary - until the dialogue is implemented
	}

	void SecondDialogueCompleted() {
		// TODO: Now we'll need to input the name of the show
		// then some more dialogue
	}

	void ThirdDialogueCompleted() {
		// Now we move on to the afternoon prop selection phase
		Application.LoadLevel ("Prop Selection");
	}
}
