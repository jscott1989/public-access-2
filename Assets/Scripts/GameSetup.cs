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
	public string[][] uNeeds;
	public int uPlayers;

	public GameSetup(int pPlayers) {

		Game mGame = FindObjectOfType<Game>();

		int numberOfTags = (pPlayers * Game.NUMBER_OF_DAYS) / 2;

		System.Random rnd = new System.Random();
		string[] tags = mGame.uTags.OrderBy(x => rnd.Next()).Take(numberOfTags).ToArray();

		// From the tags array we will pull the needs and select appropriate props and themes

		int numberOfAvailableProps = numberOfTags * 2;

		List<string> availableProps = new List<string>();

		foreach(string tag in tags) {
			availableProps.AddRange(mGame.uProps.Where(p => p.Value.uTags.Contains(tag)).OrderBy (x => rnd.Next ()).Take (2).Select (p => p.Value.uID));
		}

		string[] themes = new string[]{
			"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t"
		};

		List<List<string>> needs = new List<List<string>>();

		List<string> distributingNeeds = new List<string>();

		// TODO: Right now it just distributes these at random - this could result in people needing 5 different colours (if we were unlucky)
		// this may make it unbalanced - consider changing this so it ensures everyone has 1 colour, 1 place, 1 person, etc.

		for(int i = 0; i < pPlayers; i++) {
			List<string> oneNeeds = new List<string>();
			for (int n = 0; n < Game.NUMBER_OF_DAYS; n++) {

				if (distributingNeeds.Count == 0) {
					distributingNeeds.AddRange (tags);
					distributingNeeds = distributingNeeds.OrderBy (s => rnd.Next ()).ToList ();
				}
				// Each player needs one need per day
				string tag = distributingNeeds[0];
				distributingNeeds.Remove(tag);
				oneNeeds.Add(tag);
			}
			needs.Add (oneNeeds);
		}

		string[] themeTemplates = new string[] {
			"like (show) but with (aspect)",
			"(activity) with (person)",
			"when (person) met (person)"
		};

		List<string[]> needsAsArray = new List<string[]>();
		foreach(List<string> s in needs) {
			needsAsArray.Add (s.ToArray ());
		}

		uPlayers = pPlayers;
		uAvailableProps = availableProps.ToArray ();
		uThemes = themes.Take (pPlayers).ToArray ();
		uNeeds = needsAsArray.ToArray (); // TODO: Right now needs is just a list of tags - we need to have a human readable description of the need
	}
}
