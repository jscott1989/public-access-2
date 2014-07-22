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

	Dictionary<string, string[]> shows = new Dictionary<string, string[]>() {
		{"A", new string[]{"B","C"}},
		{"B", new string[]{"B","C"}},
	};
	
	string[] themeTemplates = new string[] {
		"like (show) but with (thing)",
		"(activity) with (person)",
		"when (person) met (person)"
	};


	string GenerateTheme() {
		System.Random rnd = new System.Random();
		string template = themeTemplates[rnd.Next(themeTemplates.Count())];


		// TODO: Collect all tags used by all other players

		// TODO: Then select two shows, two activities, two things, and two people who cover as many of those tags as possible
		string showToUse1 = "";
		string showToUse2 = "";
		string activityToUse1 = "";
		string activityToUse2 = "";
		string thingToUse1 = "";
		string thingToUse2 = "";
		string personToUse1 = "";
		string personToUse2 = "";

		template = template.Replace ("(show)", showToUse1);
		template = template.Replace ("(show)", showToUse2);
		template = template.Replace ("(activity)", activityToUse1);
		template = template.Replace ("(activity)", activityToUse2);
		template = template.Replace ("(thing)", thingToUse1);
		template = template.Replace ("(thing)", thingToUse2);
		template = template.Replace ("(person)", personToUse1);
		template = template.Replace ("(person)", personToUse2);

		return template;
	}

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

		List<string> themes = new List<string>();
		for(int i = 0; i < pPlayers; i++) {
			themes.Add (GenerateTheme());
		}


		List<string[]> needsAsArray = new List<string[]>();
		foreach(List<string> s in needs) {
			needsAsArray.Add (s.ToArray ());
		}

		uPlayers = pPlayers;
		uAvailableProps = availableProps.ToArray ();
		uThemes = themes.ToArray ();
		uNeeds = needsAsArray.ToArray (); // TODO: Right now needs is just a list of tags - we need to have a human readable description of the need
	}
}
