using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
 * This represents a collection of props, themes, and needs which are balanced
 * to make the game interesting and winnable
 */
public class GameSetup : UnityEngine.Object {

	public string[] uAvailableProps;
	public string[] uThemes;
	public string[] uNeeds;
	public int uPlayers;

	public static GameSetup generate(int pPlayers) {
		GameSetup setup = new GameSetup ();

		List<string> themes = new List<string> ();
		themes.AddRange (new string[] {
						"Like Star Wars but with Fish",
						"LOST: The Musical",
						"Cat Football",
						"Why God hates Football",
						"When Barack Obama met Clint Eastwood"
				});

		List<string> needs = new List<string> ();
		needs.AddRange (new string[]{"toy", "space", "red", "black", "music", "loud"});


		int numberOfTags = (pPlayers * Game.NUMBER_OF_DAYS) / 2;

		System.Random rnd = new System.Random();
		string[] tags = Game.uTags.OrderBy(x => rnd.Next()).Take(numberOfTags).ToArray();

		// From the tags array we will pull the needs and select appropriate props and themes

		int numberOfAvailableProps = numberOfTags * 2;



		setup.uPlayers = pPlayers;
		setup.uAvailableProps = new string[]{"dog", "cat", "strawberry", "tv"};
		setup.uThemes = themes.Take (pPlayers).ToArray ();
		setup.uNeeds = needs.Take (pPlayers).ToArray ();

		return setup;
	}
}
