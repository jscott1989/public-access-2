using UnityEngine;
using System.Collections;

public class AfternoonProp : MonoBehaviour {
	public PurchasedProp uPurchasedProp;
	public dfTextureSprite uImage;
	public GameObject mScreen;

	GameObject mRecordingPropPrefab;
	
	Props mProps;
	
	void Awake() {
		mProps = (Props) FindObjectOfType(typeof(Props));
		uImage = (dfTextureSprite) GetComponentInChildren(typeof(dfTextureSprite));
		mRecordingPropPrefab = (GameObject)Resources.Load ("Afternoon/Prefabs/RecordingProp");
		mScreen = GameObject.FindGameObjectWithTag("Screen");
	}

	void OnMouseDown() {
		gameObject.transform.parent = mScreen.transform;

		GameObject g = (GameObject) Instantiate(mRecordingPropPrefab, Vector3.zero, Quaternion.identity);
		g.transform.parent = mScreen.transform;
		g.transform.position = gameObject.transform.position;

		// TODO: Set this gameObject's parent back
		mProps.Remove (uPurchasedProp);
		Destroy (gameObject); // And don't destroy it
	}
}