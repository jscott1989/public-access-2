using UnityEngine;

[RequireComponent(typeof(UIInput))]

/**
 * The input for the lobby chat
 */
public class Chat : MonoBehaviour
{
	// The Chatlist to add messages to
	public ChatTextList textList;

	NetworkManager networkManager;

	// The text input
	UIInput mInput;
	
	void Start ()
	{
		networkManager = (NetworkManager) GameObject.FindGameObjectWithTag ("NetworkManager").GetComponent ("NetworkManager");
		mInput = GetComponent<UIInput>();
	}

	void AddMessage(string message)
	{
		textList.Add (message);
	}

	void Update() {
			if (mInput != null) {
				mInput.selected = true;
			}
		}
	
	void OnSubmit ()
	{
		if (textList != null)
		{
			// It's a good idea to strip out all symbols as we don't want user input to alter colors, add new lines, etc
			string text = NGUITools.StripSymbols(mInput.text);
			Player myPlayer = networkManager.GetMyPlayer ();
			
			if (!string.IsNullOrEmpty(text))
			{
				AddMessage("<" + myPlayer.player_name + "> " + text);
				mInput.text = "";
			}
		}
	}
}