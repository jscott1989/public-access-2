using UnityEngine;
using System;
using System.Collections;

/**
 * This class controls the "Story" text which is shown in bubbles on the bottom of the screen
 */
public class DialogueController : MonoBehaviour {
	public string[] text;
	int currentIndex = 0;

	Action callbackFunction;
	public GameObject continueButton;

	/**
	 * Make the dialog show, showing the list of strings, one per page
	 */
	public void SetText(string[] t, Action callback) {
		UILabel u = (UILabel) GetComponent (typeof(UILabel));
		callbackFunction = callback;
		text = t;
		currentIndex = 0;
		u.text = text [currentIndex];
		u.enabled = true;
		EnableContinue();
	}

	/**
	 * Move to the next stage of the dialogue, or call the callback if all dialogue is exhausted
	 */
	public void Next() {
		UILabel u = (UILabel) GetComponent (typeof(UILabel));
		currentIndex += 1;
		if (currentIndex >= text.Length) {
			// Finished - call the callback
			u.enabled = false;
			DisableContinue();
			callbackFunction ();
		} else {
			u.text = text [currentIndex];
		}
	}

	void DisableContinue() {
		foreach (UILabel l in continueButton.GetComponentsInChildren(typeof(UILabel))) {
			l.enabled = false;
		}
		foreach (UISlicedSprite l in continueButton.GetComponentsInChildren(typeof(UISlicedSprite))) {
			l.enabled = false;
		}

		BoxCollider collider = (BoxCollider) continueButton.GetComponent(typeof(BoxCollider));
		collider.enabled = false;
	}

	void EnableContinue() {
		foreach (UILabel l in continueButton.GetComponentsInChildren(typeof(UILabel))) {
			l.enabled = true;
		}
		foreach (UISlicedSprite l in continueButton.GetComponentsInChildren(typeof(UISlicedSprite))) {
			l.enabled = true;
		}
		
		BoxCollider collider = (BoxCollider) continueButton.GetComponent(typeof(BoxCollider));
		collider.enabled = true;
	}
}
