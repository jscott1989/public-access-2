using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
 * This represents a collection of props, themes, and needs which are balanced
 * to make the game interesting and winnable
 */
public class GameSetup : Object {

	public string[] uAvailableProps;
	public string[] uThemes;
	public string[] uNeeds;
	public int uPlayers;

	public static GameSetup generate(int pPlayers) {
		// For now we're going to just hardcode some test data
		// TODO: Implement this generator

		GameSetup setup = new GameSetup ();

		List<string> themes = new List<string> ();
		themes.AddRange (new string[] {
						"Like Star Wars but with Fish",
						"LOST: The Musical",
						"Cat Football",
						"Why God hates Football",
						"When Barack Obama met Clint Eastwood"
				});

		// TODO: Needs will be more complex than this - but right now it's just a single tag per person
		// at the least I'd like multiple tags and a human readable description of the need
		// if there's time it would be nice to specify things like "animals moving fast"

		List<string> needs = new List<string> ();
		needs.AddRange (new string[]{"toy", "space", "red", "black", "music", "loud"});

		setup.uPlayers = pPlayers;
		setup.uAvailableProps = new string[]{"bear","bear","bear","bear","bear","bible","bible","bible","bible","bible","soldier","soldier","soldier","soldier","soldier"};
		setup.uThemes = themes.Take (pPlayers).ToArray ();
		setup.uNeeds = needs.Take (pPlayers).ToArray ();

		return setup;
	}
}
