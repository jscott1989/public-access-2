using UnityEngine;
using System.Collections;

public class RefreshList : MonoBehaviour {

	public NetworkManager networkManager ;

	void OnClick()
	{
			networkManager.RefreshHostList();
		

	}
}
