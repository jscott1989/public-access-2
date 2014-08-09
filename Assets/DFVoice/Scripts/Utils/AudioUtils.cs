/* Copyright (C) 2014 DaikonForge */

namespace DaikonForge.VoIP
{
	using UnityEngine;

	public enum FrequencyMode
	{
		Narrow,
		Wide,
		UltraWide
	}

	public class AudioUtils
	{
		private static FastList<float> temp = new FastList<float>();

		public static int GetFrequency( FrequencyMode mode )
		{
			switch( mode )
			{
				case FrequencyMode.Narrow:
					return 8000;
				case FrequencyMode.Wide:
					return 16000;
				case FrequencyMode.UltraWide:
					return 32000;
				default:
					return 16000;
			}
		}

		public static void Resample( BigArray<float> samples, int oldFrequency, int newFrequency )
		{
			if( oldFrequency == newFrequency ) return;

			temp.Clear();

			float frequencyChange = (float)oldFrequency / (float)newFrequency;
			int newArrLen = Mathf.FloorToInt( (float)samples.Length / frequencyChange );

			temp.EnsureCapacity( newArrLen );
			temp.ForceCount( newArrLen );

			float c = 0f;
			for( int i = 0; i < newArrLen; i++ )
			{
				int sampleIndex = Mathf.FloorToInt( c );
				float remainder = c - sampleIndex;

				float sample = samples[ sampleIndex ];
				if( sampleIndex < ( newArrLen - 1 ) )
				{
					sample += samples[ sampleIndex + 1 ] * remainder;
				}

				temp[ i ] = sample;
				c += frequencyChange;
			}

			samples.Resize( newArrLen );
			samples.CopyFrom( temp.Items, 0, 0, newArrLen * 4 );
		}

		public static void ApplyGain( float[] samples, float gain )
		{
			for( int i = 0; i < samples.Length; i++ )
				samples[ i ] *= gain;
		}

		public static float GetMaxAmplitude( float[] samples )
		{
			float max = 0f;
			for( int i = 0; i < samples.Length; i++ )
				max = Mathf.Max( max, Mathf.Abs( samples[ i ] ) );
			return max;
		}
	}
}