using UnityEngine;
using System.Collections;

public class RotatableProp : MonoBehaviour {
	bool mIsDragging = false;

	Vector2 mOriginalPosition;
	float mOriginalRotation;

	dfTextureSprite mSprite;

	void Awake() {
		mSprite = GetComponentInParent<dfTextureSprite>();
	}

	void OnMouseDown() {
		mOriginalPosition = mSprite.GetManager ().ScreenToGui(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
		mOriginalRotation = mSprite.transform.eulerAngles.z;
		mIsDragging = true;
	}

	void Update() {
		if (mIsDragging) {
			Vector2 position = mSprite.GetManager ().ScreenToGui(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
			float zRotation =  mOriginalRotation + (mOriginalPosition.x - position.x) + (mOriginalPosition.y - position.y);
			print(zRotation);
			mSprite.transform.eulerAngles = new Vector3(0,0, zRotation);

			if (!Input.GetMouseButton(0)) {
				mIsDragging = false;
			}
		}
	}
}
