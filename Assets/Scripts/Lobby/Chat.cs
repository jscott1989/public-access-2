using UnityEngine;

[RequireComponent(typeof(UIInput))]

public class Chat : MonoBehaviour
{
	public UITextList textList;

	UIInput mInput;
	
	void Start ()
	{
		mInput = GetComponent<UIInput>();
		

	}

	void AddMessage(string message)
	{
		textList.Add (message);

	}

	
	void OnSubmit ()
	{
		if (textList != null)
		{
			// It's a good idea to strip out all symbols as we don't want user input to alter colors, add new lines, etc
			string text = NGUITools.StripSymbols(mInput.text);
			
			if (!string.IsNullOrEmpty(text))
			{
				mInput.text = "";

			}
		}

	}
}