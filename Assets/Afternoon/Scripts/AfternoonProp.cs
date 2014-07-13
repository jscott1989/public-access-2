using UnityEngine;
using System.Collections;

public class AfternoonProp : MonoBehaviour {
	public PurchasedProp uPurchasedProp;
	public dfTextureSprite uImage;
	GameObject mScreen;

	GameObject mRecordingPropPrefab;
	
	Props mProps;
	
	void Awake() {
		mProps = (Props) FindObjectOfType(typeof(Props));
		uImage = (dfTextureSprite) GetComponentInChildren(typeof(dfTextureSprite));
		mRecordingPropPrefab = (GameObject)Resources.Load ("Afternoon/Prefabs/RecordingProp");
		mScreen = GameObject.FindGameObjectWithTag("Screen");
	}

	void OnMouseDown() {
		GameObject g = (GameObject) Instantiate(mRecordingPropPrefab, Vector3.zero, Quaternion.identity);
		g.transform.parent = mScreen.transform;

		Vector2 mousePosition = uImage.GetManager ().ScreenToGui(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

		dfTextureSprite sprite = (dfTextureSprite)g.GetComponent (typeof(dfTextureSprite));
		// TODO: these numbers are hardcoded - copied from the unity gui - figure out how to get this programatically
		Vector2 top_left = new Vector2(mousePosition.x - 183, (0-mousePosition.y) + 123);

		sprite.Position = new Vector2(top_left.x - (sprite.Size.x / 2), top_left.y + (sprite.Size.y / 2));

		dfTextureSprite t = (dfTextureSprite)g.GetComponent (typeof(dfTextureSprite));
		t.Texture = (Texture2D)Resources.Load("Props/" + uPurchasedProp.uProp.uID);

		RecordingProp r = (RecordingProp)g.GetComponent (typeof(RecordingProp));
		r.uPurchasedProp = uPurchasedProp;

		MovableProp p = (MovableProp)g.GetComponent(typeof(MovableProp));
		p.OnMouseDown();

		mProps.Remove (uPurchasedProp);
	}
}