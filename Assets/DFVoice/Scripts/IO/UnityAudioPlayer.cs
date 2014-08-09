/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	using UnityEngine;

	/// <summary>
	/// Implements an audio player which writes to a streaming audio clip
	/// </summary>
	[AddComponentMenu( "DFVoice/Unity Audio Player" )]
	[RequireComponent( typeof( AudioSource ) )]
	public class UnityAudioPlayer : MonoBehaviour, IAudioPlayer
	{
		public bool PlayingSound
		{
			get
			{
				return audio.isPlaying;
			}
		}

		public bool IsThreeDimensional = false;

		public bool Equalize = false;
		public float EqualizeSpeed = 1f;
		public float TargetEqualizeVolume = 0.75f;
		public float MaxEqualization = 5f;

		private int frequency = 16000;

		private int writeHead = 0;
		private int totalWritten = 0;
		private AudioClip playClip;

		private int delayForFrames = 0;
		private int lastTime = 0;
		private int played = 0;

		private float currentGain = 1f;
		private float targetGain = 1f;

		void Start()
		{
			playClip = AudioClip.Create( "vc", frequency * 10, 1, frequency, IsThreeDimensional, false );

			// backwards compatibility
			if( audio == null )
				gameObject.AddComponent<AudioSource>();

			audio.clip = playClip;
			audio.Stop();
			audio.loop = true;
		}

		void Update()
		{
			if( audio.isPlaying )
			{
				if( lastTime > audio.timeSamples )
				{
					played += audio.clip.samples;
				}

				lastTime = audio.timeSamples;

				currentGain = Mathf.MoveTowards( currentGain, targetGain, Time.deltaTime * EqualizeSpeed );

				if( played + audio.timeSamples >= totalWritten )
				{
					audio.Pause();
					delayForFrames = 2;
				}
			}
		}

		void OnDestroy()
		{
			Destroy( audio.clip );
		}

		public void SetSampleRate( int sampleRate )
		{
			if( audio == null ) return;
			if( audio.clip.frequency == sampleRate ) return;

			this.frequency = sampleRate;

			Destroy( audio.clip );
			playClip = AudioClip.Create( "vc", frequency * 10, 1, frequency, IsThreeDimensional, false );
			audio.clip = playClip;
			audio.Stop();
			audio.loop = true;

			writeHead = 0;
			totalWritten = 0;
			delayForFrames = 0;
			lastTime = 0;
			played = 0;
		}

		public void BufferAudio( BigArray<float> audioData )
		{
			if( audio == null ) return;

			float[] temp = TempArray<float>.Obtain( audioData.Length );
			audioData.CopyTo( 0, temp, 0, audioData.Length * 4 );

			if( Equalize )
			{
				float maxAmp = AudioUtils.GetMaxAmplitude( temp );
				targetGain = TargetEqualizeVolume / maxAmp;

				if( targetGain > MaxEqualization )
					targetGain = MaxEqualization;

				if( targetGain < currentGain )
				{
					currentGain = targetGain;
				}

				AudioUtils.ApplyGain( temp, currentGain );
			}

			playClip.SetData( temp, writeHead );
			TempArray<float>.Release( temp );

			writeHead += audioData.Length;
			totalWritten += audioData.Length;
			writeHead %= playClip.samples;

			if( !audio.isPlaying )
			{
				delayForFrames--;
				if( delayForFrames <= 0 )
				{
					audio.Play();
				}
			}
		}
	}
}