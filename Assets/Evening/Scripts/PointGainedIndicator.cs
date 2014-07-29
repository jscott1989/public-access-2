using UnityEngine;
using System.Collections;

public class PointGainedIndicator : MonoBehaviour {

	Texture2D mNegativeTexture;
	dfTextureSprite mTexture;

	void Awake() {
		mNegativeTexture = (Texture2D)Resources.Load ("Evening/Images/unhappy");
		mTexture = gameObject.GetComponent<dfTextureSprite>();
	}

	/**
	 * Set ourselves with the negative icon, rather than the positive one
	 */
	public void SetNegative() {
		mTexture.Texture = mNegativeTexture;
	}

	/**
	 * Move to the target and then destroy ourself
	 */
	public void MoveTo(dfTextureSprite pTarget) {
		dfTweenVector3 v = GetComponent<dfTweenVector3>();
		mTexture.BringToFront();
		v.StartValue = mTexture.Position;
		v.EndValue = new Vector3(pTarget.Position.x + (pTarget.Width/2), pTarget.Position.y - (pTarget.Height/2));
		v.Play();
	}

	public void TweenFinished() {
		Destroy(gameObject);
	}

	void OnClick() {
		MoveTo(GameObject.FindGameObjectWithTag("Dad").GetComponent<dfTextureSprite>());
	}
}
