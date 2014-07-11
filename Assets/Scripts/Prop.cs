using System;
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

