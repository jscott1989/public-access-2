using UnityEngine;
using System.Collections;
using DaikonForge.VoIP;

public class VoiceController : VoiceControllerBase {
	public override bool IsLocal {
		get {
			return networkView.isMine;
		}
	}

	protected override void OnAudioDataEncoded( VoicePacketWrapper encodedFrame )
	{
		byte[] headers = encodedFrame.ObtainHeaders();
		networkView.RPC("vc", RPCMode.All, headers, encodedFrame.RawData );
		encodedFrame.ReleaseHeaders();
	}

	[RPC]
	void vc( byte[] headers, byte[] rawData ) {
		VoicePacketWrapper packet = new VoicePacketWrapper( headers, rawData );
		ReceiveAudioData( packet );
	}
}
