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

	//LOGGED ON THE START OF THE TRIAL.
	void LogTrial(int trialNumber, int numTreasureChests, int numSpecialObjects, bool isSequential){
		subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Trial Info" + separator + "NUM_TRIALS" + separator + trialNumber + separator
		                + "NUM_TREASURE" + separator + numTreasureChests + separator + "NUM_SPECIAL_OBJECTS" + separator + numSpecialObjects + separator 
		                + "IS_SEQUENTIAL" + separator + isSequential);
	}

	public void LogInstructionEvent(){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "SHOWING_INSTRUCTIONS");
			Debug.Log ("Logged instruction event.");
		}
	}

	public void LogBeginningExplorationEvent(){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "FREE_EXPLORATION_STARTED");
			Debug.Log ("Logged exploration event.");
		}
	}

	public void LogTransportationToHomeEvent(){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "HOMEBASE_TRANSPORT_STARTED");
			Debug.Log ("Logged home transport event.");
		}
	}

	public void LogTransportationToTowerEvent(){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "TOWER_TRANSPORT_STARTED");
			Debug.Log ("Logged tower transport event.");
		}
	}

	public void LogTrialNavigationStarted(){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "TRIAL_NAVIGATION_STARTED");
			Debug.Log ("Logged nav started event.");
		}
	}

	public void LogRecallPhaseStarted(){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "RECALL_PHASE_STARTED");
			Debug.Log ("Logged recall started event.");
		}
	}

	public void LogObjectToRecall(SpawnableObject spawnableToRecall){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "RECALL_SPECIAL" + separator + spawnableToRecall.GetName ());
			Debug.Log ("Logged object recall event.");
		}
	}

	public void LogFeedbackStarted(){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "FEEDBACK_STARTED");
			Debug.Log ("Logged feedback event.");
		}
	}

}