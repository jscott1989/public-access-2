using UnityEngine;
using System.Collections;

/**
 * This stupidly named class is so it doesn't conflict with PurchasedProp
 * it just represents the prop on the "Purchase" screen
 */
public class PropSelectionPurchasedProp : MonoBehaviour {
	public PurchasedProp uPurchasedProp;
	public bool uIsSelected;

	public dfTextureSprite uImage;

	MyProps mMyProps;

	void Awake() {
		mMyProps = (MyProps) FindObjectOfType(typeof(MyProps));
		uImage = (dfTextureSprite) GetComponentInChildren(typeof(dfTextureSprite));
	}

	void Start() {
		// Set up the binding between the highlight and isSelected
		dfSlicedSprite highlight = (dfSlicedSprite) GetComponentInChildren(typeof(dfSlicedSprite));
		dfPropertyBinding.Bind(this, "uIsSelected", highlight, "IsVisible");
	}

	void OnClick() {
		mMyProps.Select(uPurchasedProp);
	}
}