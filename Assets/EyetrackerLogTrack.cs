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

	public void LogPupilData(float pupilDiameter, string eye)
	{
		if(ExperimentSettings_CoinTask.isLogging)
		{

			subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "EYETRACKER_PUPIL_DIAMETER " + eye + separator + gazePos.x + separator + gazePos.y);
		}
	}

	public void LogDisplayData(Vector3 gazePos, string eye)
	{
		if(ExperimentSettings_CoinTask.isLogging)
		{

			subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "EYETRACKER_DISPLAY_POINT " + eye + separator + gazePos.x + separator + gazePos.y);
		}
	}
	public void LogGazeData(Vector3 gazePos, string eye)
    {
        if(ExperimentSettings_CoinTask.isLogging)
        {
			
			subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "EYETRACKER_GAZE_POINT " + eye + separator + gazePos.x + separator + gazePos.y + separator + gazePos.z);
        }

    }
}
