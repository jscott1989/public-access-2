using UnityEngine;
using System;
using System.Collections;

public class AfternoonManager : MonoBehaviour {
	
	public UILabel statusLabel;
	public Countdown countdown;

	void Start () {
		Action preparingCompleted =
			() => StartRecording();
		countdown.StartCountdown (5, preparingCompleted);
	}

	void StartRecording() {
		statusLabel.text = "Recording";
		Action recordingCompleted =
			() => Application.LoadLevel ("Evening");
		countdown.StartCountdown (30, recordingCompleted);
	}
}