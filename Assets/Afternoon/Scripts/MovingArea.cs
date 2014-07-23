using UnityEngine;
using System.Collections;

public class MovingArea : MonoBehaviour {
	MovableProp mMovableProp;
	RecordingProp mRecordingProp;

	void Awake() {
		mMovableProp = transform.parent.GetComponent<MovableProp>();
		mRecordingProp = transform.parent.GetComponent<RecordingProp>();
	}

	public void OnClick() {
		mRecordingProp.Select();
	}

	public void OnMouseDown() {
		mRecordingProp.Select();
		mMovableProp.StartMoving ();
	}
}
