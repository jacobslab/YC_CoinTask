using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class Experiment_CoinTask : MonoBehaviour {

	//clock!
	public GameClock theGameClock;

	//juice controller
	public JuiceController juiceController;

	//instructions
	public InstructionsController englishInstructions;
	public InstructionsController spanishInstructions;
	public InstructionsController germanInstructions;

	[HideInInspector] public InstructionsController currInstructions;
	//public InstructionsController inGameInstructionsController;
	public CameraController cameraController;

	//logging
	private string subjectLogfile; //gets set based on the current subject in Awake()
	public Logger_Threading subjectLog;
	private string eegLogfile; //gets set based on the current subject in Awake()
	public Logger_Threading eegLog;
	public string sessionDirectory;
	public static string sessionStartedFileName = "sessionStarted.txt";
	public static int sessionID;

	//session controller
	public TrialController trialController;

	//DISTRACTOR GAME
	public BoxSwapGameController boxGameController;

	//instruction video player
	public VideoPlay instrVideoPlayer;


	//eyetracker manager
	public EyetrackerManager eyeManager;

	//pocketsphinx
	public SphinxTest sphinxTest;

	//score controller
	public ScoreController scoreController;

	//audio controller
	public AudioController audioController; 
	//object controller
	public ObjectController objectController;

	//environment controller
	public EnvironmentController environmentController;

	//math operations
	public MathOperations mathOperations;

	//UI controller
	public UIController uiController;

	//avatar
	public Player player;

    public static bool expReady = false;

	//public bool isOculus = false;

	//state enum
	public ExperimentState currentState = ExperimentState.inExperiment;

	public enum ExperimentState
	{
		//instructionsState,
		inExperiment,
		inExperimentOver,
	}

	//bools for whether we have started the state coroutines
	bool isRunningInstructions = false;
	bool isRunningExperiment = false;


	//EXPERIMENT IS A SINGLETON
	private static Experiment_CoinTask _instance;

	public static Experiment_CoinTask Instance{
		get{
			return _instance;
		}
	}

	void Awake(){
		if (_instance != null) {
			Debug.Log("Instance already exists!");
			return;
		}
		_instance = this;

		juiceController.Init ();

		cameraController.SetInGame(); //don't use oculus for replay mode

		InitInstructionsController ();

		if (ExperimentSettings_CoinTask.isLogging) {
#if !UNITY_WEBPLAYER
			InitLogging();
#else
			ExperimentSettings_CoinTask.isLogging = false;
#endif
		}
		else if(ExperimentSettings_CoinTask.isReplay) {
			currInstructions.TurnOffInstructions();
		}
		if(Config_CoinTask.isDemo)
        {

			Debug.Log("running experiment");
			StartCoroutine(BeginExperiment());
		}

	}

	void InitInstructionsController(){
		switch (ExperimentSettings_CoinTask.myLanguage) {
		case ExperimentSettings_CoinTask.LanguageSetting.English:
			currInstructions = englishInstructions;
			break;
		case ExperimentSettings_CoinTask.LanguageSetting.Spanish:
			currInstructions = spanishInstructions;
			break;
		case ExperimentSettings_CoinTask.LanguageSetting.German:
			currInstructions = germanInstructions;
			break;
		}

		uiController.InitText ();
	}
	
	//TODO: move to logger_threading perhaps? *shrug*
	void InitLogging(){
		string subjectDirectory = ExperimentSettings_CoinTask.defaultLoggingPath + ExperimentSettings_CoinTask.currentSubject.name + "/";
		sessionDirectory = subjectDirectory + "session_0" + "/";
		
		sessionID = 0;
		string sessionIDString = "_0";
		
		if(!Directory.Exists(subjectDirectory)){
			Directory.CreateDirectory(subjectDirectory);
		}
		while (File.Exists(sessionDirectory + sessionStartedFileName)){//Directory.Exists(sessionDirectory)) {
			sessionID++;

			sessionIDString = "_" + sessionID.ToString();
			
			sessionDirectory = subjectDirectory + "session" + sessionIDString + "/";
		}
		
		//delete old files.
		if(Directory.Exists(sessionDirectory)){
			DirectoryInfo info = new DirectoryInfo(sessionDirectory);
			FileInfo[] fileInfo = info.GetFiles();
			for(int i = 0; i < fileInfo.Length; i++){
				File.Delete(fileInfo[i].ToString());
			}
		}
		else{ //if directory didn't exist, make it!
			Directory.CreateDirectory(sessionDirectory);
		}
		
		subjectLog.fileName = sessionDirectory + ExperimentSettings_CoinTask.currentSubject.name + "Log" + ".txt";
		eegLog.fileName = sessionDirectory + ExperimentSettings_CoinTask.currentSubject.name + "EEGLog" + ".txt";
		Debug.Log ("SUBJECT LOG: " + subjectLog.fileName);
		string audioPath=sessionDirectory+ "audio/";
		sphinxTest.SetPath (audioPath);
		Debug.Log("the audio path is this OK: " + audioPath);
	}

	//In order to increment the session, this file must be present. Otherwise, the session has not actually started.
	//This accounts for when we don't successfully connect to hardware -- wouldn't want new session folders.
	//Gets created in TrialController after any hardware has connected and the instruction video has finished playing.
	public void CreateSessionStartedFile(){
		StreamWriter newSR = new StreamWriter (sessionDirectory + sessionStartedFileName);
	}

	// Use this for initialization
	void Start () {
        //Config_CoinTask.Init();
        //inGameInstructionsController.DisplayText("");

        Application.targetFrameRate = 60;
	}

	// Update is called once per frame
	void Update () {
		//Proceed with experiment if we're not in REPLAY mode
		if (!ExperimentSettings_CoinTask.isReplay) { //REPLAY IS HANDLED IN REPLAY.CS VIA LOG FILE PARSING

			/*if (currentState == ExperimentState.instructionsState && !isRunningInstructions) {
				Debug.Log("running instructions");

				StartCoroutine(RunInstructions());

			}
			else*/
			if(!Config_CoinTask.isDemo)
			{			if (currentState == ExperimentState.inExperiment && !isRunningExperiment && EyetrackerManager.finishedCalibration)
				{
					Debug.Log("running experiment");
					StartCoroutine(BeginExperiment());
				}
			}

		}
	}

	public IEnumerator RunOutOfTrials(){

		currInstructions.SetInstructionsColorful(); //want to keep a dark screen before transitioning to the end!
		EndExperiment();

		yield return 0;
	}

	public IEnumerator RunInstructions(){
		isRunningInstructions = true;

		//IF THERE ARE ANY PRELIMINARY INSTRUCTIONS YOU WANT TO SHOW BEFORE THE EXPERIMENT STARTS, YOU COULD PUT THEM HERE...

		currentState = ExperimentState.inExperiment;
		isRunningInstructions = false;

		yield return 0;

	}


	public IEnumerator BeginExperiment(){
		isRunningExperiment = true;

		yield return StartCoroutine(trialController.RunExperiment());
		
		yield return StartCoroutine(RunOutOfTrials()); //calls EndExperiment()

		yield return 0;

	}

	public void EndExperiment(){
		currInstructions.DisplayText("...loading end screen...");

		Debug.Log ("Experiment Over");
		currentState = ExperimentState.inExperimentOver;
		isRunningExperiment = false;
#if UNITY_WEBPLAYER
		Application.LoadLevel ("MainIsland"); //avoid main menu for web build.
#else
		SceneController.Instance.LoadEndMenu();
#endif
	}

	//TODO: move to instructions controller...
	public IEnumerator ShowSingleInstruction(string line, bool isDark, bool waitForButton, bool addRandomPostJitter, float minDisplayTimeSeconds){
		if(isDark){
			currInstructions.SetInstructionsColorful();
		}
		else{
			currInstructions.SetInstructionsTransparentOverlay();
		}
		line = line.Replace ("NEWLINE", "\n");
		currInstructions.DisplayText(line);

		yield return new WaitForSeconds (minDisplayTimeSeconds);

		if (waitForButton) {
			yield return StartCoroutine (WaitForActionButton ());
		}

		if (addRandomPostJitter) {
			yield return StartCoroutine(WaitForJitter ( Config_CoinTask.randomJitterMin, Config_CoinTask.randomJitterMax ) );
		}

		currInstructions.TurnOffInstructions ();
		cameraController.SetInGame();
	}
	
	public IEnumerator WaitForActionButton(){
		bool hasPressedButton = false;
		while(Input.GetAxis(Config_CoinTask.ActionButtonName) != 0f){
			yield return 0;
		}
		while(!hasPressedButton){
			if(Input.GetAxis(Config_CoinTask.ActionButtonName) == 1.0f){
				hasPressedButton = true;
			}
			yield return 0;
		}
	}

	public IEnumerator WaitForJitter(float minJitter, float maxJitter){
		float randomJitter = Random.Range(minJitter, maxJitter);
		trialController.GetComponent<TrialLogTrack>().LogWaitForJitterStarted(randomJitter);
		
		float currentTime = 0.0f;
		while (currentTime < randomJitter) {
			currentTime += Time.deltaTime;
			yield return 0;
		}

		trialController.GetComponent<TrialLogTrack>().LogWaitForJitterEnded(currentTime);
	}

	public IEnumerator WaitForISI(float interTime)
	{
		trialController.GetComponent<TrialLogTrack>().LogWaitForISIBegan(interTime);
		float currentTime = 0.0f;
		while (currentTime < interTime) {
			currentTime += Time.deltaTime;
			yield return 0;
		}

		trialController.GetComponent<TrialLogTrack>().LogWaitForISIEnded(currentTime);
	}

	public void OnExit(){ //call in scene controller when switching to another scene!
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.close ();
			eegLog.close ();
		}
	}

	void OnApplicationQuit(){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.close ();
			eegLog.close ();
		}
	}


}
