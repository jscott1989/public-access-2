using UnityEngine;
using System;
using System.Collections;

/**
 * Attach this script to a guiText object and it will manage a countdown from the provided number
 * TODO: Set a callback for when the countdown completes
 */
public class Countdown : MonoBehaviour {

	/**
	 * The number that should be counted down from
	 */
	float countdownTimer = 30;
	bool finished = true;
	UILabel u;
	Action callback;

	void Start() {
		u = (UILabel) GetComponent (typeof(UILabel));
	}

	public void StartCountdown(float seconds, Action c) {
		countdownTimer = seconds;
		callback = c;
		finished = false;
	}

	void Update () {
		// Make the change and update the text
		if (!finished) {
			countdownTimer -= Time.deltaTime;
			if (countdownTimer <= 0) {
				finished = true;
				callback();
			}

			u.text = countdownTimer.ToString ("0");
		}
	}
}
