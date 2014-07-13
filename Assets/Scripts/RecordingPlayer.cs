using UnityEngine;
using System.Collections;

public class RecordingPlayer : MonoBehaviour {
	Player mPlayingPlayer;
	GameObject mPlayingScreen;

	/**
	 * Start playing back pPlayer's latest episode on pScreen
	 */
	public void Play(Player pPlayer, GameObject pScreen) {
		mPlayingPlayer = pPlayer;
		mPlayingScreen = pScreen;
	}
	
	/**
	 * Jump the current playback to pTime
	 */
	public void Jump(float pTime) {
		
	}
}
