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

	public void AskQuestion(string question, Action<string> callback) {
		mLabel.Text = question;
		mTextBox.Text = "";
		mCallback = callback;

		mPanel.enabled = true;
	}

	public void Submit() {
		mPanel.enabled = false;
		mCallback(mTextBox.Text);
	}
}
