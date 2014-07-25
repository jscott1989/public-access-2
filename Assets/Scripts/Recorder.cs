using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Recorder : MonoBehaviour {
	Player mRecordingPlayer;
	GameObject mRecordingScreen;

	string[] mKnownPropIDs;
	Dictionary<string, Vector3> mKnownPropPositions = new Dictionary<string, Vector3>();
	Dictionary<string, Vector2> mKnownSizes = new Dictionary<string, Vector2>();
	Dictionary<string, int> mKnownZOrders = new Dictionary<string, int>();

	double mTime = 0;

	/**
	 * Start recording to pPlayer's latest episode
	 */
	public void StartRecording(Player pPlayer, GameObject pScreen) {
		mTime = 0;
		mKnownPropIDs = new string[0];
		pPlayer.networkView.RPC ("ClearRecording", RPCMode.All);
		mRecordingPlayer = pPlayer;
		mRecordingScreen = pScreen;
	}

	public void StopRecording() {
		mRecordingPlayer = null;
	}

	public void RecordAudio(PurchasedAudio pAudio) {
		mRecordingPlayer.networkView.RPC ("RecordAction", RPCMode.All, new object[]{"AudioChange", RPCEncoder.Encode(new string[]{mTime.ToString (), pAudio.uAudio.uID})});
	}

	void Update() {
		if (mRecordingPlayer) {
			// We are recording
			mTime += Time.deltaTime;

			// First loop through all of the recordable objects in screen to check if any have been added/removed
			// TODO: This should be fine - but it might be better to specify mRecordingScreen as the parent
			RecordingProp[] currentRecordingProps = FindObjectsOfType<RecordingProp>();

			// Get the IDs from all current props
			string[] currentIDs = (from prop in currentRecordingProps select prop.uPurchasedProp.uID).ToArray ();

			// Check if any have been added
			foreach(string ID in currentIDs) {
				if (!mKnownPropIDs.Contains(ID)) {
					// This is a new prop

					// Get the full prop information
					// TODO: This could be done more efficiently with a LINQ query - but I don't know how - figure it out
					foreach(RecordingProp p in currentRecordingProps) {
						if (p.uPurchasedProp.uID == ID) {
							dfTextureSprite sprite = (dfTextureSprite) p.gameObject.GetComponent (typeof(dfTextureSprite));

							mRecordingPlayer.networkView.RPC ("RecordAction", RPCMode.All, new object[]{"InstantiationChange", RPCEncoder.Encode(new string[]{mTime.ToString (), p.uPurchasedProp.uProp.uID, p.uPurchasedProp.uID,sprite.Position.x.ToString (),sprite.Position.y.ToString(), sprite.Size.x.ToString(), sprite.Size.y.ToString()})});
							mKnownPropPositions[ID] = sprite.Position;
							mKnownZOrders[ID] = sprite.ZOrder;
							mKnownSizes[ID] = sprite.Size;
							break;
						}
					}
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
				if (sprite.Position != mKnownPropPositions[p.uPurchasedProp.uID]) {
					// The prop has moved
					mRecordingPlayer.networkView.RPC ("RecordAction", RPCMode.All, new object[]{"PositionChange", RPCEncoder.Encode(new string[]{mTime.ToString (), p.uPurchasedProp.uID, sprite.Position.x.ToString(), sprite.Position.y.ToString()})});
					mKnownPropPositions[p.uPurchasedProp.uID] = sprite.Position;
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
	}
}
