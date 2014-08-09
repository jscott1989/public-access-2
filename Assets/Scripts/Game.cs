using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


public class Game : MonoBehaviour {
	public Dictionary<string, Prop> uProps = new Dictionary<string, Prop>();
	public Dictionary<string, Backdrop> uBackdrops = new Dictionary<string, Backdrop>();
	public Dictionary<string, Audio> uAudio = new Dictionary<string, Audio>();

	// Theme parts
	public Dictionary<string, string[]> uShows = new Dictionary<string, string[]>();
	public Dictionary<string, string[]> uActivities = new Dictionary<string, string[]>();
	public Dictionary<string, string[]> uPeople = new Dictionary<string, string[]>();
	public Dictionary<string, string[]> uThings = new Dictionary<string, string[]>();

	public int[] uCashPerDay = new int[]{ 300, 100, 200, 100, 100, 100};

	public List<Station> uStations = new List<Station>();
	public Dictionary<string, Station> uStationsByID = new Dictionary<string, Station>();
	public List<string> uTags = new List<string>();
	public Dictionary<string, List<string>> uTagCategories = new Dictionary<string, List<string>>();
	public Dictionary<string, string> uTagHumanReadable = new Dictionary<string, string>();

	// This means that a single "ready" is enough to move everyone on - just to hurry during testing
	public const bool DEBUG_MODE = false;

	public const int LOBBY_COUNTDOWN = 6;
	public const int PROP_SELECTION_COUNTDOWN = 60; // 60
	public const int PREPARING_COUNTDOWN = 20; // 30
	public const int RECORDING_COUNTDOWN = 30; // 30
	public const int EVENING_PREPARING_COUNTDOWN = 5; // 5
	public const int FEEDBACK_COUNTDOWN = 40; // 60
	public const int NUMBER_OF_DAYS = 5; // 5
	public const int CHANNEL_INFORMATION_COUNTDOWN = 2;
	public const int MINIMUM_PLAYERS = 3;

	public const int CREATOR_SCORE_MULTIPLIER = 9;

	public const double CHECKSCORE_TIMEOUT = 0.1;

	public const string RANDOM_STATION_ID = "random";

	void AddProp(Prop pProp) {
		uProps.Add (pProp.uID, pProp);
	}

	void AddBackdrop(Backdrop pBackdrop) {
		uBackdrops.Add (pBackdrop.uID, pBackdrop);
	}

	void AddAudio(Audio pAudio) {
		uAudio.Add (pAudio.uID, pAudio);
	}

	void AddStation(Station pStation) {
		uStations.Add (pStation);
		uStationsByID.Add (pStation.uID, pStation);
	}

	public static PropFileReader ResourceToCSVReader(string filename) {
		TextAsset asset = Resources.Load(filename) as TextAsset;
		Stream s = new MemoryStream(asset.bytes);
		return new PropFileReader(s);
	}

	void Start() {
		//Reading prop data CSV file containing list of available props for theme generation
		using (PropFileReader reader = ResourceToCSVReader("PropSelection"))
		{
			CsvRow row = new CsvRow();
			while (reader.ReadRow(row))
			{
				string[] tags = row[3].Split (',');
				AddProp (new Prop(row[0], row[1], int.Parse(row[2]), tags));
			}
		}

		using (PropFileReader reader = ResourceToCSVReader("Tags"))
		{
			CsvRow row = new CsvRow();
			while (reader.ReadRow(row))
			{
				uTags.Add (row[0]);
				if (!uTagCategories.ContainsKey(row[1])) {
					uTagCategories[row[1]] = new List<string>();
				}
				uTagCategories[row[1]].Add (row[0]);
				uTagHumanReadable[row[0]] = row[2];
			}
		}

		using (PropFileReader reader = ResourceToCSVReader("Shows"))
		{
			CsvRow row = new CsvRow();
			while (reader.ReadRow(row))
			{
				uShows.Add (row[0], row[1].Split (','));
			}
		}

		using (PropFileReader reader = ResourceToCSVReader("Activities"))
		{
			CsvRow row = new CsvRow();
			while (reader.ReadRow(row))
			{
				uActivities.Add (row[0], row[1].Split (','));
			}
		}

		using (PropFileReader reader = ResourceToCSVReader("People"))
		{
			CsvRow row = new CsvRow();
			while (reader.ReadRow(row))
			{
				uPeople.Add (row[0], row[1].Split (','));
			}
		}

		using (PropFileReader reader = ResourceToCSVReader("Things"))
		{
			CsvRow row = new CsvRow();
			while (reader.ReadRow(row))
			{
				uThings.Add (row[0], row[1].Split (','));
			}
		}

		using (PropFileReader reader = ResourceToCSVReader("AudioSelection"))
		{
			CsvRow row = new CsvRow();
			while (reader.ReadRow(row))
			{
				string[] tags = row[3].Split (',');
				AddAudio (new Audio(row[0], row[1], int.Parse (row[2]), tags));
			}
		}

		using (PropFileReader reader = ResourceToCSVReader("Backdrops"))
		{
			CsvRow row = new CsvRow();
			while (reader.ReadRow(row))
			{
				string[] tags = row[3].Split (',');
				AddBackdrop (new Backdrop(row[0], row[1], int.Parse (row[2]), tags));
			}
		}

		using (PropFileReader reader = ResourceToCSVReader("Stations"))
		{
			CsvRow row = new CsvRow();
			while (reader.ReadRow(row))
			{
				AddStation (new Station(row[0], row[1], ""));
			}
		}
	}
}
