using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AfternoonManager : SceneManager {
	NetworkManager mNetworkManager;
	DialogueManager mDialogueManager;
	Countdown mCountdown;
	Game mGame;
	Recorder mRecorder;
	GameObject mScreen;
	AudioSource mAudioSource;

	GameObject mBackdropPrefab;

	Dictionary<PurchasedBackdrop, AfternoonBackdrop> mBackdropMap = new Dictionary<PurchasedBackdrop, AfternoonBackdrop>();

	public bool uRecording = false;

	public PurchasedBackdrop[] uPurchasedBackdrops {
		get {
			List<PurchasedBackdrop> backdrops = new List<PurchasedBackdrop>();
			if (mNetworkManager != null & mNetworkManager.myPlayer != null && mNetworkManager.myPlayer.uPurchasedProps != null) {
				foreach(KeyValuePair<string, PurchasedProp> kv in mNetworkManager.myPlayer.uPurchasedProps) {
					if (kv.Value.GetType() == typeof(PurchasedBackdrop)) {
						backdrops.Add ((PurchasedBackdrop)kv.Value);
					}
				}
				return backdrops.ToArray ();
			}
			return new PurchasedBackdrop[0];
		}
	}

	public string[] uPurchasedBackdropStrings {
		get {
			int i = 1;
			List<string> set = new List<string>();
			foreach(PurchasedBackdrop p in uPurchasedBackdrops) {
				set.Add (i.ToString () + " " + p.uProp.uName);
				i++;
			}
			return set.ToArray ();
		}
	}


	public void BackdropClicked( dfControl control, System.Int32 value ) {
		PurchasedBackdrop p = uPurchasedBackdrops[value];
		ActivateBackdrop(p);
	}

	public void ActivateBackdrop(PurchasedBackdrop pBackdrop) {
		foreach(KeyValuePair<PurchasedBackdrop, AfternoonBackdrop> kvp in mBackdropMap) {
			if (!(kvp.Key == pBackdrop)) {
				kvp.Value.Hide ();
			} else {
				kvp.Value.Show ();
			}
		}
	}

	public PurchasedAudio[] uPurchasedAudio {
		get {
			List<PurchasedAudio> audios = new List<PurchasedAudio>();
			foreach(KeyValuePair<string, PurchasedProp> kv in mNetworkManager.myPlayer.uPurchasedProps) {
				if (kv.Value.GetType() == typeof(PurchasedAudio)) {
					audios.Add ((PurchasedAudio)kv.Value);
				}
			}
			return audios.ToArray ();
		}
	}
	
	public string[] uPurchasedAudioStrings {
		get {
			int i = uPurchasedBackdrops.Length + 1;
			List<string> set = new List<string>();
			foreach(PurchasedAudio p in uPurchasedAudio) {
				set.Add (i.ToString () + " " + p.uProp.uName);
				i++;
			}
			return set.ToArray ();
		}
	}
	
	
	public void AudioClicked( dfControl control, System.Int32 value ) {
		PurchasedAudio p = uPurchasedAudio[value];
		ActivateAudio(p);
	}
	
	public void ActivateAudio(PurchasedAudio pAudio) {
		if (!mAudioSource.isPlaying) {
			mRecorder.RecordAudio(pAudio);
			mAudioSource.clip = pAudio.uAudio.uClip;
			mAudioSource.Play();
		};
	}

	void KeyPressed(int i) {
		if (i > uPurchasedBackdrops.Length) {
			i -= uPurchasedBackdrops.Length;
			// Now use i against the sounds
			if (uPurchasedAudio.Length > i-1) {
				ActivateAudio(uPurchasedAudio[i - 1]);
			}
			return;
		}

		if (uPurchasedBackdrops.Length > i-1) {
			ActivateBackdrop (uPurchasedBackdrops[i - 1]);
		}
	}

	void Update() {
		for(int i = 0; i < 10; i++) {
			if (Input.GetKeyDown (i.ToString ())) {
				// Activate this backdrop/sound
				KeyPressed(i);
			}
		}
	}

	public string uStateText {
		get {
			if (uRecording) {
				return "Recording";
			} else {
				return "Preparing";
			}
		}
	}

	public string uShowTitle {
		get {
			if (mNetworkManager == null) {
				return "";
			}
			return mNetworkManager.myPlayer.uShowName;
		}
	}
	
	public string uShowDescription {
		get {
			if (mNetworkManager == null) {
				return "";
			}
			return mNetworkManager.myPlayer.uTheme;
		}
	}

	void Awake() {
		mNetworkManager = FindObjectOfType<NetworkManager>();
		mDialogueManager = FindObjectOfType<DialogueManager>();
		mCountdown = FindObjectOfType<Countdown>();
		mGame = FindObjectOfType<Game>();
		mRecorder = FindObjectOfType<Recorder>();
		mScreen = GameObject.FindGameObjectWithTag("Screen");
		mBackdropPrefab = (GameObject)Resources.Load ("Afternoon/Prefabs/Backdrop");
		mAudioSource = GetComponent<AudioSource>();
	}

	void Start () {
		// First we need to set everyone to "Not Ready"
		if (Network.isServer) {
			foreach (Player player in mNetworkManager.players) {
				player.networkView.RPC ("SetReady", RPCMode.All, false);
			}
		}

		if (mNetworkManager.myPlayer.uDay == 1) {
			StartDay1 ();
		} else {
			StartPreparing();
		}
	}

	void StartDay1() {
		if (!Game.DEBUG_MODE) {
			string[] afternoon1Dialogue = new string[] {
				"Some stuff about afternoon...",
				"introduce it..."
			};
		
			Action afternoon1DialogueComplete =
				() => {
					mDialogueManager.StartDialogue ("Waiting for other players to continue");
					mNetworkManager.myPlayer.networkView.RPC ("SetReady", RPCMode.All, true);
			};
		
			mDialogueManager.StartDialogue (afternoon1Dialogue, afternoon1DialogueComplete);
		} else {
			StartPreparing();
		}
	}

	/**
	 * This is called on the server when any player changes their ready status
	 */
	public override void ReadyStatusChanged(Player pPlayer) {
		if (pPlayer.uReady) {
			// Check if all players are ready
			if (!Game.DEBUG_MODE) {
				foreach (Player p in mNetworkManager.players) {
					if (!p.uReady) {
						return;
					}
				}
			}
			
			// Everyone is ready, let's move on
			if (!uRecording) {
				// This must be the dialogue
				networkView.RPC ("StartPreparing", RPCMode.All);
			} else {
				// This must indicate everyone is finished recording - move on to the evening stage
				networkView.RPC ("MoveToNextScene", RPCMode.All);
			}
		}
	}

	[RPC] void StartPreparing() {
		if (Network.isServer) {
			// Set everyone to "not ready" so we can wait again after the recording stage
			foreach (Player player in mNetworkManager.players) {
				player.networkView.RPC ("SetReady", RPCMode.All, false);
			}
		}

		mDialogueManager.EndDialogue();


		// First, instatiate all of the backdrops
		foreach (PurchasedBackdrop purchasedBackdrop in uPurchasedBackdrops) {
			GameObject g = (GameObject) Instantiate(mBackdropPrefab, Vector3.zero, Quaternion.identity);
			g.transform.parent = mScreen.transform;

			dfTextureSprite sprite = (dfTextureSprite)g.GetComponent (typeof(dfTextureSprite));
			sprite.Position = new Vector2(0, 550);
			sprite.Texture = (Texture2D)Resources.Load("Props/" + purchasedBackdrop.uProp.uID);

			RecordingProp r = (RecordingProp)g.GetComponent (typeof(RecordingProp));
			r.uPurchasedProp = purchasedBackdrop;

			mBackdropMap.Add (purchasedBackdrop, g.GetComponent<AfternoonBackdrop>());
		}


		uRecording = false;
		Action finishedPreparing =
			() => {
			StartRecording();
		};

		mCountdown.StartCountdown (Game.PREPARING_COUNTDOWN, finishedPreparing);
	}

	void StartRecording() {
		mRecorder.StartRecording (mNetworkManager.myPlayer, mScreen);
		uRecording = true;
		Action finishedRecording =
			() => {
			mRecorder.StopRecording ();
			mDialogueManager.StartDialogue("Waiting for other players to finish recording");
			mNetworkManager.myPlayer.networkView.RPC("SetReady", RPCMode.All, true);
		};

		mCountdown.StartCountdown (Game.RECORDING_COUNTDOWN, finishedRecording);
	}

	/**
	 * Move to the prop selection scene
	 */
	[RPC] void MoveToNextScene() {
		mDialogueManager.EndDialogue();
		mNetworkManager.LoadLevel ("Evening");
	}
}
