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

	string mPropID;
	string mID;
	string mNewX;
	string mNewY;

	GameObject mPlayingPropPrefab;

	public InstantiationChange(String pTime, string pPropID, string pID, string pNewX, string pNewY) {
		uTime = Convert.ToDouble (pTime);
		mPropID = pPropID;
		mID = pID;
		mNewX = pNewX;
		mNewY = pNewY;
		mPlayingPropPrefab = (GameObject)Resources.Load ("Evening/Prefabs/PlayingProp");
	}

	public override void run(GameObject pScreen) {
		// TODO: Instantiate the object in pScreen
		Debug.Log("Create " + mPropID + "(" + mID + ") at " + mNewX + "," + mNewY);
		GameObject g = (GameObject) GameObject.Instantiate(mPlayingPropPrefab, Vector3.zero, Quaternion.identity);
		g.transform.parent = pScreen.transform;

		dfTextureSprite sprite = (dfTextureSprite)g.GetComponent (typeof(dfTextureSprite));
		sprite.Texture = (Texture2D)Resources.Load("Props/" + mPropID);
		sprite.Position = new Vector2(float.Parse(mNewX), float.Parse (mNewY));
		PlayingProp r = (PlayingProp)g.GetComponent (typeof(PlayingProp));
		r.uID = mID;
	}
}

public class DestroyChange : RecordingChange {

	string mID;

	public DestroyChange(string pTime, string pID) {
		uTime = Convert.ToDouble (pTime);
		mID = pID;
	}
	
	public override void run(GameObject pScreen) {
		foreach (PlayingProp p in pScreen.GetComponentsInChildren (typeof(PlayingProp))) {
			if (p.uID == mID) {
				GameObject.Destroy(p.gameObject);
				return;
			}
		}
	}
}

public class PositionChange : RecordingChange {

	string mID;
	string mNewX;
	string mNewY;

	public PositionChange(string pTime, string pID, string pNewX, string pNewY) {
		uTime = Convert.ToDouble (pTime);
		mID = pID;
		mNewX = pNewX;
		mNewY = pNewY;
	}
	
	public override void run(GameObject pScreen) {
		foreach (PlayingProp p in pScreen.GetComponentsInChildren (typeof(PlayingProp))) {
			if (p.uID == mID) {
				dfTextureSprite sprite = (dfTextureSprite) p.gameObject.GetComponent (typeof(dfTextureSprite));
				sprite.Position = new Vector2(float.Parse(mNewX), float.Parse (mNewY));
				return;
			}
		}
	}
}

public class ZOrderChange : RecordingChange { 
	string mID;
	string mZOrder;

	public ZOrderChange(string pTime, string pID, string pZOrder) {
		uTime = Convert.ToDouble (pTime);
		mZOrder = pZOrder;
	}

	public override void run(GameObject pScreen) {
		foreach (PlayingProp p in pScreen.GetComponentsInChildren (typeof(PlayingProp))) {
			if (p.uID == mID) {
				dfTextureSprite sprite = (dfTextureSprite) p.gameObject.GetComponent (typeof(dfTextureSprite));
				sprite.ZOrder = Convert.ToInt32(mZOrder);
				return;
			}
		}
	}
}