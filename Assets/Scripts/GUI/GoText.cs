using UnityEngine;
using System.Collections;

[RequireComponent (typeof (ScaleAnimator))]
[RequireComponent (typeof (ColorChanger))]
public class GoText : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public TextMesh text;

	ColorChanger myColorChanger;
	public Color startColor;
	public Color endColor;

	Vector3 startScale;
	public float animationTime = 0.8f;
	public float fadeOutTime = 0.3f;

	// Use this for initialization
	void Start () {
		Stop ();
		startScale = transform.localScale;
		myColorChanger = GetComponent<ColorChanger> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void Play(string newText){
		if (Config_CoinTask.isJuice) {
            text.text = newText;
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
