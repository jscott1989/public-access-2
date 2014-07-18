using UnityEngine;
using System;

/**
 * A selectable station
 */
public class Station {
	public string uID;
	public string uName;
	public string uDescription;
	public Texture2D uLogo;
	
	public Station (string pID, string pName, string pDescription) {
		uID = pID;
		uName = pName;
		uDescription = pDescription;
		uLogo = (Texture2D) Resources.Load ("Images/Stations/" + uID);
	}
}