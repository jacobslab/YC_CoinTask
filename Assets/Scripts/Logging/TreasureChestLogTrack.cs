using UnityEngine;
using System.Collections;

public class TreasureChestLogTrack : LogTrack { //currently a treasure chest.

	SpawnableObject spawnableObject;

	// Use this for initialization
	void Start () {
		spawnableObject = GetComponent<SpawnableObject> ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void LogOpening(string closerPivotName, bool isSpecial){ //the closer pivot is used as the object reference for opening the chest later --> thus, it will open with the opposite pivot
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), GetNameToLog() + separator + "TREASURE_OPEN" + separator
			                + "IS_SPECIAL" + separator + isSpecial + separator + "OPENER_REFERENCE" + separator + closerPivotName);
		}
	}

	public void LogTreasureLabel(string labelText){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), GetNameToLog() + separator + "TREASURE_LABEL" + separator + labelText);
		}
	}

	string GetNameToLog(){
		string name = gameObject.name;
		if (spawnableObject) {
			name = spawnableObject.GetName();
		}
		return name;
	}
}
