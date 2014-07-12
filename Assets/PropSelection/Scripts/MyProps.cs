using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyProps : MonoBehaviour {
	int selectedIndex;

	dfScrollPanel mMyPropsList;
	GameObject mPropSelectionPurchasedPropPrefab;

	Dictionary<PurchasedProp, PropSelectionPurchasedProp> mPurchasedPropObjects = new Dictionary<PurchasedProp, PropSelectionPurchasedProp>();

	public PurchasedProp uSelectedPurchasedProp = null;

	void Awake() {
		mMyPropsList = (dfScrollPanel)FindObjectOfType(typeof(dfScrollPanel));
		mPropSelectionPurchasedPropPrefab = (GameObject)Resources.Load ("PropSelection/Prefabs/PropSelectionPurchasedProp");
	}

	public void Add(PurchasedProp pPurchasedProp) {
		GameObject propSelectionPurchasedProp = (GameObject) Instantiate(mPropSelectionPurchasedPropPrefab, Vector3.zero, Quaternion.identity);
		PropSelectionPurchasedProp p = (PropSelectionPurchasedProp)propSelectionPurchasedProp.GetComponent (typeof(PropSelectionPurchasedProp));
		p.uPurchasedProp = pPurchasedProp;
		propSelectionPurchasedProp.transform.parent = mMyPropsList.transform;
		mPurchasedPropObjects.Add (pPurchasedProp, p);
	}

	public void Remove(PurchasedProp pPurchasedProp) {
		Destroy(mPurchasedPropObjects[pPurchasedProp].gameObject);
		mPurchasedPropObjects.Remove(pPurchasedProp);

		if (uSelectedPurchasedProp != null && uSelectedPurchasedProp.uID == pPurchasedProp.uID) {
			uSelectedPurchasedProp = null;
		}
	}

	public void Select(PurchasedProp pPurchasedProp) {
		foreach(KeyValuePair<PurchasedProp, PropSelectionPurchasedProp> kv in mPurchasedPropObjects) {
			if (kv.Value.uPurchasedProp.uID == pPurchasedProp.uID) {
				kv.Value.uIsSelected = true;
			} else {
				kv.Value.uIsSelected = false;
			}
		}

		uSelectedPurchasedProp = pPurchasedProp;
	}
}
