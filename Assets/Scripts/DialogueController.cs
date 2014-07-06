using UnityEngine;
using System.Collections;

/**
 * This class controls the "Story" text which is shown in bubbles on the bottom of the screen
 */
public class DialogueController : MonoBehaviour {
	public string[] text;
	int currentIndex = 0;
	UILabel u;

	void Start () {
		// For now we initialise ourselves with some text
		// TODO: Set up a controller which does this for us
		u = (UILabel) GetComponent (typeof(UILabel));
		setText (new string[]{"A", "B", "C"});
	}

	/**
	 * Make the dialog show, showing the list of strings, one per page
	 */
	public void setText(string[] t) {
		text = t;
		currentIndex = 0;
		u.text = text [currentIndex];
	}

	/**
	 * Move to the next stage of the dialogue, or call the callback if all dialogue is exhausted
	 */
	public void next() {
		print ("NEXT");
		currentIndex += 1;
		u.text = text [currentIndex];
	}
}
