using UnityEngine;
using System.Collections;

public class MouseLogTrack : LogTrack {

	// Use this for initialization
	void Start () {
	
	}

	void Update(){
		if (!ExperimentSettings_CoinTask.isOculus && ExperimentSettings_CoinTask.isLogging) {
			LogMouse ();
		}
	}

	void LogMouse(){
		//log the position
		//TODO: do a check if the mouse position is out of range.
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Mouse" + separator + "POSITION" + separator + Input.mousePosition.x + separator + Input.mousePosition.y);

		//log a clicked object
		if(Input.GetMouseButtonDown(0)){
			Ray ray;
			RaycastHit hit;
			if(Camera.main != null){
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(ray, out hit)){
					subjectLog.Log(exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Mouse"+ separator + "CLICKED" + separator + hit.collider.gameObject);
				}
				else{
					subjectLog.Log(exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Mouse" + separator +"CLICKED" + separator + "EMPTY");
				}
			}
			else{
				Debug.Log("Camera.main is null! Can't raycast mouse position.");
			}
		}
	}
}
