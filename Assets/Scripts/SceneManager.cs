using UnityEngine;
using System.Collections;

/**
 * This class should be extended by the scene managers so they can deal with
 * global events (e.g. network events)
 */
public abstract class SceneManager : MonoBehaviour {
	public virtual void PlayerConnected(int pID, NetworkPlayer pPlayer) {}
	public virtual void NewPlayer(Player pPlayer) {}
}
