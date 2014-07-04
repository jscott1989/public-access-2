using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ViewerGraph : MonoBehaviour {

	// The maximum amount of viewers that need representing on the graph
	public int maxViewers = 20;

	int[] viewingFigures;
	Sprite[] sprites;
	string[] spriteNames;

	public PretendVideoPlayer videoPlayer;

	// TODO: Calculate the width rather than hardcoding
	float second_width = 0.26f;

	public GameObject marker;

	// Use this for initialization
	void Start () {
		// Load all available sprites
		sprites = Resources.LoadAll<Sprite>("Images/Morning/graph");
		spriteNames = new string[sprites.Length];

		for(int ii=0; ii< spriteNames.Length; ii++) {
			spriteNames[ii] = sprites[ii].name;
		}

		// TODO: Basically we need a bit where the data is fed in, then converted into a reasonable format
		// for now we'll just hardcode 30 seconds of viewing figures

		viewingFigures = new int[]{0,0,0,1,2,4,6,7,11,11,12,10,8,7,7,8,9,5,5,4,6,4,4,4,3,3,3,2,1,0,0};

		for (int i = 0; i < viewingFigures.Length; i++) {
			SetViewers (i, viewingFigures[i]);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	GameObject[] GetSeconds() {
		List<GameObject> l = new List<GameObject> ();

		// No idea why this isn't easier - but build a list of the gameobjects representing seconds
		foreach (Transform child in transform)
		{
			if (child.gameObject.tag == "second_marker") {
				l.Add (child.gameObject);
			}
		}

		// Then sort it so the most leftmost is first (so it should be in order of seconds)
		l = l.OrderBy(o=>o.transform.position.x).ToList();

		return l.ToArray ();
	}

	GameObject GetSecond(int second) {
		return GetSeconds()[second];
	}

	Sprite GetSprite(int viewers, int maxViewers) {
		return sprites[Array.IndexOf(spriteNames, maxViewers.ToString () + "_" + viewers.ToString ())];
	}

	/**
	 * Set the number of viewers for a given second
	 */
	public void SetViewers(int second, int viewers) {
		SpriteRenderer s = (SpriteRenderer)GetSecond (second).GetComponent (typeof(SpriteRenderer));
		s.sprite = GetSprite (viewers, maxViewers);
	}

	public void SetTime(float time) {
		Vector3 base_position = GetSecond ((int)time).transform.position;
		time -= (int)time;
		base_position.x += time * second_width;
		marker.transform.position = base_position;
	}

	void OnMouseDown() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float mousePosition = ray.origin.x - GetSecond (0).transform.position.x - 4.062064f;

		// This gives us a range of 0 - 0.24
		// multiply it by 4 gives us basically 0-1
		mousePosition *= 4;

		// Then by 30 gives us 0-30
		mousePosition *= 30;

		float time = mousePosition;

		while (time < 0) {
			time += 30;
		}
		while (time > 30) {
			time -= 30;
		}

		videoPlayer.SetTime (time);
		SetTime (time);
	}

	void OnMouseDrag() {
		OnMouseDown();
	}
}
