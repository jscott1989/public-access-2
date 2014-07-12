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

	void Awake() {
		mNetworkManager = (NetworkManager) FindObjectOfType(typeof(NetworkManager));
		mDialogueManager = (DialogueManager) FindObjectOfType(typeof(DialogueManager));
		mCountdown = (Countdown) FindObjectOfType(typeof(Countdown));
		mAvailablePropsList = (dfListbox) FindObjectOfType(typeof(dfListbox));
		mMyProps = (MyProps) FindObjectOfType(typeof(MyProps));
		mGame = (Game) FindObjectOfType(typeof(Game));
	}

	void Start () {
		// TODO: Check what day it is - for now assume day 1 so give an introduction to prop selection

		PopulateAvailableProps();
		PopulatePurchasedProps();

		// First we need to set everyone to "Not Ready"
		if (Network.isServer) {
			foreach (Player player in mNetworkManager.players) {
				player.networkView.RPC ("SetReady", RPCMode.All, false);
			}
		}

		string[] propSelectionDialogue = new string[]{
			"text about prop selection..",
			"little bit of an overview..."
		};

		Action propSelectionDialogueComplete =
			() => {
				mNetworkManager.myPlayer.networkView.RPC("SetReady", RPCMode.All, true);
				mDialogueManager.StartDialogue("Waiting for other players to continue");
		};

		mDialogueManager.StartDialogue(propSelectionDialogue, propSelectionDialogueComplete);
	}

	void PopulateAvailableProps() {
		mAvailablePropsList.Items = new string[]{};
		foreach (Prop p in mNetworkManager.myPlayer.uUnpurchasedProps) {
			mAvailablePropsList.AddItem (p.uName + " ($" + p.uPrice + ")");
		}
	}

	void PopulatePurchasedProps() {
		// TODO This is called when first entering the scene - ensure that the MyProps lines up with what we already own
	}

	/**
	 * Can the currently selected prop be purchased
	 */
	public bool uCanBuyCurrentProp {
		get {
			if (mAvailablePropsList.SelectedIndex < 0) {
				return false;
			}
			Prop purchase = mNetworkManager.myPlayer.uUnpurchasedProps[mAvailablePropsList.SelectedIndex];
			if (purchase.uPrice <= mNetworkManager.myPlayer.uBudget) {
				return true;
			}
			return false;
		}
	}
	
	public string uPurchaseButtonText {
		get {
			if (mAvailablePropsList.SelectedIndex < 0) {
				return "Buy";
			}
			Prop purchase = mNetworkManager.myPlayer.uUnpurchasedProps[mAvailablePropsList.SelectedIndex];
			return "Buy $" + purchase.uPrice;
		}
	}

	/**
	 * The "Buy" button has been pressed
	 */
	public void BuySelectedProp() {
		Prop purchase = mNetworkManager.myPlayer.uUnpurchasedProps[mAvailablePropsList.SelectedIndex];
		mNetworkManager.myPlayer.PurchaseProp(purchase.uID);
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
			if (!mGame.DEBUG_MODE) {
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

		mCountdown.StartCountdown (60, countdownFinished);
	}

	public void ReadyButtonPressed() {
		// Once ready is pressed we need to block the rest of the scene, so we'll show a cancellable dialogue
		mNetworkManager.myPlayer.networkView.RPC("SetReady", RPCMode.All, true);
		Action readyCancelled =
			() => {
				mNetworkManager.myPlayer.networkView.RPC("SetReady", RPCMode.All, false);
		};
		mDialogueManager.StartDialogue ("Waiting for other players...", readyCancelled, "Cancel");
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
		Application.LoadLevel ("Afternoon");
	}
}
