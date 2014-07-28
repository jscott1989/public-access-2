using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class EndOfGameManager : SceneManager {

	Player[] mOverallScores;
	Player[] mCreatorScores;
	Player[] mWatcherScores;

	public string uSpecial1Category = "";
	public string uSpecial2Category = "";
	Player mSpecial1Winner;
	Player mSpecial2Winner;
	string mSpecial1Tag;
	string mSpecial2Tag;

	NetworkManager mNetworkManager;

	void Awake() {
		mNetworkManager = FindObjectOfType<NetworkManager>();
	}

	void Start() {
		// First figure out the overall winners
		mOverallScores = mNetworkManager.players.OrderBy (p => p.uOverallScore).Reverse().ToArray();
		mCreatorScores = mNetworkManager.players.OrderBy (p => p.uCreatorScore).Reverse().ToArray();
		mWatcherScores = mNetworkManager.players.OrderBy (p => p.uWatcherScore).Reverse().ToArray();
		// Then try to ensure that everyone wins /something/

		// Get a list of users ordered by least wins (so that we can try to give the special awards to those who haven't got other awards)
		Dictionary<Player, int> playerByWins = new Dictionary<Player, int>();
		foreach(Player p in mOverallScores) {
			int score = 0;
			if (p == mOverallScores[0]) score += 3;
			if (p == mOverallScores[1]) score += 2;
			if (p == mOverallScores[2]) score += 1;
			if (p == mCreatorScores[0]) score += 2;
			if (p == mWatcherScores[0]) score += 2;
			playerByWins[p] = score;
		}

		Player[] specialWinners = playerByWins.OrderBy (kvp => kvp.Value).Select (kvp => kvp.Key).ToArray ();


		// TODO: Make this work - for now we just hardcode some stuff
		uSpecial1Category = "Most Romantic Show";
		uSpecial2Category = "Most Scary Show";
		mSpecial1Winner = specialWinners[0];
		mSpecial2Winner = specialWinners[1];
		mSpecial1Tag = "romantic";
		mSpecial2Tag = "scary";

		// Choose a tag which specialWinners[0] is best at

		// Choose a tag which specialWinners[1] is best at

	}

	public string uWinnerName {
		get {
			if (mOverallScores == null) {
				return "";
			}
			return mOverallScores[0].uName;
		}
	}

	public string uWinnerPoints {
		get {
			if (mOverallScores == null) {
				return "";
			}
			return mOverallScores[0].uOverallScore.ToString() + " points";
		}
	}

	public Texture uWinnerTexture {
		get {
			if (mOverallScores == null) {
				return null;
			}
			return mOverallScores[0].uStationLogo;
		}
	}

	public string uSecondName {
		get {
			if (mOverallScores == null || mOverallScores.Length < 2) {
				return "";
			}
			return mOverallScores[1].uName;
		}
	}
	
	public string uSecondPoints {
		get {
			if (mOverallScores == null || mOverallScores.Length < 2) {
				return "";
			}
			return mOverallScores[1].uOverallScore.ToString() + " points";
		}
	}
	
	public Texture uSecondTexture {
		get {
			if (mOverallScores == null || mOverallScores.Length < 2) {
				return null;
			}
			return mOverallScores[1].uStationLogo;
		}
	}

	public string uThirdName {
		get {
			if (mOverallScores == null || mOverallScores.Length < 3) {
				return "";
			}
			return mOverallScores[2].uName;
		}
	}
	
	public string uThirdPoints {
		get {
			if (mOverallScores == null || mOverallScores.Length < 3) {
				return "";
			}
			return mOverallScores[2].uOverallScore.ToString() + " points";
		}
	}
	
	public Texture uThirdTexture {
		get {
			if (mOverallScores == null || mOverallScores.Length < 3) {
				return null;
			}
			return mOverallScores[2].uStationLogo;
		}
	}

	public string uBestShowName {
		get {
			if (mCreatorScores == null) {
				return "";
			}
			return mCreatorScores[0].uName;
		}
	}
	
	public string uBestShowPoints {
		get {
			if (mCreatorScores == null) {
				return "";
			}
			return mCreatorScores[0].uCreatorScore.ToString() + " points";
		}
	}
	
	public Texture uBestShowTexture {
		get {
			if (mCreatorScores == null) {
				return null;
			}
			return mCreatorScores[0].uStationLogo;
		}
	}

	public string uBestWatcherName {
		get {
			if (mWatcherScores == null) {
				return "";
			}
			return mWatcherScores[0].uName;
		}
	}
	
	public string uBestWatcherPoints {
		get {
			if (mWatcherScores == null) {
				return "";
			}
			return mWatcherScores[0].uWatcherScore.ToString () + " points";
		}
	}
	
	public Texture uBestWatcherTexture {
		get {
			if (mWatcherScores == null) {
				return null;
			}
			return mWatcherScores[0].uStationLogo;
		}
	}

	public string uSpecial1Name {
		get {
			if (mSpecial1Winner == null) {
				return "";
			}
			return mSpecial1Winner.uName;
		}
	}
	
	public string uSpecial1Points {
		get {
			if (mSpecial1Winner == null) {
				return "";
			}
			if (mSpecial1Winner.uSpecialScores.ContainsKey(mSpecial1Tag)) {
				return mSpecial1Winner.uSpecialScores[mSpecial1Tag].ToString () + " points";
			}
			return "";
		}
	}
	
	public Texture uSpecial1Texture {
		get {
			if (mSpecial1Winner == null) {
				return null;
			}
			return mSpecial1Winner.uStationLogo;
		}
	}

	public string uSpecial2Name {
		get {
			if (mSpecial2Winner == null) {
				return "";
			}
			return mSpecial2Winner.uName;
		}
	}
	
	public string uSpecial2Points {
		get {
			if (mSpecial2Winner == null) {
				return "";
			}
			if (mSpecial2Winner.uSpecialScores.ContainsKey (mSpecial2Tag)) {
				return mSpecial2Winner.uSpecialScores[mSpecial2Tag].ToString () + " points";
			}
			return "";
		}
	}
	
	public Texture uSpecial2Texture {
		get {
			if (mSpecial2Winner == null) {
				return null;
			}
			return mSpecial2Winner.uStationLogo;
		}
	}

	public void Quit() {
		mNetworkManager.ReturnToMainMenu();
	}
}
