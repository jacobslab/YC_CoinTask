using UnityEngine;
using System.Collections;

public class KeyboardLogTrack : LogTrack {

	public string[] Keys;

	// Use this for initialization
	void Start () {
	
	}

	void LateUpdate(){
		if(ExperimentSettings_CoinTask.isLogging){
			LogKeyboard();
			if(Input.GetAxis("MRI Trigger") > 0.0f)
				LogMRITrigger();
		}
	}

	void LogKeyboard(){
		string keyName = "";
		for (int i = 0; i < Keys.Length; i++) {
			keyName = Keys[i];
			if (Input.GetKey (keyName.ToLower())) {
				subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Keyboard" + separator + keyName);
			}
		}
	}

	void LogMRITrigger(){
		UnityEngine.Debug.Log ("logging MRI trigger");
				subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "MRI Event" + separator + "TRIGGER PULSE");
	}
}
