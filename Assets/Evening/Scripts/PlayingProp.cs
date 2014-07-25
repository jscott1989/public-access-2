using UnityEngine;
using System.Collections;

public class PlayingProp : MonoBehaviour {
	
	public string uID;
	public Prop uProp;

	public string[] uTags {
		get {
			if (uProp == null) {
				return new string[0];
			}
			return uProp.uTags;
		}
	}
}
