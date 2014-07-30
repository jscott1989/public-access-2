using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Recorder : MonoBehaviour {
	Player mRecordingPlayer;

	string[] mKnownPropIDs;
	Dictionary<string, Vector3> mKnownPositions = new Dictionary<string, Vector3>();
	Dictionary<string, Vector2> mKnownSizes = new Dictionary<string, Vector2>();
	Dictionary<string, int> mKnownZOrders = new Dictionary<string, int>();

	string[] mKnownDialogueIDs;
	Dictionary<string, string> mKnownDialogueTexts = new Dictionary<string, string>();
	Dictionary<string, float> mKnownDialogueTextScales = new Dictionary<string, float>();

	double mTime = 0;

	/**
	 * Start recording to pPlayer's latest episode
	 */
	public void StartRecording(Player pPlayer, GameObject pScreen) {
		mTime = 0;
		mKnownPropIDs = new string[0];
		mKnownDialogueIDs = new string[0];
		pPlayer.networkView.RPC ("ClearRecording", RPCMode.All);
		mRecordingPlayer = pPlayer;
	}

	public void StopRecording() {
		mRecordingPlayer = null;
	}

	public void RecordAudio(PurchasedAudio pAudio) {
		if (mRecordingPlayer != null) {
			mRecordingPlayer.networkView.RPC ("RecordAction", RPCMode.All, new object[]{"AudioChange", RPCEncoder.Encode(new string[]{mTime.ToString (), pAudio.uAudio.uID})});
		}
	}

	void Update() {
		if (mRecordingPlayer) {
			// We are recording
			mTime += Time.deltaTime;

			RecordProps();
			RecordDialogue();
		}
	}

	void RecordProps() {
		// First loop through all of the recordable objects in screen to check if any have been added/removed
		RecordingProp[] currentRecordingProps = FindObjectsOfType<RecordingProp>().Where (rp => rp.GetType () != typeof(RecordingDialogue)).ToArray();
		
		// Get the IDs from all current props
		string[] currentIDs = (from prop in currentRecordingProps select prop.uPurchasedProp.uID).ToArray ();
		
		// Check if any have been added
		foreach(string ID in currentIDs) {
			if (!mKnownPropIDs.Contains(ID)) {
				// This is a new prop
				
				// Get the full prop information
				RecordingProp p = currentRecordingProps.Where (prop => prop.uPurchasedProp.uID == ID).First ();
				dfTextureSprite sprite = (dfTextureSprite) p.gameObject.GetComponent (typeof(dfTextureSprite));

				float x_pos = sprite.Position.x;// - (sprite.Width / 2);
				float y_pos = sprite.Position.y;// + (sprite.Height / 2);
				mRecordingPlayer.networkView.RPC ("RecordAction", RPCMode.All, new object[]{"InstantiationChange", RPCEncoder.Encode(new string[]{mTime.ToString (), p.uPurchasedProp.uProp.uID, p.uPurchasedProp.uID,x_pos.ToString(),y_pos.ToString(), sprite.Size.x.ToString(), sprite.Size.y.ToString(), sprite.ZOrder.ToString ()})});
				mKnownPositions[ID] = sprite.Position;
				mKnownZOrders[ID] = sprite.ZOrder;
				mKnownSizes[ID] = sprite.Size;
			}
		}
		
		// Check if any have been removed
		foreach(string ID in mKnownPropIDs) {
			if (!currentIDs.Contains (ID)) {
				// This prop has been removed
				mRecordingPlayer.networkView.RPC ("RecordAction", RPCMode.All, new object[]{"DestroyChange", RPCEncoder.Encode(new string[]{mTime.ToString (), ID})});
			}
		}
		
		mKnownPropIDs = currentIDs;
		
		// Then check if any have moved position
		foreach(RecordingProp p in currentRecordingProps) {
			dfTextureSprite sprite = (dfTextureSprite) p.gameObject.GetComponent (typeof(dfTextureSprite));
			if (sprite.Position != mKnownPositions[p.uPurchasedProp.uID]) {
				// The prop has moved
				mRecordingPlayer.networkView.RPC ("RecordAction", RPCMode.All, new object[]{"PositionChange", RPCEncoder.Encode(new string[]{mTime.ToString (), p.uPurchasedProp.uID, sprite.Position.x.ToString(), sprite.Position.y.ToString()})});
				mKnownPositions[p.uPurchasedProp.uID] = sprite.Position;
			}
			
			if (sprite.ZOrder != mKnownZOrders[p.uPurchasedProp.uID]) {
				// The prop has moved ZOrder
				mRecordingPlayer.networkView.RPC ("RecordAction", RPCMode.All, new object[]{"ZOrderChange", RPCEncoder.Encode(new string[]{mTime.ToString (), p.uPurchasedProp.uID, sprite.ZOrder.ToString()})});
				mKnownZOrders[p.uPurchasedProp.uID] = sprite.ZOrder;
			}
			
			if (sprite.Size != mKnownSizes[p.uPurchasedProp.uID]) {
				// The prop has changed size
				mRecordingPlayer.networkView.RPC ("RecordAction", RPCMode.All, new object[]{"SizeChange", RPCEncoder.Encode(new string[]{mTime.ToString (), p.uPurchasedProp.uID, sprite.Size.x.ToString(), sprite.Size.y.ToString ()})});
				mKnownSizes[p.uPurchasedProp.uID] = sprite.Size;
			}
		}
	}

	void RecordDialogue() {
		// First loop through all of the recordable objects in screen to check if any have been added/removed
		RecordingDialogue[] currentDialogues = FindObjectsOfType<RecordingDialogue>();
		
		// Get the IDs from all current text
		string[] currentIDs = (from dialogue in currentDialogues select dialogue.uID).ToArray ();
		
		// Check if any have been added
		foreach(string ID in currentIDs) {
			if (!mKnownDialogueIDs.Contains(ID)) {
				// This is a new dialogue

				RecordingDialogue d = currentDialogues.Where (dialogue => dialogue.uID == ID).First ();
				dfSlicedSprite sprite = d.gameObject.GetComponent<dfSlicedSprite>();
				dfLabel label = sprite.GetComponentInChildren<dfLabel>();
				mRecordingPlayer.networkView.RPC ("RecordAction", RPCMode.All, new object[]{"DialogueInstantiationChange", RPCEncoder.Encode(new string[]{mTime.ToString (), d.uID, sprite.SpriteName, label.Text, label.TextScale.ToString (), sprite.Position.x.ToString (),sprite.Position.y.ToString(), sprite.Size.x.ToString(), sprite.Size.y.ToString(), sprite.ZOrder.ToString()})});
				mKnownDialogueTexts[ID] = label.Text;
				mKnownDialogueTextScales[ID] = label.TextScale;
				mKnownPositions[ID] = sprite.Position;
				mKnownZOrders[ID] = sprite.ZOrder;
				mKnownSizes[ID] = sprite.Size;
			}
		}
		
		// Check if any have been removed
		foreach(string ID in mKnownDialogueIDs) {
			if (!currentIDs.Contains (ID)) {
				// This dialogue has been removed
				mRecordingPlayer.networkView.RPC ("RecordAction", RPCMode.All, new object[]{"DestroyChange", RPCEncoder.Encode(new string[]{mTime.ToString (), ID})});
			}
		}
		
		mKnownDialogueIDs = currentIDs;
		
		// Then check if any have moved position
		foreach(RecordingDialogue p in currentDialogues) {
			dfSlicedSprite sprite = p.gameObject.GetComponent<dfSlicedSprite>();
			dfLabel label = sprite.GetComponentInChildren<dfLabel>();
			if (sprite.Position != mKnownPositions[p.uID]) {
				// The prop has moved
				mRecordingPlayer.networkView.RPC ("RecordAction", RPCMode.All, new object[]{"PositionChange", RPCEncoder.Encode(new string[]{mTime.ToString (), p.uID, sprite.Position.x.ToString(), sprite.Position.y.ToString()})});
				mKnownPositions[p.uID] = sprite.Position;
			}
			
			if (sprite.ZOrder != mKnownZOrders[p.uID]) {
				// The prop has moved ZOrder
				mRecordingPlayer.networkView.RPC ("RecordAction", RPCMode.All, new object[]{"ZOrderChange", RPCEncoder.Encode(new string[]{mTime.ToString (), p.uID, sprite.ZOrder.ToString()})});
				mKnownZOrders[p.uID] = sprite.ZOrder;
			}
			
			if (sprite.Size != mKnownSizes[p.uID]) {
				// The prop has changed size
				mRecordingPlayer.networkView.RPC ("RecordAction", RPCMode.All, new object[]{"SizeChange", RPCEncoder.Encode(new string[]{mTime.ToString (), p.uID, sprite.Size.x.ToString(), sprite.Size.y.ToString ()})});
				mKnownSizes[p.uID] = sprite.Size;
			}

			if (label.Text != mKnownDialogueTexts[p.uID]) {
				// The prop has changed size
				mRecordingPlayer.networkView.RPC ("RecordAction", RPCMode.All, new object[]{"DialogueTextChange", RPCEncoder.Encode(new string[]{mTime.ToString (), p.uID, label.Text})});
				mKnownDialogueTexts[p.uID] = label.Text;
			}

			if (label.TextScale != mKnownDialogueTextScales[p.uID]) {
				// The prop has changed size
				mRecordingPlayer.networkView.RPC ("RecordAction", RPCMode.All, new object[]{"DialogueTextScaleChange", RPCEncoder.Encode(new string[]{mTime.ToString (), p.uID, label.TextScale.ToString()})});
				mKnownDialogueTextScales[p.uID] = label.TextScale;
			}
		}
	}
}