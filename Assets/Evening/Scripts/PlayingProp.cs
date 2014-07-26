using UnityEngine;
using System.Collections;

public class PlayingProp : MonoBehaviour {
	
	public string uID;
	public Prop uProp;

	public string[] uTags {
		get {
			if (uProp == null) {
				// Let's just assume we're a dialogue TODO: This is a stupid place to put this code - sort it out
				return new string[]{"text", "white"};
			}
			return uProp.uTags;
		}
	}
}
