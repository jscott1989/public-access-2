using UnityEngine;
using System.Collections;

public class DialogueCreator : MonoBehaviour {
	public dfTextureSprite uImage;
	GameObject mScreen;
	
	GameObject mDialoguePrefab;
	public string uBackgroundSpriteName;
	
	void Awake() {
		uImage = (dfTextureSprite) GetComponentInChildren(typeof(dfTextureSprite));
		mDialoguePrefab = (GameObject)Resources.Load ("Afternoon/Prefabs/Dialogue");
		mScreen = GameObject.FindGameObjectWithTag("Screen");
	}
	
	void OnMouseDown() {
		GameObject g = (GameObject) Instantiate(mDialoguePrefab, Vector3.zero, Quaternion.identity);
		g.transform.parent = mScreen.transform;
		
		Vector2 mousePosition = uImage.GetManager ().ScreenToGui(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
		
		dfSlicedSprite sprite = g.GetComponent<dfSlicedSprite>();
		// TODO: these numbers are hardcoded - copied from the unity gui - figure out how to get this programatically
		Vector2 top_left = new Vector2(mousePosition.x - 183, (0-mousePosition.y) + 123);
		
		sprite.Position = new Vector2(top_left.x - (sprite.Size.x / 2), top_left.y + (sprite.Size.y / 2));
		sprite.SpriteName = uBackgroundSpriteName;
		sprite.Size = new Vector2(300, 300);
		
		MovingArea p = g.GetComponentInChildren<MovingArea>();
		p.OnMouseDown();
	}
}