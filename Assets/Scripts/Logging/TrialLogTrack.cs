using UnityEngine;
using System.Collections;

public class TrialLogTrack : LogTrack {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	//gets called from trial controller instead of in update!
	public void Log(int trialNumber, bool isStim){
		if (ExperimentSettings_CoinTask.isLogging) {
			LogTrial (trialNumber, isStim);
		}
	}
	
	void LogTrial(int trialNumber, bool isStim){
		subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Trial Info" + separator + "NUM_TRIALS" + separator + trialNumber + separator + "IS_STIM" + separator + isStim);
	}
}
