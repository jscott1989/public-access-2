using System;
using UnityEngine;
/**
 * This represents a prop which can exist in the system
 */
public class Prop {
	public string uID;
	public string uName;
	public int uPrice;
	public string[] uTags;

	public Prop (string pID, string pName, int pPrice, string[] pTags) {
		uID = pID;
		uName = pName;
		uPrice = pPrice;
		uTags = pTags;
	}
}

public class PurchasedProp {
	public string uID;
	public Prop uProp;

	public PurchasedProp (Prop pProp) {
		uID = Guid.NewGuid().ToString();
		uProp = pProp;
	}

	public virtual Texture2D uIconTexture {
		get {
			return (Texture2D)Resources.Load ("Props/" + uProp.uID);
		}
	}
}
