using UnityEngine;
using System.Collections;

public class DialogueContinueButton : MonoBehaviour {

	public DialogueController dialogueController;

	void OnClick() {
		print ("CLICK");
		dialogueController.next();
	}
}
