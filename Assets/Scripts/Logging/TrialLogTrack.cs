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
	public void Log(int trialNumber, int numTreasureChests, int numSpecialObjects, bool isSequential){
		if (ExperimentSettings_CoinTask.isLogging) {
			LogTrial (trialNumber, numTreasureChests, numSpecialObjects, isSequential);
		}
	}
	
	void LogTrial(int trialNumber, int numTreasureChests, int numSpecialObjects, bool isSequential){
		subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Trial Info" + separator + "NUM_TRIALS" + separator + trialNumber + separator
		                + "NUM_TREASURE" + separator + numTreasureChests + separator + "NUM_SPECIAL_OBJECTS" + separator + numSpecialObjects + separator 
		                + "IS_SEQUENTIAL" + separator + isSequential);
	}
}
