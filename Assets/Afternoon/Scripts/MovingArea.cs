using UnityEngine;
using System.Collections;

public class MovingArea : MonoBehaviour {
	MovableProp mMovableProp;

	void Awake() {
		mMovableProp = transform.parent.GetComponent<MovableProp>();
	}

	public void OnMouseDown() {
		mMovableProp.StartMoving ();
	}
}
