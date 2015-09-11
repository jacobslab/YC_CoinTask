using UnityEngine;
using System.Collections;

public class AudioLogTrack : LogTrack {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LogAudioPlaying(AudioClip audioClip, Vector3 audioLocation){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "AUDIO_PLAYING" + separator + audioClip.name + separator + audioLocation.x + separator + audioLocation.y + separator + audioLocation.z);
	}

	public void LogAudioOver(AudioClip audioClip, Vector3 audioLocation){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), gameObject.name + separator + "AUDIO_STOPPED" + separator + audioClip.name + separator + audioLocation.x + separator + audioLocation.y + separator + audioLocation.z);
	}
}
