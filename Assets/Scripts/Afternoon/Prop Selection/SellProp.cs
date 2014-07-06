using UnityEngine;
using System.Collections;

public class SellProp : MonoBehaviour {
	public PropSelectionManager propSelectionManager;

	void OnClick() {
		propSelectionManager.SellProp();
	}
}
