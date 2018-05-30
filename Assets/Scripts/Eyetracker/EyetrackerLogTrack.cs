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

	public void LogPupilData(float pupilDiameter, string eye, long deviceTimestamp)
	{
		if(ExperimentSettings_CoinTask.isLogging)
		{

			subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "EYETRACKER_PUPIL_DIAMETER " + eye + separator + pupilDiameter.ToString() + separator + deviceTimestamp.ToString());
		}
	}

	public void LogDisplayData(Vector2 gazePos, string eye, long deviceTimestamp)
	{
		if(ExperimentSettings_CoinTask.isLogging)
		{

			subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "EYETRACKER_DISPLAY_POINT " + eye + separator + gazePos.x + separator + gazePos.y+ separator + deviceTimestamp.ToString());
		}
	}
	public void LogGazeData(Vector3 gazePos, string eye, long deviceTimestamp)
	{
		if(ExperimentSettings_CoinTask.isLogging)
		{

			subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "EYETRACKER_GAZE_POINT " + eye + separator + gazePos.x + separator + gazePos.y + separator + gazePos.z + separator + deviceTimestamp.ToString());
		}

	}

	public void LogTimestamp(long gazeDeviceTimestamp, long gazeSystemTimestamp, long etTimestamp)
	{
		if(ExperimentSettings_CoinTask.isLogging)
		{

			subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "EYETRACKER_TIMESTAMP" + separator + gazeDeviceTimestamp.ToString() + separator + gazeSystemTimestamp.ToString() + separator + etTimestamp.ToString());
		}

	}

	public void LogLatency(long latency)
	{
		if(ExperimentSettings_CoinTask.isLogging)
		{

			subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "EYETRACKER_LATENCY" + separator + latency.ToString());
		}
	}

	public void LogVirtualPointData(Vector3 virtualGazePos, string eye, long deviceTimestamp)
	{
		if(ExperimentSettings_CoinTask.isLogging)
		{

			subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "EYETRACKER_VIRTUAL_POINT " + eye + separator + virtualGazePos.x + separator + virtualGazePos.y + separator + virtualGazePos.z + separator + deviceTimestamp.ToString());
		}
	}

	public void LogGazeObject(string gazeObjName, long deviceTimestamp)
	{
		if(ExperimentSettings_CoinTask.isLogging)
		{

			subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "EYETRACKER_GAZE_OBJECT " + separator + gazeObjName + separator + deviceTimestamp.ToString());
		}
	}
}