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
	public string[] uAvailableBackdrops;
	public string[] uThemes;
	public string[][] uNeeds;
	public int uPlayers;

	Dictionary<string, string[]> mShows = new Dictionary<string, string[]>();
	Dictionary<string, string[]> mActivities = new Dictionary<string, string[]>();
	Dictionary<string, string[]> mPeople = new Dictionary<string, string[]>();

	// Map the template to the type of variable needed (show, thing, activity, person)
	Dictionary<string, string[]> themeTemplates = new Dictionary<string, string[]>() {
		{"a show like (show) but with (thing)", new string[]{"show", "thing"}},
		{"(activity) with (person)", new string[]{"activity", "person"}},
		{"when (person) met (person)", new string[]{"person", "person"}}
	};


	/**
	 * Generate a theme which hints at the given needs
	 */
	string GenerateTheme(string[] pNeeds) {
		// God I wrote this code late... it's not great....
		System.Random rnd = new System.Random();

		List<string> availableShows = mShows.Where(kvp => kvp.Value.Intersect(pNeeds).Count () > 0).Select (kvp => kvp.Key).OrderBy (x => rnd.Next ()).ToList();
		List<string> availableActivities = mActivities.Where(kvp => kvp.Value.Intersect(pNeeds).Count () > 0).Select (kvp => kvp.Key).OrderBy (x => rnd.Next ()).ToList();
		List<string> availablePeople = mPeople.Where(kvp => kvp.Value.Intersect(pNeeds).Count () > 0).Select (kvp => kvp.Key).OrderBy (x => rnd.Next ()).ToList();
		List<string> availableThings = new List<string>();
		availableThings.AddRange (availableActivities);
		availableThings.AddRange (availablePeople);
		availableThings = availableThings.OrderBy (x => rnd.Next ()).ToList ();

		Dictionary<string, List<string>> variables = new Dictionary<string, List<string>>() {
			{"show", availableShows},
			{"activity", availableActivities},
			{"person", availablePeople},
			{"thing", availableThings}
		};

		List<string> availableVariables = new List<string>();
		foreach(KeyValuePair<string, List<string>> kvp in variables) {
			foreach(string v in kvp.Value) {
				availableVariables.Add (kvp.Key);
			}
		}

		string[] availableTemplates = themeTemplates.Where(kvp => kvp.Value.Intersect(availableVariables).Count() == kvp.Value.Count()).Select(kvp => kvp.Key).ToArray();

		if (availableTemplates.Count() < 1) {
			// TODO: This shouldn't happen - but hardcode a few themes just incase
			return string.Join (" ", pNeeds);
//			return "I couldn't create a theme";
		}

		string template = availableTemplates[rnd.Next(availableTemplates.Count())];

		foreach(KeyValuePair<string, List<string>> kvp in variables) {
			foreach(string v in kvp.Value) {
				template = template.Replace ("(" + kvp.Key + ")", v);
			}
		}

		return template;
	}

	public GameSetup(int pPlayers) {
		Game mGame = FindObjectOfType<Game>();

		// Load shows, activities, things, and people
		mShows.Add ("The Lion King", new string[]{"animal"});
		mShows.Add ("Songs of Praise", new string[]{"religious"});
		mShows.Add ("Home Improvement", new string[]{"tool"});

		mActivities.Add ("Reading", new string[]{"book"});
		mActivities.Add ("Praying", new string[]{"religious"});
		mActivities.Add ("Playing", new string[]{"toy", "cute"});
		mActivities.Add ("Flying", new string[]{"bird"});
		mActivities.Add ("Shooting", new string[]{"weapon"});


		mPeople.Add ("God", new string[]{"religious"});
		mPeople.Add ("Muhammad", new string[]{"religious"});
		mPeople.Add ("Dora The Explorer", new string[]{"religious"});
		mPeople.Add ("a parrot", new string[]{"bird", "animal"});
		mPeople.Add ("the Statue of Liberty", new string[]{"statue"});
		mPeople.Add ("an apple", new string[]{"fruit"});
		mPeople.Add ("Woody from Toy Story", new string[]{"toy"});
		mPeople.Add ("an electrician", new string[]{"tool"});
		mPeople.Add ("a cow", new string[]{"toy", "animal"});

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
					// When the list of remaining needs is empty, refill it
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


		foreach(List<string> n in needs) {
			// Now for each player, set every second need as a negative one (a "dislike")
			for(int i = 0; i < n.Count; i++) {
				if (i % 2 == 1) {
					n[i] = "-" + n[i];
				}
			}
		}

		List<string> themes = new List<string>();
		for(int i = 0; i < pPlayers; i++) {

			List<string> otherNeeds = new List<string>();
			for (int n = 0; n < pPlayers; n++) {
				if (n != i) {
					foreach(string need in needs[n]) {
						if (!need.StartsWith("-")) {
							if (!otherNeeds.Contains(need)) {
								otherNeeds.Add (need);
							}
						}
					}
				}
			}

			themes.Add (GenerateTheme(otherNeeds.ToArray ()));
		}


		List<string[]> needsAsArray = new List<string[]>();
		foreach(List<string> s in needs) {
			needsAsArray.Add (s.ToArray ());
		}

		uPlayers = pPlayers;
		uAvailableProps = availableProps.ToArray ();
		uAvailableBackdrops = new string[]{"mars", "beach"};
		uThemes = themes.ToArray ();
		uNeeds = needsAsArray.ToArray (); // TODO: Right now needs is just a list of tags - we need to have a human readable description of the need
	}
}
