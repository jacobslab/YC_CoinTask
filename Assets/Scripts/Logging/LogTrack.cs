using UnityEngine;
using System.Collections;

//a parent class for all log track classes
public abstract class LogTrack : MonoBehaviour {
	public Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }
	public Logger_Threading subjectLog { get { return Experiment_CoinTask.Instance.subjectLog; } }
	public Logger_Threading eegLog { get { return Experiment_CoinTask.Instance.eegLog; } }
	public string separator { get { return Logger_Threading.LogTextSeparator; } }

	string GetNameToLog(){
		string name = gameObject.name;
		SpawnableObject spawnObj = GetComponent<SpawnableObject> ();
		if (spawnObj != null) {
			name = spawnObj.GetName();
		}
		return name;
	}

	public void LogDestroy(){
		subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), GetNameToLog() + separator + "DESTROYED");
	}

	void OnDestroy(){
		if (ExperimentSettings_CoinTask.isLogging) {
			LogDestroy();
		}
	}
}
