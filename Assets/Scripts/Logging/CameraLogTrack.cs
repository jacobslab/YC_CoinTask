using UnityEngine;
using System.Collections;

public class CameraLogTrack : LogTrack {
	
	bool lastEnabled = false;

	Camera myCamera;

	// Use this for initialization
	void Start () {
		myCamera = gameObject.GetComponent<Camera>();
		if(ExperimentSettings_CoinTask.isLogging){
			//initial log
			LogCamera ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(ExperimentSettings_CoinTask.isLogging){
			if(lastEnabled != myCamera.enabled){
				LogCamera ();
			}
		}
	}

	void LogCamera(){
		if(myCamera != null){
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "CAMERA_ENABLED" + separator + myCamera.enabled);
			lastEnabled = myCamera.enabled;
		}
	}
}
