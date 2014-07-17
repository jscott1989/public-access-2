using UnityEngine;
using System.Collections;

public class ViewerChart : MonoBehaviour {

	RecordingPlayer mRecordingPlayer = null;
	public dfTextureSprite uThumb;
	public dfTextureSprite uBackground;

	bool mIsDragging = false;

	public void StartViewer(RecordingPlayer pRecordingPlayer) {
		mRecordingPlayer = pRecordingPlayer;
	}

	void Update () {
		if (mIsDragging) {
			Vector2 aPosition = uThumb.GetManager ().ScreenToGui (new Vector2 (Input.mousePosition.x, Input.mousePosition.y));
			aPosition = new Vector2 (aPosition.x, 0 - aPosition.y);
			
			// TODO: these numbers are hardcoded - copied from the unity gui - figure out how to get this programatically
			Vector2 relativePosition = new Vector2 (aPosition.x - 179, aPosition.y + 521);

			if (relativePosition.x < 0) {
				relativePosition = new Vector2(0, relativePosition.y);
			}
			if (relativePosition.x > 420) {
				relativePosition = new Vector2(420, relativePosition.y);
			}

			
			mRecordingPlayer.Jump(relativePosition.x / (420 / 30));
			
			if (!Input.GetMouseButton(0)) {
				mIsDragging = false;
				mRecordingPlayer.Continue();
			}
		}

		if (mRecordingPlayer != null) {
			uThumb.Position = new Vector2((float)mRecordingPlayer.uTime * (420/30), 0);
		}
	}

	public void OnMouseDown() {
		mRecordingPlayer.Pause ();
		mIsDragging = true;
	}
}
