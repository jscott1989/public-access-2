using UnityEngine;
using System.Collections;

public class AfternoonBackdrop : MonoBehaviour {
	int mMode = 0; // -1 means hiding, 1 means showing, 0 means do nothign

	const float MOVEMENT_SPEED = 140;

	dfTextureSprite mSprite;

	void Awake() {
		mSprite = GetComponent<dfTextureSprite>();
	}

	public void Show() {
		mMode = 1;
	}

	public void Hide() {
		mMode = -1;
		mSprite.SendToBack();
	}

	void Update() {
		if (mMode == 1) {
			mSprite.Position = new Vector2(0, mSprite.Position.y - Time.deltaTime * MOVEMENT_SPEED);
			if (mSprite.Position.y <= 0) {
				mSprite.Position = new Vector2(0,0);
				mMode = 0;
			}
		} else if (mMode == -1) {
			mSprite.Position = new Vector2(0, mSprite.Position.y + Time.deltaTime * MOVEMENT_SPEED);
			if (mSprite.Position.y >= 550) {
				mSprite.Position = new Vector2(0,550);
				mMode = 0;
			}
		}
	}
}
