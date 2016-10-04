using UnityEngine;
using System.Collections;

public class EyetrackerLogTrack : LogTrack
{

    //currently just logs one point at a time.
    public void LogScreenGazePoint(Vector2 position)
    {
        if (ExperimentSettings_CoinTask.isLogging)
        {
            subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "SCREEN_GAZE_POSITION" + separator + position.x + separator + position.y);
        }
    }

    public void LogWorldGazePoint(Vector3 position)
    {
        if (ExperimentSettings_CoinTask.isLogging)
        {
            subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "WORLD_GAZE_POSITION" + separator + position.x + separator + position.y + separator + position.z);
        }
    }

    public void LogGazeObject(GameObject gazeObject)
    {
        if (ExperimentSettings_CoinTask.isLogging)
        {
            subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "GAZE_OBJECT" + separator + gazeObject.name);
        }
    }
    public void LogLeftPupilDiameter(double pupilDiameter)
    {
        if (ExperimentSettings_CoinTask.isLogging)
        {
            subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "LEFT_PUPIL_DIAMETER" + separator + pupilDiameter.ToString("F3"));
        }
    }
    public void LogRightPupilDiameter(double pupilDiameter)
    {
        if (ExperimentSettings_CoinTask.isLogging)
        {
            subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "RIGHT_PUPIL_DIAMETER" + separator + pupilDiameter.ToString("F3"));
        }
    }
}
