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

	public void LogColors(Color startColor, Color endColor){
		if (ExperimentSettings_CoinTask.isLogging) {
			//Unity doesn't allow access to each point's colors individually -- have to log them together in order to set them later T_T
			subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), gameObject.name + separator + "LINE_RENDERER_COLOR" + separator + startColor.r + separator + startColor.g + separator + startColor.b + separator + startColor.a +
			                separator + endColor.r + separator + endColor.g + separator + endColor.b + separator + endColor.a);
		}
	}
}
