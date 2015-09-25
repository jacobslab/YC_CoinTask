using UnityEngine;
using System.Collections;

public class LineRendererLogTrack : LogTrack {

	public LineRenderer lineRenderer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//currently just logs one point at a time.
	public void LogPoint(Vector3 position, int positionIndex){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), gameObject.name + separator + "LINE_RENDERER_POSITION" + separator + positionIndex + separator + position.x + separator + position.y + separator + position.z);
		}
	}
}
