using UnityEngine;
using System.Collections;

public class Playlist : MonoBehaviour {

	public AudioClip[] uAudioClips;
	private int mCurrentIndex = 0;

	private bool isPlaying = true;

	AudioSource mAudioSource;

	void Awake(){
		mAudioSource = GetComponent<AudioSource> ();
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
		if (!mAudioSource.isPlaying && isPlaying) {
			PlayNextClip();
		}
	}

	public void StartPlaying() {
		isPlaying = true;
	}

	public void StopPlaying() {
		isPlaying = false;
		mAudioSource.Stop ();
	}
}
