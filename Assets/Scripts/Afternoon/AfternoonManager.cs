using UnityEngine;
using System;
using System.Collections;

public class AfternoonManager : MonoBehaviour {
	
	public UILabel statusLabel;
	public Countdown countdown;
	SceneRecorder sceneRecorder;

	NetworkManager networkManager;

	void Start () {
		// First set up the references
		networkManager = (NetworkManager) GameObject.FindGameObjectWithTag ("NetworkManager").GetComponent ("NetworkManager");
		sceneRecorder = (SceneRecorder)GetComponent (typeof(SceneRecorder));

		// Next - create the grid of available props
		Player myPlayer = networkManager.GetMyPlayer ();

		foreach (string prop_id in myPlayer.props) {
			print (prop_id);
		}



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