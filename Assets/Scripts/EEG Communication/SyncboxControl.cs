using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
public class SyncboxControl : MonoBehaviour {
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	[DllImport ("ASimplePlugin")]
	private static extern int OpenUSB();
	[DllImport ("ASimplePlugin")]
	private static extern int CloseUSB();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr TurnLEDOn();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr TurnLEDOff();
	[DllImport ("ASimplePlugin")]
	private static extern int CheckUSB ();
    [DllImport("ASimplePlugin")]
    private static extern int AddTwoIntegers(int a, int b);
    public bool ShouldSyncPulse = true;
	public float PulseOnSeconds;
	public float PulseOffSeconds;

	public bool isUSBOpen = false;



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
           // LogSyncInfo("LOGSTART");
            UnityEngine.Debug.Log(AddTwoIntegers(4, 5));
			StartCoroutine(ConnectSyncbox());
		}
	}
	IEnumerator ConnectSyncbox(){

		string connectionError = "";
		while(!isUSBOpen){
			//UnityEngine.Debug.Log ("attempting to connect");
			int usbOpenFeedback = OpenUSB();
            
			//UnityEngine.Debug.Log("USB Open response: " + usbOpenFeedback.ToString());
			if(usbOpenFeedback != 0){
               // LogSyncInfo("USB Connected");
				isUSBOpen = true;
			}

			yield return 0;
		}
		ShouldSyncPulse = true;
		StartCoroutine ("CheckSyncboxConnection");
		StartCoroutine ("RunSyncPulseManual");
		yield return null;
	}

    public void LogSyncInfo(string info)
    {
        using (StreamWriter outputFile = new StreamWriter(Application.dataPath + @"\syncboxInfo.txt"))
        {
            outputFile.WriteLine(info);
        }
    }

	// Update is called once per frame
	void Update () {
		GetInput ();

		//		if (Input.GetKeyDown (KeyCode.A)) {
		//			int ok = CheckUSB ();
		//
		//			UnityEngine.Debug.Log (ok.ToString());
		//		}
	}

	void GetInput(){
		//use this for debugging if you'd like
	}

	IEnumerator CheckSyncboxConnection()
	{
		while (ShouldSyncPulse) {
			int syncStatus = CheckUSB ();
            //LogSyncInfo("sync status is: " + syncStatus.ToString());
          //  UnityEngine.Debug.Log ("sync status is: " + syncStatus.ToString ());
			#if FREIBURG
			if (syncStatus == 0) {
               // LogSyncInfo("Syncbox connected");
               // UnityEngine.Debug.Log ("Syncbox connected");
			} 
			#else
			if (syncStatus == 1) {
				UnityEngine.Debug.Log ("Syncbox connected");
			} 
			#endif
			else {
				isUSBOpen = false;
               // LogSyncInfo("disconnected; initiating reconnection procedure");
                //UnityEngine.Debug.Log ("disconnected; initiating reconnection procedure");
				StartCoroutine (ReconnectSyncbox ());
			}
			yield return new WaitForSeconds (2f); //check every 2 seconds
			yield return 0;
		}
		yield return null;
	}

	IEnumerator ReconnectSyncbox()
	{
		//stop running coroutines
		StopCoroutine ("RunSyncPulseManual");
		StopCoroutine ("CheckSyncboxConnection");

		//close any lingering USB handles
		UnityEngine.Debug.Log(CloseUSB().ToString());

		ShouldSyncPulse = false;

		exp.trialController.TogglePause (); //pause the game
		//		yield return new WaitForSeconds(1f);
      //  LogSyncInfo("attempting to reconnect");
      //  UnityEngine.Debug.Log ("attempting to reconnect");
		yield return StartCoroutine(ConnectSyncbox());
		exp.trialController.TogglePause (); //unpause the game
		yield return null;
	}

	float syncPulseDuration = 0.05f;
	float syncPulseInterval = 1.0f;
	/*
		IEnumerator RunSyncPulse(){
			Stopwatch executionStopwatch = new Stopwatch ();

			while (ShouldSyncPulse) {
				executionStopwatch.Reset();

				SyncPulse(); //executes pulse, then waits for the rest of the 1 second interval

				executionStopwatch.Start();
				long syncPulseOnTime = SyncPulse();
				LogSYNCOn(syncPulseOnTime);
				while(executionStopwatch.ElapsedMilliseconds < 1500){
					yield return 0;
				}

				executionStopwatch.Stop();

			}
		}
*/

	//WE'RE USING THIS FUNCTION
	IEnumerator RunSyncPulseManual(){
		float jitterMin = 0.1f;
		float jitterMax = syncPulseInterval - syncPulseDuration;

		Stopwatch executionStopwatch = new Stopwatch ();

		while (ShouldSyncPulse) {
			executionStopwatch.Reset();
		//	UnityEngine.Debug.Log ("pulse running");

			float jitter = UnityEngine.Random.Range(jitterMin, jitterMax);//syncPulseInterval - syncPulseDuration);
			yield return StartCoroutine(WaitForShortTime(jitter));

			ToggleLEDOn();
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
	void ToggleLEDOn(){

		TurnLEDOn ();
		LogSYNCOn (GameClock.SystemTime_Milliseconds);
	}

	void ToggleLEDOff(){

		TurnLEDOff();
		LogSYNCOff (GameClock.SystemTime_Milliseconds);

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
		UnityEngine.Debug.Log(CloseUSB().ToString());
	}

}
