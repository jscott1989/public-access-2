using UnityEngine;
using System.Collections;

public class AfternoonProp : MonoBehaviour {
	public PurchasedProp uPurchasedProp;
	public dfTextureSprite uImage;
	
	MyProps mMyProps;
	
	void Awake() {
		mMyProps = (MyProps) FindObjectOfType(typeof(MyProps));
		uImage = (dfTextureSprite) GetComponentInChildren(typeof(dfTextureSprite));
	}
}