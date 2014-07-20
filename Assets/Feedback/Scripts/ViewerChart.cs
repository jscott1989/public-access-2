﻿using UnityEngine;
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
		// TODO: This divison might have problems with float precision
		if (pAmount == 0) {
			// Return 0
			r = "0";
		} else if (a == n) {
			// Return 1
			r = "1";
		} else if (a / n == 2) {
			// Return 10_5
			r = "10_5";
		} else if (a / n == (1 / 3)) {
			r = "12_4";
		} else if (a / n == (2 / 3)) {
			r = "12_8";
		} else if (a / n == (1 / 4)) {
			r = "12_3";
		} else if (a / n == (3 / 4)) {
			r = "12_9";
		} else if (a / n == (1 / 6)) {
			r = "12_2";
		} else if (a / n == (5 / 6)) {
			r = "12_10";
		} else if (a / n == (1 / 5)) {
			r = "10_2";
		} else if (a / n == (2 / 5)) {
			r = "10_4";
		} else if (a / n == (3 / 5)) {
			r = "10_6";
		} else if (a / n == (4 / 5)) {
			r = "10_8";
		} else {
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
			dfLabel label = l[i].gameObject.GetComponentInChildren<dfLabel>();
			label.Text = data[i].ToString();
		}
	}
}
