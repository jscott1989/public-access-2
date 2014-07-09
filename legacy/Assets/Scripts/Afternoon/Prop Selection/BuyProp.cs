using UnityEngine;
using System.Collections;

public class BuyProp : MonoBehaviour {

	public PropSelectionManager propSelectionManager;

	void OnClick() {
		propSelectionManager.PurchaseProp();
	}
}
