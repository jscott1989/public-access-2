using System;
/**
 * This represents a backdrop which can exist in the system
 */
public class Backdrop {
	public string uID;
	public string uName;
	public int uPrice;
	public string[] uTags;
	
	public Backdrop (string pID, string pName, int pPrice, string[] pTags) {
		uID = pID;
		uName = pName;
		uPrice = pPrice;
		uTags = pTags;
	}
}

public class PurchasedBackdrop {
	public string uID;
	public Backdrop uBackdrop;
	
	public PurchasedBackdrop (Backdrop pBackdrop) {
		uID = Guid.NewGuid().ToString();
		uBackdrop = pBackdrop;
	}
}
