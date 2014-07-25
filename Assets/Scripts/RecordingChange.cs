using System;
using UnityEngine;
using System.Linq;

/**
 * This encapsulates a change to the state of the recording
 * (e.g. a movemement, resize, rotation, sound, etc.)
 * 
 * All parameters are passed on the constructor - and can only be passed as strings - they need to be parsed from there
 * into the correct value type
 */
public abstract class RecordingChange {
	public double uTime;
	public string uID;

	public abstract void run(GameObject pScreen);
}

public class InstantiationChange : RecordingChange {

	string mPropID;
	string mNewX;
	string mNewY;
	string mSizeX;
	string mSizeY;
	string mZOrder;

	GameObject mPlayingPropPrefab;

	public InstantiationChange(String pTime, string pPropID, string pID, string pNewX, string pNewY, string pSizeX, string pSizeY, string pZOrder) {
		uTime = Convert.ToDouble (pTime);
		mPropID = pPropID;
		uID = pID;
		mNewX = pNewX;
		mNewY = pNewY;
		mSizeX = pSizeX;
		mSizeY = pSizeY;
		mZOrder = pZOrder;
		mPlayingPropPrefab = (GameObject)Resources.Load ("Evening/Prefabs/PlayingProp");
	}

	public override void run(GameObject pScreen) {

		// First we destroy the object if it already exists
		foreach (PlayingProp p in pScreen.GetComponentsInChildren<PlayingProp>().Where(p => p.uID == uID)) {
			GameObject.Destroy(p.gameObject);
		}

		GameObject g = (GameObject) GameObject.Instantiate(mPlayingPropPrefab, Vector3.zero, Quaternion.identity);
		g.transform.parent = pScreen.transform;

		dfTextureSprite sprite = (dfTextureSprite)g.GetComponent (typeof(dfTextureSprite));
		sprite.Texture = (Texture2D)Resources.Load("Props/" + mPropID);
		sprite.Position = new Vector2(float.Parse(mNewX), float.Parse (mNewY));
		sprite.Size = new Vector2(float.Parse (mSizeX), float.Parse (mSizeY));
		sprite.ZOrder = int.Parse (mZOrder);
		PlayingProp r = (PlayingProp)g.GetComponent (typeof(PlayingProp));
		r.uID = uID;
		Game mGame = GameObject.FindObjectOfType<Game>();
		if (mGame.uProps.ContainsKey(mPropID)) {
			r.uProp = mGame.uProps[mPropID];
		} else if (mGame.uBackdrops.ContainsKey(mPropID)) {
			r.uProp = mGame.uBackdrops[mPropID];
		}
		sprite.enabled = true;
	}
}

public class DialogueInstantiationChange : RecordingChange {
	
	string mText;
	string mSpriteName;
	string mTextScale;
	string mNewX;
	string mNewY;
	string mSizeX;
	string mSizeY;
	string mZOrder;
	
	GameObject mPlayingDialoguePrefab;
	
	public DialogueInstantiationChange(String pTime, string pID, String pSpriteName, String pText, string pTextScale, string pNewX, string pNewY, string pSizeX, string pSizeY, string pZOrder) {
		uTime = Convert.ToDouble (pTime);
		uID = pID;
		mText = pText;
		mSpriteName = pSpriteName;
		mTextScale = pTextScale;
		mNewX = pNewX;
		mNewY = pNewY;
		mSizeX = pSizeX;
		mSizeY = pSizeY;
		mZOrder = pZOrder;
		mPlayingDialoguePrefab = (GameObject)Resources.Load ("Evening/Prefabs/PlayingDialogue");
	}
	
	public override void run(GameObject pScreen) {
		
		// First we destroy the object if it already exists
		foreach (PlayingProp p in pScreen.GetComponentsInChildren<PlayingProp>().Where(p => p.uID == uID)) {
			GameObject.Destroy(p.gameObject);
		}
		
		GameObject g = (GameObject) GameObject.Instantiate(mPlayingDialoguePrefab, Vector3.zero, Quaternion.identity);
		g.transform.parent = pScreen.transform;
		
		dfSlicedSprite sprite = g.GetComponent<dfSlicedSprite>();
		sprite.SpriteName = mSpriteName;
		sprite.Position = new Vector2(float.Parse(mNewX), float.Parse (mNewY));
		sprite.Size = new Vector2(float.Parse (mSizeX), float.Parse (mSizeY));
		sprite.ZOrder = int.Parse (mZOrder);

		PlayingProp r = g.GetComponent<PlayingProp>();
		r.uID = uID;

		dfLabel l = g.GetComponentInChildren<dfLabel>();
		l.Text = mText;
		l.TextScale = float.Parse (mTextScale);

		sprite.enabled = true;
	}
}

