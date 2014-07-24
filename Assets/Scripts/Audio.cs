using System;
using UnityEngine;
/**
 * This represents a backdrop which can exist in the system
 */
public class Audio : Prop {
	public Audio (string pID, string pName, int pPrice, string[] pTags) : base(pID, pName, pPrice, pTags) {
		
	}
}

public class PurchasedAudio : PurchasedProp {
	public PurchasedAudio (Audio pAudio) : base(pAudio) {
		
	}

	public override Texture2D uIconTexture {
		get {
			return (Texture2D)Resources.Load ("Props/soundeffect");
		}
	}
}
