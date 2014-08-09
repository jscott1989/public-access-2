/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	using UnityEngine;

	public class AudioClipInputDevice : AudioInputDeviceBase
	{
		public AudioClip testClip;

		public override void StartRecording()
		{
			float[] data = new float[ testClip.samples ];
			testClip.GetData( data, 0 );

			BigArray<float> d = new BigArray<float>( data.Length, 0 );
			d.Resize( data.Length );
			d.CopyFrom( data, 0, 0, data.Length * 4 );

			AudioUtils.Resample( d, testClip.frequency, 16000 );
			//Debug.Log( "Buffer ready: " + d.Length + " samples" );

			bufferReady( d, 16000 );
		}

		public override void StopRecording()
		{
		}
	}
}