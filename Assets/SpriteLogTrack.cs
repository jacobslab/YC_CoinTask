using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class SpriteLogTrack : LogTrack {

	SpriteRenderer mySpriteRenderer;
	Color currentColor = Color.black;

	bool firstLog = false; //should make an initial log

	// Use this for initialization
	void Awake () {
		mySpriteRenderer = GetComponent<SpriteRenderer> ();
	}

	//log on late update so that everything for that frame gets set first
	void LateUpdate () {
		if (mySpriteRenderer == null) {
			Debug.Log("Text is null! Did you mean to add a regular TextLogTrack instead?");
		}
		if (!firstLog) {
			firstLog = true;
			LogColor();
		}

		if(ExperimentSettings_CoinTask.isLogging && ( currentColor != mySpriteRenderer.color ) ){ //if the text has changed, log it!
			LogColor ();
		}
	}

	void LogColor(){
		currentColor = mySpriteRenderer.color;

		subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "SPRITE_RENDERER_COLOR" + separator + mySpriteRenderer.color.r + separator + mySpriteRenderer.color.g + separator + mySpriteRenderer.color.b + separator + mySpriteRenderer.color.a);
	}
}
