using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	public Dictionary<string, Prop> uProps = new Dictionary<string, Prop>();

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
