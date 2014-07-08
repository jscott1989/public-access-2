using UnityEngine;
using System;
using System.Collections;

public class AfternoonManager : MonoBehaviour {
	
	public UILabel statusLabel;
	public Countdown countdown;
	SceneRecorder sceneRecorder;

	NetworkManager networkManager;

	void Start () {
		networkManager = (NetworkManager) GameObject.FindGameObjectWithTag ("NetworkManager").GetComponent ("NetworkManager");
		sceneRecorder = (SceneRecorder)GetComponent (typeof(SceneRecorder));

		Action preparingCompleted =
			() => StartRecording();
		countdown.StartCountdown (30, preparingCompleted);
	}

	void StartRecording() {
		statusLabel.text = "Recording";
		sceneRecorder.StartRecording (networkManager.GetMyPlayer());
		Action recordingCompleted =
			() => StopRecording ();
		countdown.StartCountdown (30, recordingCompleted);
	}

	void StopRecording() {
		sceneRecorder.Stop();
		Application.LoadLevel ("Evening");
	}
}