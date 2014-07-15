using UnityEngine;
using System.Collections;

public class ViewerChart : MonoBehaviour {

	RecordingPlayer mRecordingPlayer = null;;

	public void StartViewer(RecordingPlayer pRecordingPlayer) {
		mRecordingPlayer = pRecordingPlayer;
	}

	void Update () {
		if (mRecordingPlayer != null) {
			// TODO: Position the "thumb" at the right location for the recording player
		}
	}
}
