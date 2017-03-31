using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
public class ExperimentSettings_CoinTask : MonoBehaviour { //should be in main menu AND experiment


	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }



	private static Subject _currentSubject;

	public static Subject currentSubject{ 
		get{ return _currentSubject; } 
		set{ 
			_currentSubject = value;
		}
	}

//	public GameObject quitButton; //must turn this off in web build

	//subject selection controller
	public SubjectSelectionController subjectSelectionController;
	public Image micTestIndicator;


	//LANGUAGE SELECTION
	public enum LanguageSetting
	{
		English,
		Spanish
	}
	#if !SPANISH
	public static LanguageSetting myLanguage = LanguageSetting.English;
	#else
	public static LanguageSetting myLanguage = LanguageSetting.Spanish;
	#endif
	public Dropdown languageDropdown;


	public enum RecallType
	{
		Location,
		Object
	}

	public static RecallType myRecall=RecallType.Object;
	public Dropdown recallType;


	//TOGGLES & SETTINGS
	public static bool isOculus = false;
	public static bool isReplay = false;
	public static bool isLogging = true; //if not in replay mode, should log things! or can be toggled off in main menu.

	public Toggle oculusToggle; //only exists in main menu -- make sure to null check
	public Toggle loggingToggle; //only exists in main menu -- make sure to null check

	public Button returnMenuButton;
	public Button quitButton;

	public GameObject loggingPathOptions; //for easy un-enabling in web version

	public InputField NumTreasureChestsInputField; //Frames Per Second

	public GameObject nonPilotOptions;
	public bool isRelease { get { return GetIsRelease (); } }


	public static string defaultLoggingPath = ""; //SET IN RESETDEFAULTLOGGINGPATH();
	string TH1Folder = "TH1/";
	string TH2Folder = "TH2/";
	string TH3Folder = "TH3/";
	string MRIFolder = "MRI/";
	string MRIPracticeFolder = "MRIPractice/";
	public Text defaultLoggingPathDisplay;
	public InputField loggingPathInputField;
	public Text manualTrialFilePathDisplay;

	//IF YOU WANT MANUAL TRIAL FILE
	public static string manualTrialFilePath; //if you want to define trials & objects manually
	public InputField manualTrialPathInputField;



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
		if (SceneManager.GetActiveScene ().name != "EndMenu") {
#if UNITY_WEBPLAYER
		InitWebSettings ();
#else
			DoMicTest ();
			InitLoggingPath ();
#endif
			InitMainMenuLabels ();
		} else
			AttachSceneController ();

		QualitySettings.vSyncCount = 1;
	}

	void AttachSceneController()
	{
		GameObject sceneControl = GameObject.Find ("SceneController");
		if (sceneControl != null) {
			returnMenuButton.onClick.RemoveAllListeners ();
			returnMenuButton.onClick.AddListener (() => SceneController.Instance.LoadMainMenu ());

			quitButton.onClick.RemoveAllListeners ();
			quitButton.onClick.AddListener (() => SceneController.Instance.Quit ());
		}
	}
	void ResetDefaultLoggingPath(){
		#if (!UNITY_WEBPLAYER)
			#if MRIVERSION
			defaultLoggingPath = System.IO.Directory.GetCurrentDirectory() + "/TextFiles/";
			#else

		if (Config_CoinTask.isSystem2) {
			defaultLoggingPath = "/Users/" + System.Environment.UserName + "/RAM_2.0/data/THR/";
		}else if (Config_CoinTask.isSYS3) {
			defaultLoggingPath = "/Users/" + System.Environment.UserName + "/RAM_3.0/data/THR/";
		} else if (Config_CoinTask.isSyncbox && !Config_CoinTask.isSYS3) {
				defaultLoggingPath = "/Users/" + System.Environment.UserName + "/RAM/data/THR/";
			} else {
			defaultLoggingPath = System.IO.Directory.GetCurrentDirectory() + "/TextFiles/THR/";

			Debug.Log("current dir: " + System.IO.Directory.GetCurrentDirectory());
			}
			#endif
		#endif
	}


	void DoMicTest(){
		if (micTestIndicator != null) {
			if (AudioRecorder.CheckForRecordingDevice ()) {
				micTestIndicator.color = Color.green;
			} else {
				micTestIndicator.color = Color.red;
			}
		}
	}

	void InitLoggingPath(){
		ResetDefaultLoggingPath ();
		Debug.Log ("reset log path: "+ defaultLoggingPath);
		if(!Directory.Exists(defaultLoggingPath)) {
			Directory.CreateDirectory(Path.GetDirectoryName(defaultLoggingPath));
		}

		if (Config_CoinTask.BuildVersion == Config_CoinTask.Version.TH1) {
			//if(Config_CoinTask.isSyncbox || Config_CoinTask.isSystem2){ //only add the folder if it's not the demo version.
				defaultLoggingPath += TH1Folder;
			Debug.Log ("assigned it ");
			//}
		} 
		else if (Config_CoinTask.BuildVersion == Config_CoinTask.Version.TH2) {
			defaultLoggingPath += TH2Folder;
		}
		else if (Config_CoinTask.BuildVersion == Config_CoinTask.Version.TH3) {
			defaultLoggingPath += TH3Folder;
		}
		else if (Config_CoinTask.BuildVersion == Config_CoinTask.Version.MRI){
			if(Config_CoinTask.isPractice){
				defaultLoggingPath += MRIPracticeFolder;
			}
			else{
				defaultLoggingPath += MRIFolder;
			}
		}

		if(!Directory.Exists(defaultLoggingPath)){ //if that TH folder doesn't exist, make it!
			Directory.CreateDirectory(defaultLoggingPath);
			Debug.Log ("created new directory");
		}

		if (defaultLoggingPathDisplay != null) {
			defaultLoggingPathDisplay.text = defaultLoggingPath;
		}
	}


	public Text ExpNameVersion;
	public Text BuildType;
	void InitMainMenuLabels(){
		if (Application.loadedLevel == 0) {
			ExpNameVersion.text = Config_CoinTask.BuildVersion.ToString () + "/" + Config_CoinTask.VersionNumber;
		#if SYS3COMBO
			if (Config_CoinTask.isSyncbox && !Config_CoinTask.isSYS3) {
		#else
			if (Config_CoinTask.isSyncbox) {
		#endif
				BuildType.text = "Sync Box";
			} else if (Config_CoinTask.isSystem2) {
				BuildType.text = "System 2";
			}
			#if SYS3COMBO
			else if (Config_CoinTask.isSYS3 && !Config_CoinTask.isSyncbox) {
			#else 
			else if(Config_CoinTask.isSYS3){
			#endif
				BuildType.text = "SYS3";
			}
			#if SYS3COMBO
			else if (Config_CoinTask.isSYS3 && Config_CoinTask.isSyncbox) {
				BuildType.text = "SYS3 on Syncbox";
			} 
			#endif
			else {
				BuildType.text = "Demo";
			}
			#if UNITY_WEBPLAYER
				BuildType.text = "WebDemo";
			#elif MRIVERSION
				BuildType.text = "MRI";
			#endif
			if(Config_CoinTask.isPractice){
				BuildType.text += " Practice";
			}
		}
	}

	void InitWebSettings(){
		isLogging = false;
		loggingToggle.enabled = false;
		loggingPathOptions.SetActive(false);
		subjectSelectionController.SubjectInputField.gameObject.SetActive(false);
		quitButton.enabled=false;
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

	bool GetIsRelease(){
		if (nonPilotOptions.activeSelf) {
			return false;
		}
		return true;
	}

	public void ChangeLoggingPath(){
		if (Directory.Exists (loggingPathInputField.text)) {
			defaultLoggingPath = loggingPathInputField.text;

			//check if the last character is a slash --> if not, add a slash...
			char[] defaultLoggingPathChars = defaultLoggingPath.ToCharArray();
			int numChars = defaultLoggingPathChars.Length;
			if(defaultLoggingPathChars [ numChars - 1 ] != '/'){
				defaultLoggingPath += "/";
			}
		}

		defaultLoggingPathDisplay.text = defaultLoggingPath;
	}

	public void ChangeManualTrialFilePath(){
		if (File.Exists (manualTrialPathInputField.text)) {
			manualTrialFilePath = manualTrialPathInputField.text;
		}
		else if (manualTrialPathInputField.text == "") {
			manualTrialFilePath = "None";
		}

		manualTrialFilePathDisplay.text = manualTrialFilePath;
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
	public void SetRecallType(){
		Debug.Log ("SETTING Recall Type");
		if (recallType != null) {
			switch (recallType.options [recallType.value].text) {
			case "Location":
				myRecall = RecallType.Location;
				break;
			case "Object":
				myRecall = RecallType.Object;
				break;
			}

			Debug.Log ("CURR RECALL: " + myRecall);
		}
	}
	public void SetLanguage(){
		Debug.Log ("SETTING LANGUAGE");
		if (languageDropdown != null) {
			switch (languageDropdown.options [languageDropdown.value].text) {
			case "English":
				myLanguage = LanguageSetting.English;
				break;
			case "Spanish":
				myLanguage = LanguageSetting.Spanish;
				break;
			}

			Debug.Log ("CURR LANGUAGE: " + myLanguage);
		}
	}
}
