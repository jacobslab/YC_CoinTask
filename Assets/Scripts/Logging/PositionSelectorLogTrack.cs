using UnityEngine;
using System.Collections;

public class PositionSelectorLogTrack : LogTrack {
	GameObject selectorObject { get { return Experiment_CoinTask.Instance.environmentController.myPositionSelector.PositionSelector; } }
	GameObject correctIndicatorObject { get { return Experiment_CoinTask.Instance.environmentController.myPositionSelector.CorrectPositionIndicator; } }

	
	// Use this for initialization
	void Start () {

	}
	
	void Update(){
		if (ExperimentSettings_CoinTask.isLogging) {
			Log ();
		}
	}
	
	// LOGGING SHOULD BE INDEPENDENT OF FRAME RATE
	void FixedUpdate () {
		
	}
	
	
	public void Log ()
	{
		LogSelectorPosition();
		LogCorrectIndicatorPosition ();
		LogSelectorVisibility ();
		LogCorrectIndicatorVisibility ();
	}
	
	void LogSelectorPosition(){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "SELECTOR_POSITION" + separator + selectorObject.transform.position.x + separator + selectorObject.transform.position.y + separator + selectorObject.transform.position.z);
	}

	void LogSelectorVisibility(){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "SELECTOR_VISIBILITY" + separator + selectorObject.GetComponent<VisibilityToggler>().GetVisibility());
	}

	void LogCorrectIndicatorPosition(){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "CORRECT_INDICATOR_POSITION" + separator + correctIndicatorObject.transform.position.x + separator + correctIndicatorObject.transform.position.y + separator + correctIndicatorObject.transform.position.z);
	}

	void LogCorrectIndicatorVisibility(){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "CORRECT_INDICATOR_VISIBILITY" + separator + correctIndicatorObject.GetComponent<VisibilityToggler>().GetVisibility());
	}

	public void LogPositionChosen(){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "CHOSEN_TEST_POSITION" + separator + transform.position.x + separator + transform.position.y + separator + transform.position.z);
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "CORRECT_TEST_POSITION" + separator + transform.position.x + separator + transform.position.y + separator + transform.position.z);
	}
	
}
