using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextUIColorCycler : MonoBehaviour {

	public Color[] colors;

	Text myText;

	// Use this for initialization
	void Start () {
		myText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public IEnumerator CycleColors(){
		if (myText && colors.Length > 1) {

			float currentTime = 0.0f;
			myText.color = colors [0];

			while (true) {
				for (int i = 0; i < colors.Length; i++) {
					while (currentTime < 1.0f) {
						currentTime += Time.deltaTime;
						if (i < colors.Length - 1) {
							myText.color = Color.Lerp (colors [i], colors [i + 1], currentTime);
						} else {
							myText.color = Color.Lerp (colors [i], colors [0], currentTime);
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
