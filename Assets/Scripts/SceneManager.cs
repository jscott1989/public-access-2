using UnityEngine;
using System.Collections;

/**
 * This class should be extended by the scene managers so they can deal with
 * global events (e.g. network events)
 */
public abstract class SceneManager : MonoBehaviour {
	public virtual void PlayerConnected(int pID, NetworkPlayer pPlayer) {}
	public virtual void PlayerDisconnected(int pID, NetworkPlayer pPlayer) {
		Player player = FindObjectOfType<NetworkManager>().GetPlayerWithID(pID);

		// We now mark the player as disconnected and inform all players
		player.networkView.RPC ("HasDisconnected",RPCMode.All);
	}

	public virtual void NewPlayer(Player pPlayer) {}
	public virtual void PlayerLeaves(Player pPlayer) {}
	public virtual void ReadyStatusChanged(Player pPlayer) {}

	public virtual void PropPurchased(Player pPlayer, PurchasedProp pPurchasedProp) {}
	public virtual void PropSold(Player pPlayer, PurchasedProp pPurchasedProp) {}

	public virtual void AudioPlayed(Audio pAudio) {}
}
