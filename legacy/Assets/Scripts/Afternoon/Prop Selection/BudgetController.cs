using UnityEngine;
using System.Collections;

public class BudgetController : MonoBehaviour {

	public int StartingBudget = 1000;

	private int _budget;
	private UILabel u;
	
	public int budget
	{
		get {
			return _budget;
		}
		set {
			_budget = value;

			u.text = "$" + _budget.ToString() + " left to spend";
		}
	}

	// Use this for initialization
	void Start () {
		u = (UILabel)GetComponent (typeof(UILabel));
		budget = StartingBudget;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
