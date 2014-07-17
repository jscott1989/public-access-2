using UnityEngine;
using System.Collections;

public class ViewerChart : MonoBehaviour {

	RecordingPlayer mRecordingPlayer = null;
	public dfTextureSprite uThumb;
	public dfTextureSprite uBackground;

	public void StartViewer(RecordingPlayer pRecordingPlayer) {
		mRecordingPlayer = pRecordingPlayer;
	}

	void Update () {
		if (mRecordingPlayer != null) {
			// TODO: Position the "thumb" at the right location for the recording player
			uThumb.Position = new Vector2((float)mRecordingPlayer.uTime * (420/30), 0);
		}
	}
}
