using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ExperimentSettings_CoinTask : MonoBehaviour { //should be in main menu AND experiment


	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }



	private static Subject _currentSubject;

	public static Subject currentSubject{ 
		get{ return _currentSubject; } 
		set{ 
			_currentSubject = value;
			//fileName = "TextFiles/" + _currentSubject.name + "Log.txt";
		}
	}



	//subject selection controller
	public SubjectSelectionController subjectSelectionController;

	//TOGGLES
	public static bool isOculus = false;
	public static bool isJoystickInput = true;
	public static bool isReplay = false;
	public static bool isLogging = true; //if not in replay mode, should log things! or can be toggled off in main menu.

	public Toggle oculusToggle; //only exists in main menu -- make sure to null check
	public Toggle joystickInputToggle; //only exists in main menu -- make sure to null check
	public Toggle loggingToggle; //only exists in main menu -- make sure to null check

	public Toggle aiToggle;
	public Toggle aiiToggle;
	public Toggle biToggle;
	public Toggle biiToggle;



	//TODO: will probably want to refactor this later, as practice difficulty etc. are used in Trial.cs and TrialController.cs
	//difficulty setting system needs to be decided on!
	public static Trial.DifficultySetting difficultySetting = Trial.DifficultySetting.easy;


	public InputField NumTreasureChestsInputField; //Frames Per Second


	public Text endCongratsText;
	public Text endScoreText;
	public Text endSessionText;


	public GameObject nonPilotOptions;
	public bool isPilot { get { return GetIsPilot (); } }



	//SINGLETON
	private static ExperimentSettings_CoinTask _instance;
	
	public static ExperimentSettings_CoinTask Instance{
		get{
			return _instance;
		}
	}
	
	void Awake(){
		
		if (_instance != null) {
			Debug.Log("Instance already exists!");
			Destroy(transform.gameObject);
			return;
		}
		_instance = this;
	}
	// Use this for initialization
	void Start () {
		SetOculus();
		if(Application.loadedLevelName == "EndMenu"){
			if(currentSubject != null){
				endCongratsText.text = "Congratulations " + currentSubject.name + "!";
				endScoreText.text = currentSubject.score.ToString();
				endSessionText.text = currentSubject.trials.ToString();
			}
			else{
				Debug.Log("Current subject is null!");
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (NumTreasureChestsInputField != null) {
			//UpdateNumTreasureChests ();
		}
	}

	bool GetIsPilot(){
		if (nonPilotOptions.activeSelf) {
			return false;
		}
		return true;
	}

	//set the number of treasure chests/default objects
	/*void UpdateNumTreasureChests(){
		int inputNum = int.Parse (NumTreasureChestsInputField.text);
		Config_CoinTask.numDefaultObjects = inputNum;
	}*/

	public void ChangeObjectMode(){
		if(ObjectController.objectMode == ObjectController.ObjectMode.bii){
			ObjectController.objectMode = ObjectController.ObjectMode.ai;
			aiToggle.isOn = true;
			aiiToggle.isOn = false;
			biToggle.isOn = false;
			biiToggle.isOn = false;
		}
		else if (ObjectController.objectMode == ObjectController.ObjectMode.ai){
			ObjectController.objectMode = ObjectController.ObjectMode.aii;
			aiToggle.isOn = false;
			aiiToggle.isOn = true;
			biToggle.isOn = false;
			biiToggle.isOn = false;
		}
		else if(ObjectController.objectMode == ObjectController.ObjectMode.aii){
			ObjectController.objectMode = ObjectController.ObjectMode.bi;
			aiToggle.isOn = false;
			aiiToggle.isOn = false;
			biToggle.isOn = true;
			biiToggle.isOn = false;
		}
		else if(ObjectController.objectMode == ObjectController.ObjectMode.bi){
			ObjectController.objectMode = ObjectController.ObjectMode.bii;
			aiToggle.isOn = false;
			aiiToggle.isOn = false;
			biToggle.isOn = false;
			biiToggle.isOn = true;
		}

		Debug.Log ("DIFFICULTY MODE: " + ObjectController.objectMode);
	}

	public void SetReplayTrue(){
		isReplay = true;
		isLogging = false;
		loggingToggle.isOn = false;
	}

	//TODO: use this.
	public static bool isOneByOneReveal = true;
	public Toggle treasureRevealToggle;
	public void SetTreasureRevealMode(){
		isOneByOneReveal = treasureRevealToggle.isOn;
	}

	public void SetReplayFalse(){
		isReplay = false;
		//shouldLog = true;
	}

	public void SetLogging(){
		if(isReplay){
			isLogging = false;
		}
		else{
			if(loggingToggle){
				isLogging = loggingToggle.isOn;
				Debug.Log("should log?: " + isLogging);
			}
		}

	}

	public void SetJoystickInput(){
		if (joystickInputToggle) {
			isJoystickInput = joystickInputToggle.isOn;
		}
	}

	public void SetOculus(){
		if(oculusToggle){
			isOculus = oculusToggle.isOn;
		}
	}
	
}
