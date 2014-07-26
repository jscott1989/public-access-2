using UnityEngine;
using System.Collections;
using System;

public class Dialogue : MonoBehaviour {
	QuestionPanel mQuestionPanel;
	dfSlicedSprite mSprite;
	dfLabel mLabel;
	DialogueSizeTester mDialogueSizeTester;

	void Awake() {
		mQuestionPanel = FindObjectOfType<QuestionPanel>();
		mSprite = FindObjectOfType<dfSlicedSprite>();
		mLabel = GetComponentInChildren<dfLabel>();
		mDialogueSizeTester = FindObjectOfType<DialogueSizeTester>();

		mLabel.Text = "";
		ScaleText ();
	}

	public void SetText() {
		Action<string> textSelected =
		(string pText) => {
			mLabel.Text = pText;
			ScaleText();
		};

		mQuestionPanel.AskQuestion ("Enter text", textSelected, mLabel.Text);
	}

	public void SizeChanged() {
		ScaleText ();
	}

	void ScaleText() {
		mLabel.TextScale = mDialogueSizeTester.GetTextScale(mLabel.Width, mLabel.Height, mLabel.Text);
	}
}
