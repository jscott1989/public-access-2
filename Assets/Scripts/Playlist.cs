using UnityEngine;
using System.Collections;

public class Playlist : MonoBehaviour {

	public AudioClip[] uAudioClips;
	private int mCurrentIndex = 0;

	AudioSource mAudioSource;

	void Awake(){

		mAudioSource = GetComponent<AudioSource> ();

	}

	void Start(){

		PlayNextClip ();

	}

	void PlayNextClip(){
	
		mCurrentIndex += 1;
		if (mCurrentIndex >= uAudioClips.Length) {
			mCurrentIndex = 0;		
		}

		mAudioSource.clip = uAudioClips [mCurrentIndex];

		mAudioSource.Play ();
	}

	void Update(){


		if (!mAudioSource.isPlaying) {
		
			PlayNextClip();
		}
	}

}
