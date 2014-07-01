using UnityEngine;
using System.Collections;

/**
 * Attach this script to a guiText object and it will manage a countdown from the provided number
 * TODO: Set a callback for when the countdown completes
 */
public class Countdown : MonoBehaviour {

	/**
	 * The number that should be counted down from
	 */
	public float countdownTimer = 30;

	void Update () {
		// Make the change and update the text
		countdownTimer -= Time.deltaTime;
		guiText.text = countdownTimer.ToString ("0");
	}
}
