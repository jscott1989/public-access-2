using UnityEngine;
using System.Collections;

public class Countdown : MonoBehaviour {
	public float countdownTimer = 30;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		countdownTimer -= Time.deltaTime;
		guiText.text = countdownTimer.ToString ("0");
	}
}
