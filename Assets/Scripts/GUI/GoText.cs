using UnityEngine;
using System.Collections;

[RequireComponent (typeof (ScaleAnimator))]
[RequireComponent (typeof (ColorChanger))]
public class GoText : MonoBehaviour {

	public TextMesh text;

	ColorChanger myColorChanger;
	public Color startColor;
	public Color endColor;

	Vector3 startScale;
	float animationTime = 0.3f;
	float fadeOutTime = 0.1f;

	// Use this for initialization
	void Start () {
		Stop ();
		startScale = transform.localScale;
		myColorChanger = GetComponent<ColorChanger> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Play(){
		if (Config_CoinTask.isJuice) {
			text.text = "GO!";
			StartCoroutine (ScaleUp ());
			ChangeColor ();
		}
	}

	public void Stop(){
		text.text = "";
	}

	void ResetScale(){
		transform.localScale = startScale;
	}

	IEnumerator ScaleUp(){
		ResetScale ();
		float fullScaleMult = 3.0f;
		float smallScaleMult = 1.0f;
		yield return StartCoroutine( GetComponent<ScaleAnimator>().AnimateScaleUp(animationTime, fullScaleMult, smallScaleMult) );
		yield return StartCoroutine (FadeOut ());
		Stop ();
	}

	IEnumerator FadeOut(){
		Color TransparentEndColor = new Color (endColor.r, endColor.g, endColor.b, 0.0f);
		yield return StartCoroutine (myColorChanger.LerpChangeColor (TransparentEndColor, fadeOutTime));
	}

	void ChangeColor(){
		myColorChanger.ChangeColor (startColor);
		StartCoroutine (myColorChanger.LerpChangeColor (endColor, animationTime - fadeOutTime) );
	}
	
}
