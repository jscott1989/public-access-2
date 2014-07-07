using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	private const string typeName = "PublicAccessWars";
	
	public void StartServer(string gameName)
	{
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}
	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");

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

		//print ("Refresh 2");

	}
	public void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}
	
	void OnConnectedToServer()
	{
		Debug.Log("Server Joined");
	}


}
