using UnityEngine;
using System.Collections;

public class RecordingProp : MonoBehaviour {

	public PurchasedProp uPurchasedProp;
	Props mProps;

	public bool uIsSelected = false;

	void Awake() {
		mProps = FindObjectOfType<Props>();
	}

	public void Select() {
		foreach(RecordingProp p in FindObjectsOfType<RecordingProp>()) {
			if (p != this) {
				p.uIsSelected = false;
			}
		}
		uIsSelected = true;
	}

	/**
	 * Put the prop back in the prop box
	 */
	public virtual void PutBackInBox() {
		mProps.Add (uPurchasedProp);
		Destroy (gameObject);
	}

	public virtual void FirstDrop() {
	}
}
