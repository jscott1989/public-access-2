using UnityEngine;
using System.Collections;

/**
 * Helper script to stop things being destroyed between scene changes
 */
public class DoNotDestroy : MonoBehaviour {
	void Start () {
		DontDestroyOnLoad (gameObject);
	}
}
