using UnityEngine;
using System.Collections;

public class WatchedStationAction {
	public Player uPlayer;
	public float uStartTime;
	public float uEndTime;

	public WatchedStationAction(Player pPlayer, float pStartTime, float pEndTime = -1) {
		uPlayer = pPlayer;
		uStartTime = pStartTime;
		uEndTime = pEndTime;
	}
}
