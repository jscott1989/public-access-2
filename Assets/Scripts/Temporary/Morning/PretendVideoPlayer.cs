using UnityEngine;
using System.Collections;

/**
 * Since we don't yet have a usable video player script this will just increment the timer and interact with the rest of the scene
 */
public class PretendVideoPlayer : MonoBehaviour {
	float timePassed = 0;

	public ViewerGraph graph;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timePassed += Time.deltaTime;

		if (timePassed >= 31) {
			timePassed = 0;
		}
		guiText.text = ((int)timePassed).ToString () + " seconds";

		graph.SetTime (timePassed);
	}

	public void SetTime(float time) {
		timePassed = time;
	}
}
