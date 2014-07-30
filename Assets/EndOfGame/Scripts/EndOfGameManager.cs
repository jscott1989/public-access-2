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

	dfTweenVector3 mOverallDetailTween;
	dfTweenVector3 mDetailOverallTween;

	dfScrollPanel mFinalPlayerScores;
	GameObject mFinalPlayerScorePrefab;

	NetworkManager mNetworkManager;
	Game mGame;

	void Awake() {
		mNetworkManager = FindObjectOfType<NetworkManager>();
		mGame = FindObjectOfType<Game>();
		mFinalPlayerScores = GameObject.Find ("FinalPlayerScores").GetComponent<dfScrollPanel>();
		mFinalPlayerScorePrefab = (GameObject)Resources.Load ("EndOfGame/Prefabs/FinalPlayerScore");
		GameObject container = GameObject.Find ("Container");
		dfTweenVector3[] tweens = container.GetComponents<dfTweenVector3>();
		mOverallDetailTween = tweens[0];
		mDetailOverallTween = tweens[1];
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
			if (mOverallScores.Length > 1 && p == mOverallScores[1]) score += 2;
			if (mOverallScores.Length > 2 && p == mOverallScores[2]) score += 1;
			if (p == mCreatorScores[0]) score += 2;
			if (p == mWatcherScores[0]) score += 2;
			playerByWins[p] = score;
		}

		Player[] specialWinners = playerByWins.OrderBy (kvp => kvp.Value).Select (kvp => kvp.Key).ToArray ();
		mSpecial1Winner = specialWinners[0];
		if (specialWinners.Length > 1) {
			mSpecial2Winner = specialWinners[1];
		} else {
			mSpecial2Winner = specialWinners[0];
		}

		Dictionary<string, List<Player>> playersOrderedByTagScore = new Dictionary<string, List<Player>>();
		Dictionary<string, List<Player>> playersOrderedByTagLostScore = new Dictionary<string, List<Player>>();
		foreach(string t in mGame.uTags) {
			playersOrderedByTagScore[t] = mNetworkManager.players.OrderByDescending (p => p.uScoreFromWatching.ContainsKey(t) ? p.uScoreFromWatching[t] : 0).ToList ();
			playersOrderedByTagLostScore[t] = mNetworkManager.players.OrderByDescending (p => p.uScoreLostFromWatching.ContainsKey(t) ? p.uScoreLostFromWatching[t] : 0).ToList ();
		}

		// TODO: This doesn't /actually/ give us the winner of these categories - but it gives us something cute to talk about and I have
		// no time to make it work correctly.

		mSpecial1Tag = playersOrderedByTagScore.OrderBy (kvp => kvp.Value.IndexOf (mSpecial1Winner)).Select (kvp => kvp.Key).First ();
		mSpecial2Tag = playersOrderedByTagLostScore.OrderBy (kvp => kvp.Value.IndexOf (mSpecial2Winner)).Select (kvp => kvp.Key).First ();

		uSpecial1Category = "Biggest fan of " + mGame.uTagHumanReadable[mSpecial1Tag];
		uSpecial2Category = "Hates " + mGame.uTagHumanReadable[mSpecial2Tag];


		// Now set up the FinalPlayerScores box

		foreach(Player p in mOverallScores) {
			GameObject f = (GameObject)Instantiate (mFinalPlayerScorePrefab, Vector3.zero, Quaternion.identity);
			f.transform.Find ("PlayerName").GetComponent<dfLabel>().Text = p.uName;
			f.transform.Find ("ShowTitle").GetComponent<dfLabel>().Text = p.uShowName;
			f.transform.Find ("ChannelLogo").GetComponent<dfTextureSprite>().Texture = p.uStationLogo;

			for(int i = 0; i < Game.NUMBER_OF_DAYS; i++) {
				Transform day = f.transform.Find ("Day " + (i + 1));
				day.Find ("Like").GetComponent<dfLabel>().Text = p.uFormattedDailyNeed(i);
				int score = p.uDailyCreatorScore[i] + p.uDailyWatchingScore[i];
				day.Find ("Score").GetComponent<dfLabel>().Text = score + " points";
			}

			f.transform.parent = mFinalPlayerScores.transform;
		}
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
			if (mSpecial1Winner == null || mSpecial1Tag == null) {
				return "";
			}

			if (mSpecial1Winner.uScoreFromWatching.ContainsKey(mSpecial1Tag)) {
				return mSpecial1Winner.uScoreFromWatching[mSpecial1Tag].ToString () + " points";
			}
			return "0 points";
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
			if (mSpecial2Winner == null || mSpecial2Tag == null) {
				return "";
			}

			if (mSpecial2Winner.uScoreLostFromWatching.ContainsKey (mSpecial2Tag)) {
				return mSpecial2Winner.uScoreLostFromWatching[mSpecial2Tag].ToString () + " points lost";
			}
			return "0 points lost";
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

	public void ShowDetail() {
		mOverallDetailTween.Play ();
	}

	public void ShowOverall() {
		mDetailOverallTween.Play ();
	}
}
