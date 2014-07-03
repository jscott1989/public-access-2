using UnityEngine;
using System.Collections;

/**
 * This class controls the "Story" text which is shown in bubbles on the bottom of the screen
 */
public class DialogueController : MonoBehaviour {
	public string[] text;

	void Start () {
		// For now we initialise ourselves with some text
	}

	/**
	 * Make the dialog show, showing the list of strings, one per page
	 */
	public void setText(string[] t) {
		text = t;
	}

	/**
	 * Move to the next stage of the dialogue, or call the callback if all dialogue is exhausted
	 */
	public void next() {

	}
}
