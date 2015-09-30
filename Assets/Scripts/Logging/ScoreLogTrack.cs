using UnityEngine;
using System.Collections;

public class ScoreLogTrack : LogTrack {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LogTreasureOpenScoreAdded(int scoreAdded){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "SCORE_ADDED_TREASURE" + separator + scoreAdded);
	}

	public void LogTimeBonusAdded(int scoreAdded){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "SCORE_ADDED_TIME" + separator + scoreAdded);
	}

	public void LogMemoryScoreAdded(int scoreAdded){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "SCORE_ADDED_MEMORY" + separator + scoreAdded);
	}
}
