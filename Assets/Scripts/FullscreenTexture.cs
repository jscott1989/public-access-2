using UnityEngine;
using System.Collections;


/**
 * This script should attach to backgrounds and it will cause them to go fullscreen regardless of resolution
 * 
 * If the aspect ratio is incorrect - the script will scale the texture too large and then center it
 */
public class FullscreenTexture : MonoBehaviour {

	public float lastScreenWidth = 0f;
	public float lastScreenHeight = 0f;

	void Update(){
		if( lastScreenWidth != Screen.width || lastScreenHeight != Screen.height ){
			lastScreenWidth = Screen.width;
			lastScreenHeight = Screen.height;
			StartCoroutine("AdjustScale");
		}
		
	}

	IEnumerator AdjustScale(){
		float aspect_ratio =  (float) guiTexture.texture.height / (float) guiTexture.texture.width;
		
		int texture_width = Screen.width;
		int texture_height = (int) (Screen.width * aspect_ratio);

		if (texture_height < Screen.height) {
			// If we can't make it fit by width, do it by height
			aspect_ratio = (float) guiTexture.texture.width / (float) guiTexture.texture.height;
			texture_height = Screen.height;
			texture_width = (int) (Screen.height * aspect_ratio);
		}
		
		// Ensure it is centered
		int x_offset = (Screen.width - texture_width) / 2;
		int y_offset = (Screen.height - texture_height) / 2;
		
		guiTexture.pixelInset = new Rect(x_offset, y_offset, texture_width, texture_height);
		//		guiTexture.pixelInset = new Rect(0, 0, Screen.width, Screen.height);
		return null;
	}
}
