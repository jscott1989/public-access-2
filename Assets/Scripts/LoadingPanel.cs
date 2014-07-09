using UnityEngine;
using System.Collections;

/**
 * This allows us to overlay a panel on the scene to indicate the game is busy
 */
public class LoadingPanel : MonoBehaviour {

	dfPanel mPanel;
	dfLabel mAlertText;

	void Awake () {
		mPanel = (dfPanel)gameObject.GetComponent (typeof(dfPanel));
		mAlertText = (dfLabel)gameObject.GetComponentInChildren (typeof(dfLabel));
		mPanel.enabled = false;

		/**
		 * Persist the LoadingPanel between scenes
		 */
		DontDestroyOnLoad (gameObject);
	}

	public void ShowAlert(string pAlertText) {
		mAlertText.Text = pAlertText;
		mPanel.enabled = true;
	}

	public void HideAlert() {
		mPanel.enabled = false;
	}
}
