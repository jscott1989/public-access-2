using UnityEngine;
using System.Collections;

public class ResizableCorner : MonoBehaviour {
	public Texture2D uCursorTexture;

	public ResizableCorner uOppositeCorner;
	ResizableProp mResizableProp;

	public dfButton uButton;

	void Awake() {
		uButton = GetComponent<dfButton>();
		mResizableProp = gameObject.transform.parent.parent.GetComponent<ResizableProp>();

//		dfButton myButton = gameObject.GetComponent<dfButton>();
//		// Figure out which corner we are
//		foreach(ResizableCorner c in gameObject.transform.parent.GetComponentsInChildren<ResizableCorner>()) {
//			dfButton d = c.gameObject.GetComponent<dfButton>();
//			if (d.Position.x != myButton.Position.x && d.Position.y != myButton.Position.y) {
//				// This is the opposite corner
//				uOppositeCorner = c;
//				break;
//			}
//		}
	}

	void OnMouseDown() {
		// TODO: Set ourselves as resizing - and record the "pivot point" which will have a fixed position
		mResizableProp.StartResizing(uOppositeCorner);
	}

	void OnMouseEnter() {
		print ("ENTER");
//		Cursor.SetCursor(uCursorTexture, Vector2.zero, CursorMode.Auto);
	}

	void OnMouseExit() {
		// TODO: This isn't calling
		print("EXIT");
//		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	}
}
