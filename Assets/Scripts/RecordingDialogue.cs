using UnityEngine;
using System;
using System.Collections;

public class RecordingDialogue : RecordingProp {

	public string uID = Guid.NewGuid().ToString();

	/**
	 * Put the prop back in the prop box
	 */
	public override void PutBackInBox() {
		Destroy (gameObject);
	}
}
