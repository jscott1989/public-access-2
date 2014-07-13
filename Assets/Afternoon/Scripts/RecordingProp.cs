using UnityEngine;
using System.Collections;

public class RecordingProp : MonoBehaviour {

	public PurchasedProp uPurchasedProp;
	Props mProps;

	void Awake() {
		mProps = (Props)FindObjectOfType(typeof(Props));
	}

	/**
	 * Put the prop back in the prop box
	 */
	public void PutBackInBox() {
		mProps.Add (uPurchasedProp);
		Destroy (gameObject);
	}
}
