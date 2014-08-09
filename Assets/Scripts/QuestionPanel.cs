using UnityEngine;
using System;
using System.Collections;

public class QuestionPanel : MonoBehaviour {
	dfPanel mPanel;
	dfLabel mLabel;
	dfTextbox mTextBox;

	Action<string> mCallback;

	void Awake() {
		mPanel = gameObject.GetComponent<dfPanel>();
		mLabel = gameObject.GetComponentInChildren<dfLabel>();
		mTextBox = gameObject.GetComponentInChildren<dfTextbox>();
		mPanel.enabled = false;

		DontDestroyOnLoad (gameObject);
	}

	/**
	 * Is the panel visible
	 */
	public bool isEnabled() {
		return mPanel.enabled;
	}

	public void AskQuestion(string question, Action<string> callback, string defaultText = "") {
		mLabel.Text = question;
		mTextBox.Text = defaultText;
		mCallback = callback;

		mPanel.enabled = true;
		mTextBox.Focus();
	}

	public void Submit() {
		mPanel.enabled = false;
		mCallback(mTextBox.Text);
	}

	public void EndQuestion() {
		Submit ();
	}
}
