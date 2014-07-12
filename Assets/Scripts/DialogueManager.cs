using UnityEngine;
using System;
using System.Collections;

/**
 * This manages showing dialogues at the bottom of the screen, and can callback when the dialogue is finished
 */
public class DialogueManager : MonoBehaviour {
	string[] mDialogue = new string[]{};
	Action mCallback;

	public string uContinueButtonText;

	int currentDialogue = 0;

	/**
	 * Should the dialogue be visible?
	 */
	public bool uDialogueVisible {
		get {
			return (currentDialogue < mDialogue.Length);
		}
	}

	/**
	 * Should the continue button be visible?
	 */
	public bool uContinueVisible {
		get {
			if (currentDialogue < mDialogue.Length - 1) {
				return true;
			} else if (mCallback != null) {
				return true;
			}
			return false;
		}
	}
	
	/**
	 * What is the current text of the dialogue?
	 */
	public string uCurrentText {
		get {
			if (currentDialogue < mDialogue.Length) {
				return mDialogue[currentDialogue];
			}
			return "";
		}
	}

	/**
	 * Start a new dialogue
	 * provide a list of strings for the dialogue to show
	 * and a callback for once the dialogue is completed - if there is no callback the final "Continue" button will be hidden
	 * and the dialogue can only be ended with EndDialogue
	 */
	public void StartDialogue(string[] pDialogue, Action pCallback = null, string pContinueButtonText = "Continue") {
		mDialogue = pDialogue;
		mCallback = pCallback;
		uContinueButtonText = pContinueButtonText;

		currentDialogue = 0;
	}
	public void StartDialogue(string pDialogue, Action pCallback = null, string pContinueButtonText = "Continue") {
		StartDialogue (new string[]{pDialogue}, pCallback, pContinueButtonText);
	}

	/**
	 * End the current dialogue
	 */
	public void EndDialogue() {
		print("Ending dialogue");
		currentDialogue = 0;
		mDialogue = new string[]{};
		mCallback = null;
	}

	/**
	 * Move to the next part of the dialogue
	 */
	public void next() {
		currentDialogue += 1;
		if (!uDialogueVisible) {
			// Dialogue is finished
			mCallback();
		}
	}
}
