using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class SyncboxControl : MonoBehaviour {
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	//DYNLIB FUNCTIONS
	[DllImport ("liblabjackusb")]
	private static extern float LJUSB_GetLibraryVersion( );


	[DllImport ("ASimplePlugin")]
	private static extern int PrintANumber();
	[DllImport ("ASimplePlugin")]
	private static extern float AddTwoFloats(float f1,float f2);
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr OpenUSB();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr CloseUSB();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr TurnLEDOn();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr TurnLEDOff();
	[DllImport ("ASimplePlugin")]
	private static extern long SyncPulse();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr StimPulse(float durationSeconds, float freqHz, bool doRelay);
	
	public bool ShouldSyncPulse = true;
	public float PulseOnSeconds;
	public float PulseOffSeconds;
	public TextMesh DownCircle;
	public Color DownColor;
	public Color UpColor;

	public bool isUSBOpen = false; //TODO: set to true.

	bool isToggledOn = false;


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
		if(ExperimentSettings_CoinTask.isSyncbox){
			//Debug.Log(AddTwoFloats(2.5F,4F));
			//Debug.Log ("OH HAYYYY");
			//Debug.Log(PrintANumber());
			//Debug.Log (LJUSB_GetLibraryVersion ());
			StartCoroutine(ConnectSyncbox());
		}
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

		//Debug.Log(Marshal.PtrToStringAuto (CloseUSB()));
		//StartCoroutine (TestPulse ());
		StartCoroutine (RunSyncPulse ());
	}
	
	// Update is called once per frame
	void Update () {
		/*if(ExperimentSettings_CoinTask.isSyncbox){
			if (!ShouldSyncPulse) {
				GetInput ();
			}
		}*/
	}

	void GetInput(){
		/*if (Input.GetKey (KeyCode.DownArrow)) {
			ToggleOn();
		}
		else{
			ToggleOff ();
		}
		if(Input.GetKeyDown(KeyCode.S)){
			//DoSyncPulse();
			DoStimPulse();
		}*/
	}

	//float syncPulseDuration = 0.01f;
	float syncPulseInterval = 1.0f;
	//float minSyncPulseJitter = 0.8f;
	//float maxSyncPulseJitter = 1.2f;
	IEnumerator RunSyncPulse(){
		Stopwatch executionStopwatch = new Stopwatch ();

		while (ShouldSyncPulse) {
			executionStopwatch.Reset();

			/*ToggleLEDOn();
			yield return StartCoroutine(WaitForShortTime(syncPulseDuration));
			ToggleLEDOff();*/
			SyncPulse(); //executes pulse, then waits for the rest of the 1 second interval

			executionStopwatch.Start();
			long syncPulseOnTime = SyncPulse();
			LogSYNCOn(syncPulseOnTime, exp.eegLog.GetFrameCount());
			while(executionStopwatch.ElapsedMilliseconds < 1000){
				yield return 0;
			}

			executionStopwatch.Stop();
			//yield return new WaitForSeconds(syncPulseInterval);
			
			/*float timeToWait = syncPulseInterval - jitter;
			yield return new WaitForSeconds(timeToWait);*/
		}
	}

	//return microseconds it took to turn on LED
	void ToggleLEDOn(){
		Stopwatch executionStopwatch = new Stopwatch ();

		executionStopwatch.Start();

		TurnLEDOn ();

		executionStopwatch.Stop();


		long executionTimeMicro = GetMicroseconds (executionStopwatch.ElapsedTicks);
		LogSYNCOn (GameClock.SystemTime_Milliseconds, executionTimeMicro);
	}

	void ToggleLEDOff(){
		Stopwatch executionStopwatch = new Stopwatch ();
		
		executionStopwatch.Start();

		TurnLEDOff();

		executionStopwatch.Stop();
		
		
		long executionTimeMicro = GetMicroseconds (executionStopwatch.ElapsedTicks);
		LogSYNCOff (GameClock.SystemTime_Milliseconds, executionTimeMicro);

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


	//LOGGING
	/*void LogTEST(long time, bool isOn){
		if(ExperimentSettings_CoinTask.isLogging){
			if(isOn){
				exp.eegLog.Log(time, exp.eegLog.GetFrameCount(), "LED ON");
			}
			else{
				exp.eegLog.Log(time, exp.eegLog.GetFrameCount(), "LED OFF");
			}
		}
	}*/

	void LogSYNCOn(long time, long microSecondsPassed){
		if (ExperimentSettings_CoinTask.isLogging) {
			exp.eegLog.Log (time, microSecondsPassed, "ON"); //NOTE: NOT USING FRAME IN THE FRAME SLOT
		}
	}

	void LogSYNCOff(long time, long microSecondsPassed){
		if (ExperimentSettings_CoinTask.isLogging) {
			exp.eegLog.Log (time, microSecondsPassed, "OFF"); //NOTE: NOT USING FRAME IN THE FRAME SLOT
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

	/*void LogSTIM(long time, float duration){
		if (ExperimentSettings_CoinTask.isLogging) {
			exp.eegLog.Log (time, exp.eegLog.GetFrameCount (), "STIM PULSE" + Logger_Threading.LogTextSeparator + duration);
		}
	}*/


	//TOGGLING
	/*
	void ToggleOn(){
		if (!isToggledOn) {
			DownCircle.color = DownColor;
			Debug.Log(Marshal.PtrToStringAuto (TurnLEDOn()));
		}
		isToggledOn = true;
	}

	void ToggleOff(){
		if (isToggledOn) {
			DownCircle.color = UpColor;
			Debug.Log(Marshal.PtrToStringAuto (TurnLEDOff()));
		}
		isToggledOn = false;
	}


	//ex: a 10 ms pulse every second — until the duration is over...
	void DoSyncPulse(){
		LogSYNCStarted (GameClock.SystemTime_Milliseconds, syncPulseDuration);
		float timeBeforePulse = SyncPulse ();
		LogSYNCPulseInfo (GameClock.SystemTime_Milliseconds, timeBeforePulse);
		Debug.Log (timeBeforePulse);
	}

	void DoStimPulse(){
		//TODO: move these to a config file or something.
		float durationSeconds = 1.0f;
		float freqHz = 10;
		LogSTIM (GameClock.SystemTime_Milliseconds, durationSeconds);
		Debug.Log(Marshal.PtrToStringAuto (StimPulse (durationSeconds, freqHz, false)));
	}

	IEnumerator TestPulse (){
		yield return new WaitForSeconds(TCP_Config.numSecondsBeforeAlignment);
		while (true) {
			if(ShouldSyncPulse){
				ToggleOn();
				LogTEST(GameClock.SystemTime_Milliseconds, true);
				yield return new WaitForSeconds(PulseOnSeconds);
				ToggleOff();
				LogTEST(GameClock.SystemTime_Milliseconds, false);
				yield return new WaitForSeconds(PulseOffSeconds);
			}
			else{
				yield return 0;
			}
		}
	}*/

	void OnApplicationQuit(){
		UnityEngine.Debug.Log(Marshal.PtrToStringAuto (CloseUSB()));
	}

}

/*
public class SyncThread : ThreadedJob
{
	public bool isRunning = false;
	
	public SyncThread() {
		
	}
	
	protected override void ThreadFunction()
	{
		isRunning = true;
		// Do your threaded task. DON'T use the Unity API here
		while (isRunning) {
			DoSyncPulse();
		}
		
	}

	void DoSyncPulse(){

	}

	protected override void OnFinished()
	{
		// This is executed by the Unity main thread when the job is finished
		
	}
	
	public void End(){
		isRunning = false;
	}

	
}*/
