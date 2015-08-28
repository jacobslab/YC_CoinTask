using UnityEngine;
using System.Collections;

public class Experiment_CoinTask : MonoBehaviour {

	//clock!
	public GameClock theGameClock;

	//instructions
	public InstructionsController instructionsController;
	//public InstructionsController inGameInstructionsController;
	public CameraController cameraController;

	//logging
	private string subjectLogfile; //gets set based on the current subject in Awake()
	public Logger_Threading subjectLog;
	private string eegLogfile; //gets set based on the current subject in Awake()
	public Logger_Threading eegLog;

	//session controller
	public TrialController trialController;

	//score controller
	public ScoreController scoreController;

	//object controller
	public ObjectController objectController;

	//environment controller
	public EnvironmentController environmentController;

	//environment map visuals
	public EnvironmentMap environmentMap;

	//avatar
	public AvatarControls avatar;

	//public bool isOculus = false;


	//state enum
	public ExperimentState currentState = ExperimentState.instructionsState;

	public enum ExperimentState
	{
		instructionsState,
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

		/*if (ExperimentSettings_CoinTask.isLogging) {
			subjectLogfile = "TextFiles/" + ExperimentSettings_CoinTask.currentSubject.name + "Log.txt";
			eegLogfile = "TextFiles/" + ExperimentSettings_CoinTask.currentSubject.name + "EEGLog.txt";

			subjectLog.fileName = subjectLogfile;
			eegLog.fileName = eegLogfile;

		}
		else if(ExperimentSettings_CoinTask.isReplay) {
			instructionsController.TurnOffInstructions();
			cameraController.SetInGame(); //don't use oculus for replay mode
		}*/

	}

	// Use this for initialization
	void Start () {
		Config_CoinTask.Init();
		//inGameInstructionsController.DisplayText("");
	}

	// Update is called once per frame
	void Update () {
		//Proceed with experiment if we're not in REPLAY mode
		/*if (!ExperimentSettings_CoinTask.isReplay) { //REPLAY IS HANDLED IN REPLAY.CS VIA LOG FILE PARSING

			if (currentState == ExperimentState.instructionsState && !isRunningInstructions) {
				Debug.Log("running instructions");

				StartCoroutine(RunInstructions());

			}
			else if (currentState == ExperimentState.inExperiment && !isRunningExperiment) {
				Debug.Log("running experiment");
				StartCoroutine(BeginExperiment());
			}

		}*/
	}

	public IEnumerator RunOutOfTrials(){
		while(environmentMap.IsActive){
			yield return 0; //thus, should wait for the button press before ending the experiment
		}

		cameraController.SetInstructions(); //TODO: might be unecessary? evaluate for oculus...? 
		
		yield return StartCoroutine(ShowSingleInstruction("You have finished your trials! \nPress the button to proceed.", true, true, 0.0f));
		instructionsController.SetInstructionsColorful(); //want to keep a dark screen before transitioning to the end!
		instructionsController.DisplayText("...loading end screen...");
		EndExperiment();

		yield return 0;
	}

	public IEnumerator RunInstructions(){
		isRunningInstructions = true;

		cameraController.SetInstructions();

		//instructionsController.RunInstructions ();

		//while (!instructionsController.isFinished) { //wait until instructions parser has finished showing the instructions
		//	yield return 0;
		//}
		yield return StartCoroutine (ShowSingleInstruction (Config_CoinTask.initialInstructions, true, true, Config_CoinTask.minInitialInstructionsTime));

		currentState = ExperimentState.inExperiment;
		isRunningInstructions = false;

		yield return 0;

	}


	public IEnumerator BeginExperiment(){
		isRunningExperiment = true;
		
		//in case instructions are still on... should perhaps make this it's own function.
		instructionsController.TurnOffInstructions ();
		cameraController.SetInGame();

		yield return StartCoroutine (WaitForActionButton ()); //explore the environment until this button press

		yield return StartCoroutine(trialController.RunExperiment());

		//TODO: should take this out. check that player doesn't get stuck moving forward when experiment ends.
		/*while (true) {
			yield return 0;
		}*/
		
		yield return StartCoroutine(RunOutOfTrials()); //calls EndExperiment()

		yield return 0;

	}

	public void EndExperiment(){
		Debug.Log ("Experiment Over");
		currentState = ExperimentState.inExperimentOver;
		isRunningExperiment = false;
		
		SceneController.Instance.LoadEndMenu();
	}


	public IEnumerator ShowSingleInstruction(string line, bool isDark, bool waitForButton, float minDisplayTimeSeconds){
		if(isDark){
			instructionsController.SetInstructionsColorful();
		}
		else{
			instructionsController.SetInstructionsTransparentOverlay();
		}
		cameraController.SetInstructions();
		instructionsController.DisplayText(line);

		yield return new WaitForSeconds (minDisplayTimeSeconds);

		if (waitForButton) {
			yield return StartCoroutine (WaitForActionButton ());
		}

		instructionsController.TurnOffInstructions ();
		cameraController.SetInGame();
	}
	
	public IEnumerator WaitForActionButton(){
		bool hasPressedButton = false;
		while(Input.GetAxis("ActionButton") != 0f){
			yield return 0;
		}
		while(!hasPressedButton){
			if(Input.GetAxis("ActionButton") == 1.0f){
				hasPressedButton = true;
			}
			yield return 0;
		}
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
