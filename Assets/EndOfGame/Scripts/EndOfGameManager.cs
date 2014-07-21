using UnityEngine;
using System.Linq;
using System.Collections;

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
		mOverallScores = mNetworkManager.players.OrderBy (p => p.uOverallScore).ToArray();
		mCreatorScores = mNetworkManager.players.OrderBy (p => p.uCreatorScore).ToArray();
		mWatcherScores = mNetworkManager.players.OrderBy (p => p.uWatcherScore).ToArray();
		// Then try to ensure that everyone wins /something/

		// TODO: Make this work - for now we just hardcode some stuff
		uSpecial1Category = "Most Romantic Show";
		uSpecial2Category = "Most Scary Show";
		mSpecial1Winner = mOverallScores[0];
		mSpecial2Winner = mOverallScores[0];
		mSpecial1Tag = "romantic";
		mSpecial2Tag = "scary";

	}

	public string uWinnerName {
		get {
			return mOverallScores[0].uName;
		}
	}

	public int uWinnerPoints {
		get {
			return mOverallScores[0].uOverallScore;
		}
	}

	public Texture uWinnerTexture {
		get {
			return mOverallScores[0].uStationLogo;
		}
	}

	public string uSecondName {
		get {
			if (mOverallScores.Length < 2) {
				return "";
			}
			return mOverallScores[1].uName;
		}
	}
	
	public int uSecondPoints {
		get {
			if (mOverallScores.Length < 2) {
				return 0;
			}
			return mOverallScores[1].uOverallScore;
		}
	}
	
	public Texture uSecondTexture {
		get {
			if (mOverallScores.Length < 2) {
				return null;
			}
			return mOverallScores[1].uStationLogo;
		}
	}

	public string uThirdName {
		get {
			if (mOverallScores.Length < 3) {
				return "";
			}
			return mOverallScores[2].uName;
		}
	}
	
	public int uThirdPoints {
		get {
			if (mOverallScores.Length < 3) {
				return 0;
			}
			return mOverallScores[2].uOverallScore;
		}
	}
	
	public Texture uThirdTexture {
		get {
			if (mOverallScores.Length < 3) {
				return null;
			}
			return mOverallScores[2].uStationLogo;
		}
	}

	public string uBestShowName {
		get {
			return mCreatorScores[0].uName;
		}
	}
	
	public int uBestShowPoints {
		get {
			return mCreatorScores[0].uCreatorScore;
		}
	}
	
	public Texture uBestShowTexture {
		get {
			return mCreatorScores[0].uStationLogo;
		}
	}

	public string uBestWatcherName {
		get {
			return mWatcherScores[0].uName;
		}
	}
	
	public int uBestWatcherPoints {
		get {
			return mWatcherScores[0].uWatcherScore;
		}
	}
	
	public Texture uBestWatcherTexture {
		get {
			return mWatcherScores[0].uStationLogo;
		}
	}

	public string uSpecial1Name {
		get {
			return mSpecial1Winner.uName;
		}
	}
	
	public int uSpecial1Points {
		get {
			if (mSpecial1Winner.uSpecialScores.ContainsKey(mSpecial1Tag)) {
				return mSpecial1Winner.uSpecialScores[mSpecial1Tag];
			}
			return 0;
		}
	}
	
	public Texture uSpecial1Texture {
		get {
			return mSpecial1Winner.uStationLogo;
		}
	}

	public string uSpecial2Name {
		get {
			return mSpecial2Winner.uName;
		}
	}
	
	public int uSpecial2Points {
		get {
			if (mSpecial2Winner.uSpecialScores.ContainsKey (mSpecial2Tag)) {
				return mSpecial2Winner.uSpecialScores[mSpecial2Tag];
			}
			return 0;
		}
	}
	
	public Texture uSpecial2Texture {
		get {
			return mSpecial2Winner.uStationLogo;
		}
	}

	public void Quit() {
		mNetworkManager.ReturnToMainMenu();
	}
}
