using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class ExperimentSettings_CoinTask : MonoBehaviour { //should be in main menu AND experiment


	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }



	private static Subject _currentSubject;

	public static Subject currentSubject{ 
		get{ return _currentSubject; } 
		set{ 
			_currentSubject = value;
		}
	}



	//subject selection controller
	public SubjectSelectionController subjectSelectionController;

	//TOGGLES
	public static bool isOculus = false;
	public static bool isReplay = false;
	public static bool isLogging = true; //if not in replay mode, should log things! or can be toggled off in main menu.

	public Toggle oculusToggle; //only exists in main menu -- make sure to null check
	public Toggle loggingToggle; //only exists in main menu -- make sure to null check

	//EEG, STIM/SYNC TOGGLES
	public static bool isSystem2 = false;
	public static bool isSyncbox = false;

	public Toggle system2Toggle;
	public Toggle syncboxToggle;


	public InputField NumTreasureChestsInputField; //Frames Per Second

	public GameObject nonPilotOptions;
	public bool isPilot { get { return GetIsPilot (); } }


	public static string defaultLoggingPath = "/Users/" + System.Environment.UserName + "/RAM_2.0/TH1/";
	public Text defaultLoggingPathDisplay;
	public InputField loggingPathInputField;

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

		if(Directory.Exists(defaultLoggingPath)){
			defaultLoggingPathDisplay.text = defaultLoggingPath;
		}
		else{
			defaultLoggingPath = "TextFiles/";
			defaultLoggingPathDisplay.text = defaultLoggingPath;
		}

	}
	// Use this for initialization
	void Start () {
		SetOculus();
		//SetSystem2();
		//SetSyncBox();
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

	public void ChangeLoggingPath(){
		if (Directory.Exists (loggingPathInputField.text)) {
			defaultLoggingPath = loggingPathInputField.text;
		}

		defaultLoggingPathDisplay.text = defaultLoggingPath;
	}

	public void SetReplayTrue(){
		isReplay = true;
		isLogging = false;
		loggingToggle.isOn = false;
	}

	//TODO: use this.
	public static bool isOneByOneReveal = true;

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

	public void SetOculus(){
		if(oculusToggle){
			isOculus = oculusToggle.isOn;
		}
	}

	public void SetSystem2(){
		if(system2Toggle){
			isSystem2 = system2Toggle.isOn;
		}
	}

	public void SetSyncBox(){
		if(syncboxToggle){
			isSyncbox = syncboxToggle.isOn;
		}
	}
	
}
