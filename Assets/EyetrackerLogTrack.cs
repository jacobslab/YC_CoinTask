using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class EyetrackerLogTrack : LogTrack {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LogGazeData(Vector3 gazePos, string eye)
    {
        if(ExperimentSettings_CoinTask.isLogging)
        {
			
			subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "EYETRACKER " + eye + separator + gazePos.x + separator + gazePos.y + gazePos.z);
        }

    }
}
