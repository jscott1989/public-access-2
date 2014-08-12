using UnityEngine;
using System.Collections;
using DaikonForge.VoIP;

public class VoiceController : VoiceControllerBase {
	float passedTime = 0;
	float lastRecordedTime = 0;

	Game mGame;


	void Awake() {
		mGame = FindObjectOfType<Game>();
	}

	void Update() {
		passedTime += Time.deltaTime;
	}

	public override bool IsLocal {
		get {
			return networkView.isMine;
		}
	}

	public bool IsRecording {
		get {
			if (!mGame.uVoiceChatEnabled) {
				return false;
			}
			return (lastRecordedTime - passedTime) < 1;
		}
	}

	public bool IsPlaying {
		get {
			if (!mGame.uVoiceChatEnabled) {
				return false;
			}
			return GetComponent<UnityAudioPlayer>().PlayingSound;
		}
	}

	protected override void OnAudioDataEncoded( VoicePacketWrapper encodedFrame )
	{
		if (mGame.uVoiceChatEnabled) {
			lastRecordedTime = passedTime;
			byte[] headers = encodedFrame.ObtainHeaders();
			networkView.RPC("vc", RPCMode.All, headers, encodedFrame.RawData );
			encodedFrame.ReleaseHeaders();
		}
	}

	[RPC]
	void vc( byte[] headers, byte[] rawData ) {
		VoicePacketWrapper packet = new VoicePacketWrapper( headers, rawData );
		ReceiveAudioData( packet );
	}
}
