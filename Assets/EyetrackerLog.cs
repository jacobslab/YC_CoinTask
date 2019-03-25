using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyetrackerLog : LogTrack
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void LogEyetrackerScreenPosition(Vector3 screenPos, long timestamp)
    {
        subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "EYETRACKER_DISPLAY_POINT" + separator + screenPos.x.ToString() + separator + screenPos.y.ToString() + separator + timestamp.ToString());
    }

    public void LogGazeObject(string objName, long timestamp)
    {
        subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "EYETRACKER_GAZE_OBJECT" + separator + objName + separator + timestamp.ToString());
    }
    public void LogVirtualPointData(Vector3 hitpoint, long timestamp)
    {
        subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "EYETRACKER_VIRTUAL_POINT" + separator + hitpoint.x.ToString() + separator + hitpoint.y.ToString() + separator + hitpoint.z.ToString() + separator + timestamp.ToString());
    }
}
