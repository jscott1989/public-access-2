using System;
/**
 * This represents a backdrop which can exist in the system
 */
public class Backdrop : Prop {
	public Backdrop (string pID, string pName, int pPrice, string[] pTags) : base(pID, pName, pPrice, pTags) {

	}
}

public class PurchasedBackdrop : PurchasedProp {
	public PurchasedBackdrop (Backdrop pBackdrop) : base(pBackdrop) {

	}
}
