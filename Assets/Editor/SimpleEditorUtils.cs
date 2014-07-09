// IN YOUR EDITOR FOLDER, make a script SimpleEditorUtils.cs.
// paste in this text.
// to play, hit command-zero rather than command-P
// (the zero key, is near the P key, so it's easy to remember)
// simply insert the actual name of your opening scene
// "__preEverythingScene" on the second last line of code below.

using UnityEditor;
using UnityEngine;
using System.Collections;

[InitializeOnLoad]
public static class SimpleEditorUtils
{
	// click command-0 to go to the prelaunch scene and then play
	
	
	
	[MenuItem("Edit/Play-Unplay, But From Prelaunch Scene %0")]
	public static void PlayFromPrelaunchScene()
	{
		if ( EditorApplication.isPlaying == true )
		{
			EditorApplication.isPlaying = false;
			EditorApplication.OpenScene(
				"Assets/Lobby/Lobby.unity");
			return;
		}
		EditorApplication.SaveCurrentSceneIfUserWantsTo();
		EditorApplication.OpenScene(
			"Assets/MainMenu/MainMenu.unity");
		EditorApplication.isPlaying = true;
	}
}