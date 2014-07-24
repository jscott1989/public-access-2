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
	public List<Station> uStations = new List<Station>();
	public Dictionary<string, Station> uStationsByID = new Dictionary<string, Station>();
	public List<string> uTags = new List<string>();

	// This means that a single "ready" is enough to move everyone on - just to hurry during testing
	public const bool DEBUG_MODE = false;

	public const int LOBBY_COUNTDOWN = 6;
	public const int PROP_SELECTION_COUNTDOWN = 60; // 60
	public const int PREPARING_COUNTDOWN = 5; // 30
	public const int RECORDING_COUNTDOWN = 30; // 30
	public const int EVENING_PREPARING_COUNTDOWN = 5;
	public const int FEEDBACK_COUNTDOWN = 60; // 60
	public const int NUMBER_OF_DAYS = 5; // 5
	public const int CHANNEL_INFORMATION_COUNTDOWN = 2;

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

	void Start() {
		//Reading prop data CSV file containing list of available props for theme generation
		using (PropFileReader reader = new PropFileReader("Assets/PropSelection.csv"))
		{
			CsvRow row = new CsvRow();
			bool hasReadFirst = false;
			while (reader.ReadRow(row))
			{
				if (!hasReadFirst) {
					hasReadFirst = true;
					continue;
				}
				string[] tags = row[3].Split (',');
				foreach(string tag in tags) {
					if (!uTags.Contains (tag)) {
						uTags.Add (tag);
					}
				}
				AddProp (new Prop(row[0], row[1], int.Parse(row[2]), tags));
			}
		}


		AddBackdrop (new Backdrop("mars", "Mars", 50, new string[]{}));
		AddBackdrop (new Backdrop("beach", "Beach", 50, new string[]{}));

		AddAudio (new Audio("craw", "Craw", 49, new string[]{}));
		AddAudio (new Audio("laugh", "Bark", 51, new string[]{}));

		// Set up stations
		AddStation(new Station("random", "Random", ""));
		AddStation(new Station("channel1", "Channel 1", "Channel 1 Desc"));
		AddStation(new Station("channel2", "Channel 2", "Channel 2 Desc"));
		AddStation(new Station("channel3", "Channel 3", "Channel 3 Desc"));
		AddStation(new Station("channel4", "Channel 4", "Channel 4 Desc"));
		AddStation(new Station("channel5", "Channel 5", "Channel 5 Desc"));
		AddStation(new Station("channel6", "Channel 6", "Channel 6 Desc"));
		AddStation(new Station("channel7", "Channel 7", "Channel 7 Desc"));
		AddStation(new Station("channel8", "Channel 8", "Channel 8 Desc"));
		AddStation(new Station("channel9", "Channel 9", "Channel 9 Desc"));
		AddStation(new Station("channel10", "Channel 10", "Channel 10 Desc"));
	}
}
