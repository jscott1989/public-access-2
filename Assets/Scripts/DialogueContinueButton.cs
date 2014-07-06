using UnityEngine;
using System.Collections;

public class DialogueContinueButton : MonoBehaviour {

	public DialogueController dialogueController;

	void OnClick() {
		dialogueController.Next();
	}
}
