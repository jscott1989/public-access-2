using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	public Dictionary<string, Prop> uProps = new Dictionary<string, Prop>();

	// This means that a single "ready" is enough to move everyone on - just to hurry during testing
	public bool DEBUG_MODE2 = false;

	public int LOBBY_COUNTDOWN = 6;
	public int PROP_SELECTION_COUNTDOWN = 60;
	public int PREPARING_COUNTDOWN = 5; // 30
	public int RECORDING_COUNTDOWN = 10; // 30
	public int NUMBER_OF_DAYS = 5;

	void AddProp(Prop pProp) {
		uProps.Add (pProp.uID, pProp);
	}

	void Start() {
		// TODO: This should be able to go as part of the definition - but I'm on a train so can't google how

		// TODO: Consider if the prices should be set at this level or set when the game state is generated (to balance the game)
		AddProp (new Prop("bear", "Bear", 100, new string[]{"animal", "cute", "toy", "brown"}));
		AddProp (new Prop("bible", "bible", 50, new string[]{"religious", "book", "red"}));
		AddProp (new Prop("soldier", "Soldier", 100, new string[]{"toy", "war", "green"}));
	}
}
