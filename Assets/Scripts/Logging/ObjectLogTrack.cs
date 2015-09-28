using UnityEngine;
using System.Collections;

public class ObjectLogTrack : LogTrack {
	SpawnableObject spawnableObject;
	string nameToLog { get { return GetNameToLog (); } }

	bool firstLog = false; //should log spawned on the first log

	//if we want to only log objects when something has changed... should start with keep track of last positions/rotations.
	//or I could set up some sort of a delegate system.
	Vector3 lastPosition;
	Quaternion lastRotation;
	Vector3 lastScale;
	bool lastVisibility;

	void Awake(){
		spawnableObject = GetComponent<SpawnableObject> ();
	}

	void Update(){
		if (ExperimentSettings_CoinTask.isLogging) {
			Log ();
		}
	}

	void Log ()
	{
		//the following is set up to log properties only when they change, or on an initial log.

		if (lastPosition != transform.position || !firstLog) {
			lastPosition = transform.position;
			LogPosition ();
		}
		if (lastRotation != transform.rotation || !firstLog) {
			lastRotation = transform.rotation;
			LogRotation ();
		}
		if (lastScale != transform.localScale || !firstLog) {
			lastScale = transform.localScale;
			LogScale ();
		}
		if (spawnableObject != null) {
			if (lastVisibility != spawnableObject.isVisible || !firstLog) {
				lastVisibility = spawnableObject.isVisible;
				LogVisibility ();
			}
		}

		if(!firstLog){
			LogSpawned();
			firstLog = true;
		}
	}

	void LogSpawned(){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), nameToLog + separator + "SPAWNED");
	}

	void LogPosition(){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), nameToLog + separator + "POSITION" + separator + transform.position.x + separator + transform.position.y + separator + transform.position.z);
	}
	
	void LogRotation(){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), nameToLog + separator + "ROTATION" + separator + transform.rotation.eulerAngles.x + separator + transform.rotation.eulerAngles.y + separator + transform.rotation.eulerAngles.z);
	}

	void LogScale(){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), nameToLog + separator + "SCALE" + separator + transform.localScale.x + separator + transform.localScale.y + separator + transform.localScale.z);
	}
	
	void LogVisibility(){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), nameToLog + separator + "VISIBILITY" + separator + spawnableObject.isVisible);
	}

	public void LogShadowSettings(UnityEngine.Rendering.ShadowCastingMode shadowMode){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), nameToLog + separator + "SHADOW_SETTING" + separator + shadowMode);
		}
	}

	public void LogLayerChange(){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), nameToLog + separator + "LAYER_CHANGE" + separator + gameObject.layer);
		}
	}

	void LogDestroy(){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), nameToLog + separator + "DESTROYED");
	}

	void OnDestroy(){
		if (ExperimentSettings_CoinTask.isLogging) {
			LogDestroy();
		}
	}

	string GetNameToLog(){
		string name = gameObject.name;
		if (spawnableObject) {
			name = spawnableObject.GetName();
		}
		return name;
	}

}