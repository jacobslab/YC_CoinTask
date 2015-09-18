using UnityEngine;
using System.Collections;

public class ParticleLogTrack : LogTrack {

	//should be able to log either of these.
	public ParticleSystem particleSystem;
	public EllipsoidParticleEmitter particleEmitter;

	SpawnableObject spawnableObject;

	bool isSystemPlaying = false;
	bool isEmitterPlaying = false;
	// Use this for initialization
	void Start () {
		spawnableObject = GetComponent<SpawnableObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (ExperimentSettings_CoinTask.isLogging) {
			LogParticles ();
		}
	}

	void LogParticles(){
		if (particleSystem != null) {
			if (!isSystemPlaying && particleSystem.isPlaying) {
				isSystemPlaying = true;
				LogParticleSystemPlaying (particleSystem.transform.position);
			} 
			else if (isSystemPlaying && !particleSystem.isPlaying) {
				isSystemPlaying = false;
				LogParticleSystemOver (particleSystem.transform.position);
			}
		}

		if (particleEmitter != null) {
			if (!isSystemPlaying && particleEmitter.emit) {
				isSystemPlaying = true;
				LogParticleSystemPlaying (particleEmitter.transform.position);
			} 
			else if (isSystemPlaying && !particleEmitter.emit) {
				isSystemPlaying = false;
				LogParticleSystemOver (particleEmitter.transform.position);
			}
		}
	}

	//for logging the particle system
	public void LogParticleSystemPlaying(Vector3 systemLocation){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), GetNameToLog() + separator + "PARTICLE_SYSTEM_PLAYING" + separator + particleSystem.name + separator + "IS_LOOPING" + separator + particleSystem.loop + separator + systemLocation.x + separator + systemLocation.y + separator + systemLocation.z);
	}

	public void LogParticleSystemOver(Vector3 systemLocation){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), GetNameToLog() + separator + "PARTICLE_SYSTEM_STOPPED" + separator + particleSystem.name + separator + "IS_LOOPING" + separator + particleSystem.loop + separator + systemLocation.x + separator + systemLocation.y + separator + systemLocation.z);
	}

	//for logging the particle emitter
	//note: emitters do not have a looping property
	public void LogParticleEmitterPlaying(Vector3 emitterLocation){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), GetNameToLog() + separator + "PARTICLE_EMITTER_PLAYING" + separator + particleSystem.name + separator + emitterLocation.x + separator + emitterLocation.y + separator + emitterLocation.z);
	}
	
	public void LogParticleEmitterOver(Vector3 emitterLocation){
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), GetNameToLog() + separator + "PARTICLE_EMITTER_STOPPED" + separator + particleSystem.name + separator + emitterLocation.x + separator + emitterLocation.y + separator + emitterLocation.z);
	}

	string GetNameToLog(){
		string name = gameObject.name;
		if (spawnableObject) {
			name = spawnableObject.GetName();
		}
		return name;
	}

	void OnDestroy(){
		if( isSystemPlaying ) {
			isSystemPlaying = false;
			LogParticleSystemOver (particleSystem.transform.position);
		}

		if (isEmitterPlaying) {
			isEmitterPlaying = false;
			LogParticleEmitterOver(particleEmitter.transform.position);
		}
	}

	void OnDisable(){
		if( isSystemPlaying ) {
			isSystemPlaying = false;
			particleSystem.Stop();
			LogParticleSystemOver (particleSystem.transform.position);
		}
		if (isEmitterPlaying) {
			isEmitterPlaying = false;
			particleEmitter.emit = false;
			LogParticleEmitterOver(particleEmitter.transform.position);
		}
	}
}
