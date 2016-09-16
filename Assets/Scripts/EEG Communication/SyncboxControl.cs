using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class SyncboxControl : MonoBehaviour {
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	[DllImport ("ASimplePlugin")]
	private static extern IntPtr OpenUSB();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr CloseUSB();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr TurnLEDOn();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr TurnLEDOff();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr TurnStimOn();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr TurnStimOff();
	[DllImport ("ASimplePlugin")]
	private static extern long SyncPulse(float stimFreq, int channel);
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr StimPulse(float durationSeconds, float freqHz, bool doRelay);
	[DllImport ("ASimplePlugin")]
	private static extern int AddTwoIntegers(int a, int b);
	public bool ShouldSyncPulse = true;
	public float PulseOnSeconds;
	public float PulseOffSeconds;
	private int channel=0;
	int pulses=0;
	public bool isUSBOpen = false;
	public static bool doingStim=false;

	//SINGLETON
	private static SyncboxControl _instance;
	
	public static SyncboxControl Instance{
		get{
			return _instance;
		}
	}
	
	void Awake(){
		
		if (_instance != null) {
			UnityEngine.Debug.Log("Instance already exists!");
			Destroy(transform.gameObject);
			return;
		}
		_instance = this;
		
	}

	// Use this for initialization
	void Start () {
		if(Config_CoinTask.isSyncbox){
            UnityEngine.Debug.Log("fake interval" + Config_CoinTask.fakeInterval);
            StartCoroutine(ConnectSyncbox());
			//StartCoroutine (RunSyncPulseManual ());
		}
	}

	public static IEnumerator RunStimPulse (float timeDuration, float frequency, bool doRelay)
	{
		UnityEngine.Debug.Log ("about to run stim pulse");
		while(!doingStim){
			UnityEngine.Debug.Log ("WAITING");
//			string stimFeedback = Marshal.PtrToStringAuto (StimPulse (timeDuration, frequency, doRelay));
//			UnityEngine.Debug.Log(stimFeedback);
//			if(stimFeedback != "finished stim pulse!"){
				doingStim = true;
			//}

			yield return 0;
		}
		UnityEngine.Debug.Log ("ME DNE");
		yield return null;

	}

	IEnumerator ConnectSyncbox(){
		while(!isUSBOpen){
			string usbOpenFeedback = Marshal.PtrToStringAuto (OpenUSB());
			UnityEngine.Debug.Log(usbOpenFeedback);
			if(usbOpenFeedback != "didn't open USB..."){
				isUSBOpen = true;
			}

			yield return 0;
		}

		StartCoroutine (RunSyncPulseManual ());
	}
	
	// Update is called once per frame
	void Update () {
		GetInput ();

		if (Input.GetKeyDown (KeyCode.D))
			channel++;
	}

	void GetInput(){
		//use this for debugging if you'd like
	}

	float syncPulseDuration = 0.05f;
	float syncPulseInterval = 1.0f;


	public IEnumerator RunSyncPulse(){
		float jitterMin = 0.1f;
		float syncPulseFakeInterval = Config_CoinTask.fakeInterval; 
		syncPulseInterval = 1f / Config_CoinTask.frequency; //the gap between wave of pulses
		syncPulseDuration = 0.01f;
		float jitterMax = syncPulseInterval - syncPulseDuration;
		Stopwatch executionStopwatch = new Stopwatch ();
		UnityEngine.Debug.Log ("about to run sync pulse");
		while (exp.trialController.NumDefaultObjectsCollected!=4) {
			executionStopwatch.Reset();
			UnityEngine.Debug.Log ("executing sync at: " + Config_CoinTask.frequency);
			pulses++;

			ToggleStimOn ();
            yield return StartCoroutine(WaitForShortTime(syncPulseFakeInterval));
			ToggleStimOff();

            //decide what to wait for next pulse
			float timeToWait;
            //change: 3 pulses per train now, instead of 2
			//at the end of the train, decide how much time to wait before the onset of the next pulse train
			if (pulses % 3 == 0) {
				timeToWait = (syncPulseInterval - (3*syncPulseFakeInterval));
				if (timeToWait < 0) {
					timeToWait = 0;
				}
                pulses = 0;
			} else {
				//do not wait, mod for simon's eeg
				/*
				timeToWait = syncPulseFakeInterval;
				if (timeToWait < 0) {
					timeToWait = 0;
				}
				*/
				timeToWait = 0;
			}
            yield return StartCoroutine(WaitForShortTime(timeToWait));

            executionStopwatch.Stop();
		}

		UnityEngine.Debug.Log ("done with sync");
	}


	//WE'RE USING THIS FUNCTION
	IEnumerator RunSyncPulseManual(){
		float jitterMin = 0.1f;
		float jitterMax = syncPulseInterval - syncPulseDuration;

		Stopwatch executionStopwatch = new Stopwatch ();
		
		while (ShouldSyncPulse) {
			executionStopwatch.Reset();


			float jitter = UnityEngine.Random.Range(jitterMin, jitterMax);//syncPulseInterval - syncPulseDuration);
			yield return StartCoroutine(WaitForShortTime(jitter));

			ToggleLEDOn ();
			yield return StartCoroutine(WaitForShortTime(syncPulseDuration));
			ToggleLEDOff();

			float timeToWait = (syncPulseInterval - syncPulseDuration) - jitter;
			if(timeToWait < 0){
				timeToWait = 0;
			}

			yield return StartCoroutine(WaitForShortTime(timeToWait));
			
			executionStopwatch.Stop();
		}
	}
	//return microseconds it took to turn on LED
	void ToggleStimOn(){
		string ledfeedback=Marshal.PtrToStringAuto(TurnStimOn());
		UnityEngine.Debug.Log ("At channel: " + ledfeedback);
		transform.GetChild (0).gameObject.GetComponent<TextMesh> ().text = ledfeedback;
		//TurnLEDOn ();
		//LogSYNCOn (GameClock.SystemTime_Milliseconds);
	}

	void ToggleStimOff(){

		string ledfeedback=Marshal.PtrToStringAuto(TurnStimOff());
		UnityEngine.Debug.Log ("turn off: " + ledfeedback);
		transform.GetChild (0).gameObject.GetComponent<TextMesh> ().text = ledfeedback;
		//LogSYNCOff (GameClock.SystemTime_Milliseconds);

	}

	//return microseconds it took to turn on LED
	void ToggleLEDOn(){
		string ledfeedback=Marshal.PtrToStringAuto(TurnLEDOn());
		UnityEngine.Debug.Log ("At channel: " + ledfeedback);
		transform.GetChild (0).gameObject.GetComponent<TextMesh> ().text = ledfeedback;
		//TurnLEDOn ();
		//LogSYNCOn (GameClock.SystemTime_Milliseconds);
	}

	void ToggleLEDOff(){

		string ledfeedback=Marshal.PtrToStringAuto(TurnLEDOff());
		UnityEngine.Debug.Log ("turn off: " + ledfeedback);
		transform.GetChild (0).gameObject.GetComponent<TextMesh> ().text = ledfeedback;
		//LogSYNCOff (GameClock.SystemTime_Milliseconds);

	}

	long GetMicroseconds(long ticks){
		long microseconds = ticks / (TimeSpan.TicksPerMillisecond / 1000);
		return microseconds;
	}
	
	IEnumerator WaitForShortTime(float jitter){
		float currentTime = 0.0f;
		while (currentTime < jitter) {
			currentTime += Time.deltaTime;
			yield return 0;
		}
		
	}

	void LogSYNCOn(long time){
		if (ExperimentSettings_CoinTask.isLogging) {
			exp.eegLog.Log (time, exp.eegLog.GetFrameCount(), "ON"); //NOTE: NOT USING FRAME IN THE FRAME SLOT
		}
	}

	void LogSYNCOff(long time){
		if (ExperimentSettings_CoinTask.isLogging) {
			exp.eegLog.Log (time, exp.eegLog.GetFrameCount(), "OFF"); //NOTE: NOT USING FRAME IN THE FRAME SLOT
		}
	}

	void LogSYNCStarted(long time, float duration){
		if (ExperimentSettings_CoinTask.isLogging) {
			exp.eegLog.Log (time, exp.eegLog.GetFrameCount (), "SYNC PULSE STARTED" + Logger_Threading.LogTextSeparator + duration);
		}
	}

	void LogSYNCPulseInfo(long time, float timeBeforePulseSeconds){
		if (ExperimentSettings_CoinTask.isLogging) {
			exp.eegLog.Log (time, exp.eegLog.GetFrameCount (), "SYNC PULSE INFO" + Logger_Threading.LogTextSeparator + timeBeforePulseSeconds*1000); //log milliseconds
		}
	}

	void OnApplicationQuit(){
		UnityEngine.Debug.Log(Marshal.PtrToStringAuto (CloseUSB()));
	}

}
