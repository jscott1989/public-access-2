using UnityEngine;
using System;
using System.Collections;

public class Manager : MonoBehaviour {
	Action mCallback;
	public void Show(Action callback) {
		mCallback = callback;
		dfTweenVector3 t = GetComponent<dfTweenVector3>();
		t.Play ();
	}

	public void TweenFinished() {
		print ("MANAGER ARRIVED");
		mCallback();
	}
}
