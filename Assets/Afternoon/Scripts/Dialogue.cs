using UnityEngine;
using System.Collections;
using System;

public class Dialogue : MonoBehaviour {
	const string DEFAULT_TEXT = "Click here to set text";
	QuestionPanel mQuestionPanel;
	dfSlicedSprite mSprite;
	dfLabel mLabel;
	DialogueSizeTester mDialogueSizeTester;

	void Awake() {
		mQuestionPanel = FindObjectOfType<QuestionPanel>();
		mSprite = FindObjectOfType<dfSlicedSprite>();
		mLabel = GetComponentInChildren<dfLabel>();
		mDialogueSizeTester = FindObjectOfType<DialogueSizeTester>();

		mLabel.Text = DEFAULT_TEXT;
		ScaleText ();
	}

	public void SetText() {
		Action<string> textSelected =
		(string pText) => {
			mLabel.Text = pText;
			ScaleText();
		};

		if (mLabel.Text != DEFAULT_TEXT) {
			mQuestionPanel.AskQuestion ("Enter text", textSelected, mLabel.Text);
		} else {
			mQuestionPanel.AskQuestion ("Enter text", textSelected);
		}
	}

	public void SizeChanged() {
		ScaleText ();
	}

	void ScaleText() {
		mLabel.TextScale = mDialogueSizeTester.GetTextScale(mLabel.Width, mLabel.Height, mLabel.Text);
	}
}
