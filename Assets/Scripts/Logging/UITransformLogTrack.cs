using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class UITransformLogTrack : LogTrack {

	public bool ShouldLogPosition = false;
	public bool ShouldLogScale = false;

	//TODO: log rotation too.

	// Use this for initialization
	void Awake () {

	}
	
	// Update is called once per frame
	void Update () {
		if(ExperimentSettings_CoinTask.isLogging ){ //if the text has changed, log it!
			Log ();
		}
	}

	void Log(){
		if (ShouldLogPosition) {
			LogPosition();
		}
		if (ShouldLogScale) {
			LogScale();
		}
	}

	void LogPosition(){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "UI_POSITION" + separator + transform.position.x + separator + transform.position.y + separator + transform.position.z);
	}
	
	void LogScale(){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "UI_SCALE" + separator + transform.localScale.x + separator + transform.localScale.y + separator + transform.localScale.z);
	}
}