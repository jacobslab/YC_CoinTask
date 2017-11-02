using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class EyetrackerLogTrack : LogTrack {

	// Use this for initialization
	void Start () {
        LogGazeData(Vector3.zero);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LogGazeData(Vector3 gazePos)
    {
        if(ExperimentSettings_CoinTask.isLogging)
        {
            using (StreamWriter outputFile = new StreamWriter(Application.dataPath + @"\eyeTrackerInfo.txt"))
            {
                string gazeString = "EYETRACKER LEFT: " + gazePos.x.ToString() + "," + gazePos.y.ToString() + "," + gazePos.z.ToString();
                outputFile.WriteLine(gazeString);
            }
            //subjectLog.Log(GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "EYETRACKER LEFT" + separator + gazePos.x + separator + gazePos.y + gazePos.z);
        }

    }
}
