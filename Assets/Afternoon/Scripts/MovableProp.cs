using UnityEngine;
using System.Collections;

/**
 * A prop on the Afternoon scene which has been pulled from the prop box and is live
 * This adds the ability for it to be moved, resized, and rotated, by the player
 */
public class MovableProp : MonoBehaviour {
	bool mIsDragging = false;

	dfPanel mScreenPanel;
	dfTextureSprite mSprite;

	// The drag offset
	float mXOffset;
	float mYOffset;

	void Awake() {
		mScreenPanel = (dfPanel)GameObject.FindGameObjectWithTag("Screen").GetComponent(typeof(dfPanel));
		mSprite = (dfTextureSprite)GetComponent(typeof(dfTextureSprite));
	}

	void Update() {
		if (mIsDragging) {
			Vector2 aPosition = mScreenPanel.GetManager ().ScreenToGui(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
			aPosition = new Vector2(aPosition.x, 0 - aPosition.y);
			// TODO: these numbers are hardcoded - copied from the unity gui - figure out how to get this programatically
			Vector2 relativePosition = new Vector2(aPosition.x - 183, aPosition.y + 123);
			print(mScreenPanel.Position);
			mSprite.Position = new Vector2(relativePosition.x - mXOffset, relativePosition.y - mYOffset);

			// Left = 183
			// Top = 123
			if (!Input.GetMouseButton(0)) {
				mIsDragging = false;
			}
		}
	}

	void OnMouseDown() {
		Vector2 aPosition = mScreenPanel.GetManager ().ScreenToGui(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
		aPosition = new Vector2(aPosition.x - 183, (0 - aPosition.y) + 123);
		Vector2 bPosition = new Vector2(aPosition.x - mSprite.Position.x, aPosition.y - mSprite.Position.y);

		mXOffset = bPosition.x;
		mYOffset = bPosition.y;

		mIsDragging = true;
	}
}
