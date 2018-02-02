using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorCycler : MonoBehaviour {

	public Color[] colors;

	SpriteRenderer mySprite;

	// Use this for initialization
	void Start () {
		mySprite = GetComponent<SpriteRenderer> ();
	}

	// Update is called once per frame
	void Update () {

	}

	public IEnumerator CycleColors(){
		if (mySprite && colors.Length > 1) {

			float currentTime = 0.0f;
			mySprite.color = colors [0];

			while (true) {
				for (int i = 0; i < colors.Length; i++) {
					while (currentTime < 1.0f) {
						currentTime += Time.deltaTime;
						if (i < colors.Length - 1) {
							mySprite.color = Color.Lerp (colors [i], colors [i + 1], currentTime);
						} else {
							mySprite.color = Color.Lerp (colors [i], colors [0], currentTime);
						}
						yield return 0;
					}
					currentTime = 0.0f;
				}
				yield return 0;
			}

		}
		yield return 0;
	}
}
