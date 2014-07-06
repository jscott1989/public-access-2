using UnityEngine;
using System.Collections;

public class MoveToScene : MonoBehaviour {

	public string sceneName;

	void OnClick() {
		Application.LoadLevel (sceneName);
	}
}
