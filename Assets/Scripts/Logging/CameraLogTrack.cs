using UnityEngine;
using System.Collections;

public class CameraLogTrack : LogTrack {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(ExperimentSettings_CoinTask.isLogging){
			LogCamera ();
		}
	}

	void LogCamera(){
		Camera myCamera = GetComponent<Camera>();
		if(myCamera != null){
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "CAMERA_ENABLED" + separator + myCamera.enabled);
		}
	}
}
