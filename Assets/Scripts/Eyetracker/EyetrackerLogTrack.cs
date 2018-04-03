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

			subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "EYETRACKER_PUPIL_DIAMETER " + eye + separator + pupilDiameter.ToString());
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
	public void LogVirtualPointData(Vector3 virtualGazePos, string eye)
	{
		if(ExperimentSettings_CoinTask.isLogging)
		{

			subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "EYETRACKER_VIRTUAL_POINT " + eye + separator + virtualGazePos.x + separator + virtualGazePos.y + separator + virtualGazePos.z);
		}
	}

	public void LogGazeObject(string gazeObjName)
	{
		if(ExperimentSettings_CoinTask.isLogging)
		{

			subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "EYETRACKER_GAZE_OBJECT " + separator + gazeObjName);
		}
	}
}
