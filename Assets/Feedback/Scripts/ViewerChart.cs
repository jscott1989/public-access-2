using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ViewerChart : MonoBehaviour {

	RecordingPlayer mRecordingPlayer = null;
	public dfTextureSprite uThumb;
	public dfTextureSprite uBackground;
	public GameObject uBars;

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

	Texture2D GetTexture(int pNumberOfPlayers, int pAmount) {
		float n = pNumberOfPlayers;
		float a = pAmount;

		string r = "";
		if (pAmount == 0) {
			// Return 0
			r = "0";
		} else if (pAmount == 10) {
			// Return 1
			r = "1";
		} else if (a / n == 2) {
			// Return 10_5
			r = "10_5";
		} else if (a / n == (1 / 3)) {
			r = "12_4"; // TODO: Create this image
		} else if (a / n == (2 / 3)) {
			r = "12_8"; // TODO: Create this image
		} else if (a / n == (1 / 4)) {
			r = "12_3"; // TODO: Create this image
		} else if (a / n == (3 / 4)) {
			r = "12_9"; // TODO: Create this image
		} else if (a / n == (1 / 6)) {
			r = "12_2"; // TODO: Create this image
		} else if (a / n == (5 / 6)) {
			r = "12_10"; // TODO: Create this image
		} else {
			// TODO: Add 1/5, 2/5, 3/5, 4/5, 1/7, 2/7, 3/7, 4/7, 5/7, 6/7, 1/8, 3/8, 5/8, 7/8, 1/8, 1/9, 2/9, 4/9, 5/9, 7/9, 8/9
			r = pNumberOfPlayers.ToString () + "_" + pAmount.ToString ();
		}
		return (Texture2D) Resources.Load ("Feedback/Images/Chart/" + r);
	}

	public void UpdateChart(int pNumberOfPlayers, int[] data) {
//		List<dfTextureSprite> l = new List<dfTextureSprite> ();
		dfTextureSprite[] l = uBars.GetComponentsInChildren<dfTextureSprite>();
		l = l.OrderBy(o=>o.Position.x).ToArray();

		for (int i = 0; i < Game.RECORDING_COUNTDOWN; i++) {
			l[i].Texture = GetTexture(pNumberOfPlayers, data[i]);
		}
	}
}
