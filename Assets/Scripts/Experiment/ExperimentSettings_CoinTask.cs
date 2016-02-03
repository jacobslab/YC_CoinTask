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


	public InputField NumTreasureChestsInputField; //Frames Per Second

	public GameObject nonPilotOptions;
	public bool isPilot { get { return GetIsPilot (); } }


	public static string defaultLoggingPath = "/Users/" + System.Environment.UserName + "/RAM_2.0/data";
	string TH1Folder = "/TH1/";
	string TH2Folder = "/TH2/";
	string TH3Folder = "/TH3/";
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

		InitLoggingPath ();
		InitMainMenuLabels ();

	}

	void InitLoggingPath(){

		if(Directory.Exists(defaultLoggingPath)){
			if (Config_CoinTask.BuildVersion == Config_CoinTask.Version.TH1) {
				defaultLoggingPath += TH1Folder;
			} 
			else if (Config_CoinTask.BuildVersion == Config_CoinTask.Version.TH2) {
				defaultLoggingPath += TH2Folder;
			}
			else if (Config_CoinTask.BuildVersion == Config_CoinTask.Version.TH3) {
				defaultLoggingPath += TH3Folder;
			}

			if(!Directory.Exists(defaultLoggingPath)){ //if that TH folder doesn't exist, make it!
				Directory.CreateDirectory(defaultLoggingPath);
			}
		}
		else{
			defaultLoggingPath = "TextFiles/";
		}

		defaultLoggingPathDisplay.text = defaultLoggingPath;
	}


	public Text ExpNameVersion;
	public Text BuildType;
	void InitMainMenuLabels(){
		ExpNameVersion.text = Config_CoinTask.BuildVersion.ToString () + "/" + Config_CoinTask.VersionNumber;
		if (Config_CoinTask.isSyncbox) {
			BuildType.text = "Sync Box";
		}
		else if (Config_CoinTask.isSystem2){
			BuildType.text = "System 2";
		}
		else{
			BuildType.text = "Demo";
		}
	}

	// Use this for initialization
	void Start () {
		SetOculus();
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
		if (oculusToggle) {
			isOculus = oculusToggle.isOn;
		}
	}
	
}