public class DestroyChange : RecordingChange {

	public DestroyChange(string pTime, string pID) {
		uTime = Convert.ToDouble (pTime);
		uID = pID;
	}
	
	public override void run(GameObject pScreen) {
		foreach (PlayingProp p in pScreen.GetComponentsInChildren<PlayingProp>().Where(p => p.uID == uID)) {
			GameObject.Destroy(p.gameObject);
		}
	}
}

public class PositionChange : RecordingChange {
	string mNewX;
	string mNewY;

	public PositionChange(string pTime, string pID, string pNewX, string pNewY) {
		uTime = Convert.ToDouble (pTime);
		uID = pID;
		mNewX = pNewX;
		mNewY = pNewY;
	}
	
	public override void run(GameObject pScreen) {
		foreach (PlayingProp p in pScreen.GetComponentsInChildren<PlayingProp>().Where(p => p.uID == uID)) {
			dfControl sprite = p.gameObject.GetComponent<dfTextureSprite>();
			if (sprite == null) {
				sprite = p.gameObject.GetComponent<dfSlicedSprite>();
			}
			sprite.Position = new Vector2(float.Parse(mNewX), float.Parse (mNewY));
		}
	}
}

public class ZOrderChange : RecordingChange { 
	string mZOrder;

	public ZOrderChange(string pTime, string pID, string pZOrder) {
		uTime = Convert.ToDouble (pTime);
		uID = pID;
		mZOrder = pZOrder;
	}

	public override void run(GameObject pScreen) {
		foreach (PlayingProp p in pScreen.GetComponentsInChildren<PlayingProp>().Where(p => p.uID == uID)) {
			dfControl sprite = p.gameObject.GetComponent<dfTextureSprite>();
			if (sprite == null) {
				sprite = p.gameObject.GetComponent<dfSlicedSprite>();
			}
			sprite.ZOrder = Convert.ToInt32(mZOrder);
		}
	}
}

public class SizeChange : RecordingChange { 
	float mX;
	float mY;
	
	public SizeChange(string pTime, string pID, string pX, string pY) {
		uTime = Convert.ToDouble (pTime);
		uID = pID;
		mX = float.Parse(pX);
		mY = float.Parse (pY);
	}
	
	public override void run(GameObject pScreen) {
		foreach (PlayingProp p in pScreen.GetComponentsInChildren<PlayingProp>().Where(p => p.uID == uID)) {
			dfControl sprite = p.gameObject.GetComponent<dfTextureSprite>();
			if (sprite == null) {
				sprite = p.gameObject.GetComponent<dfSlicedSprite>();
			}
			sprite.Size = new Vector2(mX, mY);
		}
	}
}

public class DialogueTextChange : RecordingChange { 
	string mText;
	
	public DialogueTextChange(string pTime, string pID, string pText) {
		uTime = Convert.ToDouble (pTime);
		uID = pID;
		mText = pText;
	}
	
	public override void run(GameObject pScreen) {
		foreach (PlayingProp p in pScreen.GetComponentsInChildren<PlayingProp>().Where(p => p.uID == uID)) {
			dfLabel l = p.GetComponentInChildren<dfLabel>();
			l.Text = mText;
		}
	}
}

public class DialogueTextScaleChange : RecordingChange { 
	string mTextScale;
	
	public DialogueTextScaleChange(string pTime, string pID, string pTextScale) {
		uTime = Convert.ToDouble (pTime);
		uID = pID;
		mTextScale = pTextScale;
	}
	
	public override void run(GameObject pScreen) {
		foreach (PlayingProp p in pScreen.GetComponentsInChildren<PlayingProp>().Where(p => p.uID == uID)) {
			dfLabel l = p.GetComponentInChildren<dfLabel>();
			l.TextScale = float.Parse (mTextScale);
		}
	}
}

public class AudioChange : RecordingChange {

	public AudioChange(string pTime, string pID) {
		uTime = Convert.ToDouble (pTime);
		uID = pID;
	}

	public override void run(GameObject pScreen) {
		// TODO: Play the audio - also change the score as needed (because the scorer won't be able to pick this up)
		Debug.Log("playing audio " + uID + " at " + uTime.ToString ());
		RecordingPlayer recordingPlayer = GameObject.FindObjectOfType<RecordingPlayer>();
		recordingPlayer.PlayAudio(uID);
	}
}