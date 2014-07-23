using UnityEngine;
using System.Collections;

public class ResizableProp : MonoBehaviour {
	bool mIsDragging = false;
	ResizableCorner mFixedCorner;
	dfButton mFixedCornerButton;

	dfTextureSprite mSprite;

	dfPanel mScreenPanel;

	void Awake() {
		mSprite = GetComponent<dfTextureSprite>();
		mScreenPanel = GameObject.FindGameObjectWithTag("Screen").GetComponent<dfPanel>();
	}

	public void StartResizing(ResizableCorner pFixedCorner) {
		mFixedCorner = pFixedCorner;

		if (mFixedCorner.uButton.Position.x < mFixedCorner.uOppositeCorner.uButton.Position.x) {
			// We're left
			if (mFixedCorner.uButton.Position.y > mFixedCorner.uOppositeCorner.uButton.Position.y) {
				// Top Left
				mSprite.Pivot = dfPivotPoint.TopLeft;
			} else {
				// Bottom Left
				mSprite.Pivot = dfPivotPoint.BottomLeft;
			}
		} else {
			// We're right
			if (mFixedCorner.uButton.Position.y > mFixedCorner.uOppositeCorner.uButton.Position.y) {
				// Top Right
				mSprite.Pivot = dfPivotPoint.TopRight;
			} else {
				// Bottom Right
				mSprite.Pivot = dfPivotPoint.BottomRight;
			}
		}

		mFixedCornerButton = pFixedCorner.gameObject.GetComponent<dfButton>();
		mIsDragging = true;
	}

	void Update() {
		if (mIsDragging) {
			Vector2 position = mFixedCornerButton.GetManager ().ScreenToGui(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

			float fixedCornerX = mFixedCornerButton.GetAbsolutePosition().x;
			float fixedCornerY = mFixedCornerButton.GetAbsolutePosition().y;

			print(fixedCornerX);
			print (fixedCornerY);

			float width = Mathf.Abs(position.x - fixedCornerX);
			float height = Mathf.Abs(position.y - fixedCornerY);

			mSprite.Size = new Vector2(width, height);


			if (!Input.GetMouseButton(0)) {
				StopDragging();
			}
		}
	}

	void StopDragging() {
		mIsDragging = false;
		mSprite.Pivot = dfPivotPoint.MiddleCenter;
	}
}
