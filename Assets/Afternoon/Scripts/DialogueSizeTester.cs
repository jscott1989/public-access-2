using UnityEngine;
using System.Collections;

public class DialogueSizeTester : MonoBehaviour {

	const int MAX_TEXT_SCALE = 20;

	dfLabel mLabel;

	void Awake() {
		mLabel = GetComponent<dfLabel>();
	}

	public float GetTextScale(float pWidth, float pHeight, string pText) {
		mLabel.MaximumSize = new Vector2(pWidth, pHeight);
		mLabel.Text = pText;

		mLabel.TextScale = MAX_TEXT_SCALE;
		while (mLabel.Width > pWidth || mLabel.Height > pHeight) {
			if (mLabel.TextScale <= 0.5) {
				return 1;
			}
			mLabel.TextScale -= 0.5f;
		}

		return mLabel.TextScale;
	}
}
