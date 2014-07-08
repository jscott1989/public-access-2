using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	private const string typeName = "PublicAccessWars";
	public GameObject playerPrefab;
	
	public void StartServer(string gameName)
	{
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}

	public HostData[] hostList;
	
	public void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);

		//print ("Refreshed");
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived) {
			hostList = MasterServer.PollHostList ();
			print (hostList[0]);
		}

	}
	public void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}
	
	void OnConnectedToServer()
	{
		Debug.Log("Server Joined");
		SpawnPlayer();
	}

	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");
		SpawnPlayer();
	}
	
	private void SpawnPlayer()
	{
		Network.Instantiate(playerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
	}

	/**
	 * Get the player object of the current network client
	 */
	public Player GetMyPlayer() {
		foreach (GameObject p in GameObject.FindGameObjectsWithTag ("Player")) {
			if (p.networkView.isMine) {
				return (Player) p.GetComponent ("Player");
			}
		}
		return null;
	}

}
