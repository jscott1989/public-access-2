using UnityEngine;
using System.Collections;

/**
 * Control clicking the "Quit" button
 */
public class Quit : MonoBehaviour {
	public GameObject networkManagerPrefab;

	void OnClick() {
		// Destroy and recreate the NetworkManager
		// TODO: For some reason it's not destroying
		// TODO: Also, if I'm replacing it like this - I'll need to change anywhere I link this up in the GUI to instead search by tag
		Destroy(GameObject.FindGameObjectWithTag ("NetworkManager"));
		Instantiate (networkManagerPrefab);
		// Destroy all players
		foreach (GameObject o in GameObject.FindGameObjectsWithTag("Player")) {
			Destroy (o);
		}

		Application.LoadLevel ("Main Menu");
	}
}
