using UnityEngine;
using System;
using System.Collections;


/**
 * This manages the purchase of props
 */
public class PropSelectionManager : MonoBehaviour {

	public BudgetController budgetController;
	public Countdown countdown;

	void Start () {
		// TODO: The available props should have been pre-configured
		// for now we can just set some

		Action countdownCompleted =
			() => Application.LoadLevel ("Afternoon");
		countdown.StartCountdown (30, countdownCompleted);
	}

	public void PurchaseProp() {
		if (budgetController.budget >= 200) {
			budgetController.budget -= 200;
		}
	}

	public void SellProp() {
		budgetController.budget += 200;
	}
}
