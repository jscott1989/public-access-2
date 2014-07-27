using UnityEngine;
using System.Collections;

/**
 * This class should be extended by the scene managers so they can deal with
 * global events (e.g. network events)
 */
public abstract class SceneManager : MonoBehaviour {
	public virtual void PlayerConnected(int pID, NetworkPlayer pPlayer) {}
	public virtual void PlayerDisconnected(int pID, NetworkPlayer pPlayer) {}
	public virtual void NewPlayer(Player pPlayer) {}
	public virtual void PlayerLeaves(Player pPlayer) {}
	public virtual void ReadyStatusChanged(Player pPlayer) {}

	public virtual void PropPurchased(Player pPlayer, PurchasedProp pPurchasedProp) {}
	public virtual void PropSold(Player pPlayer, PurchasedProp pPurchasedProp) {}

	public virtual void AudioPlayed(Audio pAudio) {}
}
