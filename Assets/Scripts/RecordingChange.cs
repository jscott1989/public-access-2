using System;
using UnityEngine;

/**
 * This encapsulates a change to the state of the recording
 * (e.g. a movemement, resize, rotation, sound, etc.)
 * 
 * All parameters are passed on the constructor - and can only be passed as strings - they need to be parsed from there
 * into the correct value type
 */
public abstract class RecordingChange {
	public double uTime;

	public abstract void run(GameObject pScreen);
}

public class InstantiationChange : RecordingChange {
	public InstantiationChange(String pTime, string pID, string newX, string newY) {
		uTime = Convert.ToDouble (pTime);
	}

	public override void run(GameObject pScreen) {
		// TODO: Instantiate the object in pScreen
	}
}

public class DestroyChange : RecordingChange {
	public DestroyChange(string pTime, string pID) {
		uTime = Convert.ToDouble (pTime);
	}
	
	public override void run(GameObject pScreen) {
		// TODO: Destroy the object in pScreen
	}
}

public class PositionChange : RecordingChange {
	public PositionChange(string pTime, string pID, string newX, string newY) {
		uTime = Convert.ToDouble (pTime);
	}
	
	public override void run(GameObject pScreen) {
		// TODO: Move pID to pNewPosition
	}
}
