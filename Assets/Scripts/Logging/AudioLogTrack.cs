using UnityEngine;
using System.Collections;

public class AudioLogTrack : LogTrack {
	//public bool useParentsName = false;

	public AudioSource audioSource;
	SpawnableObject spawnableObject;

	bool isAudioPlaying = false;
	// Use this for initialization
	void Start () {
		spawnableObject = GetComponent<SpawnableObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (ExperimentSettings_CoinTask.isLogging) {
			LogAudio ();
		}
	}

	void LogAudio(){
		if (!isAudioPlaying && audioSource.isPlaying) {
			isAudioPlaying = true;
			LogAudioPlaying (audioSource.clip, audioSource.transform.position);
		} 
		else if (isAudioPlaying && !audioSource.isPlaying) {
			isAudioPlaying = false;
			LogAudioOver (audioSource.clip, audioSource.transform.position);
		}
	}

	public void LogAudioPlaying(AudioClip audioClip, Vector3 audioLocation){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), GetNameToLog() + separator + "AUDIO_PLAYING" + separator + audioClip.name + separator + "IS_LOOPING" + separator + audioSource.loop + separator + audioLocation.x + separator + audioLocation.y + separator + audioLocation.z);
	}

	public void LogAudioOver(AudioClip audioClip, Vector3 audioLocation){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), GetNameToLog() + separator + "AUDIO_STOPPED" + separator + audioClip.name + separator + "IS_LOOPING" + separator + audioSource.loop + separator + audioLocation.x + separator + audioLocation.y + separator + audioLocation.z);
	}

	string GetNameToLog(){
		string name = gameObject.name;
		/*if (useParentsName) {
			SpawnableObject parentSpawnable = transform.parent.GetComponent<SpawnableObject>();
			if( parentSpawnable != null ){
				name = parentSpawnable.GetName();
			}
			else{
				name = transform.parent.name;
			}
		}
		else */if (spawnableObject) {
			name = spawnableObject.GetName();
		}
		return name;
	}

	void OnDestroy(){
		if( isAudioPlaying ) {
			isAudioPlaying = false;
			LogAudioOver (audioSource.clip, audioSource.transform.position);
		}
	}

	void OnDisable(){
		if( isAudioPlaying ) {
			isAudioPlaying = false;
			audioSource.Stop();
			LogAudioOver (audioSource.clip, audioSource.transform.position);
		}
	}
}
