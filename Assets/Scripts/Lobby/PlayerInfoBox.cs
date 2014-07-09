using UnityEngine;
using System.Collections;

public class PlayerInfoBox : MonoBehaviour {
	[RPC] public void AddPlayerInfo (string username) {
		UIGrid grid = (UIGrid) GameObject.FindGameObjectWithTag ("Player List").GetComponent (typeof(UIGrid));
		UILabel l = (UILabel) gameObject.GetComponentInChildren (typeof(UILabel));
		l.text = username;
		
		UIDragPanelContents p = (UIDragPanelContents)gameObject.GetComponent (typeof(UIDragPanelContents));
		gameObject.transform.parent = grid.transform;
		
		gameObject.transform.localScale = new Vector3 (1, 1, 1);
		
		grid.Reposition ();

		if (Network.isServer) {
			networkView.RPC ("AddPlayerInfo", RPCMode.Others, username);
		}
	}
}
