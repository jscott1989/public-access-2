using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PropSelectionManager : SceneManager {
	NetworkManager mNetworkManager;
	DialogueManager mDialogueManager;
	Countdown mCountdown;

	MyProps mMyProps;
	dfListbox mAvailablePropsList;
	Game mGame;

	// This is just to track where we are in the scene
	// 0 = the intial dialogue
	// 1 = the prop selection
	int state = 0;

	/**
	 * Proxy for the player's budget so it can be displayed on the interface
	 */
	public string uBudgetText {
		get {
			if (mNetworkManager == null) {
				return "$300 remaining";
			}
			return "$" + mNetworkManager.myPlayer.uBudget.ToString () + " remaining";
		}
	}

	public string uShowTitle {
		get {
			if (mNetworkManager == null) {
				return "";
			}
			return mNetworkManager.myPlayer.uShowName;
		}
	}

	public string uShowDescription {
		get {
			return mNetworkManager.myPlayer.uTheme;
		}
	}

	void Awake() {
		mNetworkManager = (NetworkManager) FindObjectOfType(typeof(NetworkManager));
		mDialogueManager = (DialogueManager) FindObjectOfType(typeof(DialogueManager));
		mCountdown = (Countdown) FindObjectOfType(typeof(Countdown));
		mAvailablePropsList = (dfListbox) FindObjectOfType(typeof(dfListbox));
		mMyProps = (MyProps) FindObjectOfType(typeof(MyProps));
		mGame = (Game) FindObjectOfType(typeof(Game));
	}

	void Start () {
		PopulateAvailableProps();
		PopulatePurchasedProps();

		// First we need to set everyone to "Not Ready"
		if (Network.isServer) {
			foreach (Player player in mNetworkManager.players) {
				player.networkView.RPC ("SetReady", RPCMode.All, false);
			}
		}

		if (mNetworkManager.myPlayer.uDay == 1) {
			StartFirstDay ();
		} else {
			StartOtherDay ();
		}
	}

	void StartFirstDay() {
		if (!Game.DEBUG_MODE) {
			string[] propSelectionDialogue = new string[]{
				"Here is our prop room. As you can see we have quite a wide variety of items.",
				"I've been told from the higher ups that we can't give you it all though, so you're going to have to budget.",
				"I'll see if we can get you some more money later on in the week.",
				"Click on the item you want to buy on the left and click 'Buy'. You can sell the items back by clicking 'Sell'.",
				"You have one minute - if you finish early click 'Continue'"
			};

			Action propSelectionDialogueComplete =
			() => {
				mDialogueManager.WaitForReady();
			};

			mDialogueManager.StartDialogue (propSelectionDialogue, propSelectionDialogueComplete);
		} else {
			mNetworkManager.myPlayer.PurchaseProp(mNetworkManager.myPlayer.uUnpurchasedProps[0].uID);
			mNetworkManager.myPlayer.PurchaseProp(mNetworkManager.myPlayer.uUnpurchasedProps[0].uID);
			mNetworkManager.myPlayer.PurchaseProp(mNetworkManager.myPlayer.uUnpurchasedProps[0].uID);

			StartPropSelection();
		}
	}

	void StartOtherDay() {
		if (!Game.DEBUG_MODE) {
			string[] propSelectionDialogue = new string[]{
				"You have a chance to change your props now. Try to ensure you're giving the people what they want!",
				"I've managed to get you an extra $" + mGame.uCashPerDay[mNetworkManager.myPlayer.uDay - 1] + " for your budget too"
			};
			
			Action propSelectionDialogueComplete =
			() => {
				mDialogueManager.WaitForReady();
			};
			
			mDialogueManager.StartDialogue (propSelectionDialogue, propSelectionDialogueComplete);
		} else {
			mNetworkManager.myPlayer.PurchaseProp(mNetworkManager.myPlayer.uUnpurchasedProps[0].uID);
			mNetworkManager.myPlayer.PurchaseProp(mNetworkManager.myPlayer.uUnpurchasedProps[0].uID);
			mNetworkManager.myPlayer.PurchaseProp(mNetworkManager.myPlayer.uUnpurchasedProps[0].uID);
			
			StartPropSelection();
		}
	}

	void PopulateAvailableProps() {
		mAvailablePropsList.Items = new string[]{};

		mAvailablePropsList.AddItem ("[color#ff0000]Props[/color]");

		foreach (Prop p in mNetworkManager.myPlayer.uUnpurchasedProps) {
			mAvailablePropsList.AddItem ("[color#000000]" + p.uName + " ($" + p.uPrice + ")[/color]");
		}
		mAvailablePropsList.AddItem ("[color#ff0000]Backdrops[/color]");
		foreach (Backdrop b in mNetworkManager.myPlayer.uUnpurchasedBackdrops) {
			mAvailablePropsList.AddItem ("[color#000000]" + b.uName + " ($" + b.uPrice + ")[/color]");
		}

		mAvailablePropsList.AddItem ("[color#ff0000]Sound Effects[/color]");
		foreach (Audio b in mNetworkManager.myPlayer.uUnpurchasedAudio) {
			mAvailablePropsList.AddItem ("[color#000000]" + b.uName + " ($" + b.uPrice + ")[/color]");
		}
	}

	void PopulatePurchasedProps() {
		foreach (PurchasedProp purchasedProp in mNetworkManager.myPlayer.uPurchasedProps.Values) {
			mMyProps.Add(purchasedProp);
		}
	}

	public Prop uSelectedProp {
		get {
			if (mAvailablePropsList.SelectedIndex < 1 || mAvailablePropsList.SelectedIndex > mNetworkManager.myPlayer.uUnpurchasedProps.Length) {
				return null;
			}
			return mNetworkManager.myPlayer.uUnpurchasedProps[mAvailablePropsList.SelectedIndex - 1];
		}
	}

	public Backdrop uSelectedBackdrop {
		get {
			if (mAvailablePropsList.SelectedIndex < mNetworkManager.myPlayer.uUnpurchasedProps.Length + 2 || mAvailablePropsList.SelectedIndex > mNetworkManager.myPlayer.uUnpurchasedProps.Length + 1 + mNetworkManager.myPlayer.uUnpurchasedBackdrops.Length) {
				return null;
			}
			return mNetworkManager.myPlayer.uUnpurchasedBackdrops[mAvailablePropsList.SelectedIndex - (mNetworkManager.myPlayer.uUnpurchasedProps.Length + 2)];
		}
	}

	public Audio uSelectedAudio {
		get {
			if (mAvailablePropsList.SelectedIndex < mNetworkManager.myPlayer.uUnpurchasedProps.Length + mNetworkManager.myPlayer.uUnpurchasedBackdrops.Length + 3) {
				return null;
			}
			return mNetworkManager.myPlayer.uUnpurchasedAudio[mAvailablePropsList.SelectedIndex - (mNetworkManager.myPlayer.uUnpurchasedProps.Length + mNetworkManager.myPlayer.uUnpurchasedBackdrops.Length + 3)];
		}
	}

	/**
	 * Can the currently selected prop be purchased
	 */
	public bool uCanBuyCurrentProp {
		get {
			if (uSelectedProp == null) {
				if (uSelectedBackdrop == null) {
					if (uSelectedAudio == null) {
						return false;
					}
					if (uSelectedAudio.uPrice <= mNetworkManager.myPlayer.uBudget) {
						return true;
					}
					return false;
				}
				if (uSelectedBackdrop.uPrice <= mNetworkManager.myPlayer.uBudget) {
					return true;
				}
				return false;
			}
			if (uSelectedProp.uPrice <= mNetworkManager.myPlayer.uBudget) {
				return true;
			}
			return false;
		}
	}
	
	public string uPurchaseButtonText {
		get {
			if (uSelectedProp == null) {
				if (uSelectedBackdrop == null) {
					if (uSelectedAudio == null) {
						return "Buy";
					}
					return "Buy $" + uSelectedAudio.uPrice;
				}
				return "Buy $" + uSelectedBackdrop.uPrice;
			}
			return "Buy $" + uSelectedProp.uPrice;
		}
	}

	/**
	 * The "Buy" button has been pressed
	 */
	public void BuySelectedProp() {
		if (uSelectedProp != null) {
			mNetworkManager.myPlayer.PurchaseProp(uSelectedProp.uID);
		} else if (uSelectedBackdrop != null) {
			mNetworkManager.myPlayer.PurchaseBackdrop(uSelectedBackdrop.uID);
		} else if (uSelectedAudio != null) {
			mNetworkManager.myPlayer.PurchaseAudio(uSelectedAudio.uID);
		}
	mAvailablePropsList.SelectedIndex = -1;
	}

	public override void PropPurchased(Player pPlayer, PurchasedProp pPurchasedProp) {
		if (pPlayer.uID == mNetworkManager.myPlayer.uID) {
			PopulateAvailableProps();
			mMyProps.Add(pPurchasedProp);
		}
	}

	public override void PropSold(Player pPlayer, PurchasedProp pPurchasedProp) {
		if (pPlayer.uID == mNetworkManager.myPlayer.uID) {
			mMyProps.Remove(pPurchasedProp);
			PopulateAvailableProps();
		}
	}

	/**
	 * Can the currently selected prop be sold
	 */
	public bool uCanSellCurrentProp {
		get {
			return (mMyProps.uSelectedPurchasedProp != null);
		}
	}
	
	public string uSellButtonText {
		get {
			if (mMyProps.uSelectedPurchasedProp == null) {
				return "Sell";
			}
			return "Sell $" + mMyProps.uSelectedPurchasedProp.uProp.uPrice;
		}
	}
	
	/**
	 * The "Sell" button has been pressed
	 */
	public void SellSelectedProp() {
		mNetworkManager.myPlayer.SellProp(mMyProps.uSelectedPurchasedProp.uID);
	}

	/**
	 * This is called on the server when any player changes their ready status
	 */
	public override void ReadyStatusChanged(Player pPlayer) {
		if (pPlayer.uReady) {
			// Check if all players are ready - if so we can start
			if (!Game.DEBUG_MODE) {
				foreach (Player p in mNetworkManager.players) {
					if (!p.uReady) {
						return;
					}
				}
			}
			
			// Everyone is ready, let's move on
			if (state == 0) {
				foreach (Player player in mNetworkManager.players) {
					player.networkView.RPC ("SetReady", RPCMode.All, false);
				}
				networkView.RPC ("StartPropSelection", RPCMode.All);
			} else {
				EndPropSelection();
			}
		}
	}
	
	/**
	 * Move to prop selection proper
	 */
	[RPC] void StartPropSelection() {
		mDialogueManager.EndDialogue();
		state = 1;

		Action countdownFinished =
			() => {
			if (Network.isServer) {
				// Only one person should push us to the next scene, so let it be the server
				EndPropSelection();
			}
		};

		mCountdown.StartCountdown (Game.PROP_SELECTION_COUNTDOWN, countdownFinished);
	}

	public void ReadyButtonPressed() {
		// Once ready is pressed we need to block the rest of the scene, so we'll show a cancellable dialogue
		mDialogueManager.WaitForReady(true);
	}

	/**
	 * This will only be called on the server
	 */
	void EndPropSelection() {
		// TODO: Force everyone to have at least X props (ensure that they aren't stuck with nothing on the next scene)
		foreach(Player p in mNetworkManager.players) {
			p.networkView.RPC ("EnsureMinimumProps", RPCMode.All);
		}
		networkView.RPC ("MoveToNextScene", RPCMode.All);
	}

	/**
	 * Move to afternoon scene
	 */
	[RPC] void MoveToNextScene() {
		mNetworkManager.LoadLevel ("Afternoon");
	}
}
