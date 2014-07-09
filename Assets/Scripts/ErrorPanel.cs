using UnityEngine;
using System;
using System.Collections;

/**
 * This allows us to overlay a panel on the scene to indicate an error
 */
public class ErrorPanel : MonoBehaviour {
	
	dfPanel mPanel;
	dfLabel mErrorText;
	Action mErrorCallback = null;
	
	void Awake () {
		mPanel = (dfPanel)gameObject.GetComponent (typeof(dfPanel));
		mErrorText = (dfLabel)gameObject.GetComponentInChildren (typeof(dfLabel));
		mPanel.enabled = false;
		
		/**
		 * Persist the LoadingPanel between scenes
		 */
		DontDestroyOnLoad (gameObject);
	}
	
	public void ShowError(string pErrorText, Action pErrorCallback = null) {
		mErrorText.Text = pErrorText;
		mErrorCallback = pErrorCallback;
		mPanel.enabled = true;
	}
	
	public void HideError() {
		mPanel.enabled = false;
	}

	public void OkayButtonPressed() {
		if (mErrorCallback != null) {
			mErrorCallback ();
		}
		HideError ();
	}
}
