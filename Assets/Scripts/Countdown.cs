using UnityEngine;
using System;
using System.Collections;

/**
 * Attach to a label, and bind the uSecondsRemaining property to the label's text
 * this will then manage a countdown
 */
public class Countdown : MonoBehaviour {
	float countdownTimer = 30;
	bool finished = true;
	Action callback;

	public string uSecondsRemaining {
		get {
			return countdownTimer.ToString ("0");
		}
	}

	public void StartCountdown(float pSeconds, Action pCallback) {
		countdownTimer = pSeconds;
		callback = pCallback;
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
		}
	}
}
