using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrophyLogTrack  : LogTrack {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LogTrophyAwardedEvent()
	{
		Debug.Log ("awarded trophy " + gameObject.name);
		subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "TROPHY_AWARDED" + separator + gameObject.name);
	}

	public void LogTrophyRedeemedEvent()
	{
		Debug.Log ("redeemed trophy " + gameObject.name);
		subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "TROPHY_REDEEMED" + separator + gameObject.name);
	}
}
