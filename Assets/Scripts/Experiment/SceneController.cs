using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour { //there can be a separate scene controller in each scene


	//SINGLETON
	private static SceneController _instance;
	
	public static SceneController Instance{
		get{
			return _instance;
		}
	}
	
	void Awake(){
//		if (_instance != null) {
//			Debug.Log("Instance already exists!");
//			Destroy(transform.gameObject);
//			return;
//		}
		_instance = this;
	}


	// Use this for initialization
	void Start () {

		DontDestroyOnLoad (this.gameObject);

	}


	// Update is called once per frame
	void Update () {

	}

	public void LoadMainMenu(){
		if(Experiment_CoinTask.Instance != null){
			Experiment_CoinTask.Instance.OnExit();
		}

		Debug.Log("loading main menu!");
		SubjectReaderWriter.Instance.RecordSubjects();
		SceneManager.LoadScene(0);
	}

	public void LoadExperiment(){
		//should be no new data to record for the subject
		if(Experiment_CoinTask.Instance != null){
			Experiment_CoinTask.Instance.OnExit();
		}

		if (ExperimentSettings_CoinTask.Instance.isRelease){ //no subject, not replay, is pilot
			ExperimentSettings_CoinTask.Instance.subjectSelectionController.SendMessage("AddNewSubject");
			if(ExperimentSettings_CoinTask.currentSubject != null){
				LoadExperimentLevel();
			}
		}
		
		else if (ExperimentSettings_CoinTask.currentSubject != null && !ExperimentSettings_CoinTask.Instance.isRelease) {
			LoadExperimentLevel();
		} 
		else if (ExperimentSettings_CoinTask.isReplay) {
			Debug.Log ("loading experiment!");
			SceneManager.LoadScene(1);
		}
	}

	void LoadExperimentLevel(){
		if (ExperimentSettings_CoinTask.currentSubject.trials < Config_CoinTask.GetTotalNumTrials ()) {
			Debug.Log ("loading experiment!");
			SceneManager.LoadScene(1);
		} else {
			Debug.Log ("Subject has already finished all blocks! Loading end menu.");
			SceneManager.LoadScene(2);
		}
	}

	public void LoadEndMenu(){
		if(Experiment_CoinTask.Instance != null){
			Experiment_CoinTask.Instance.OnExit();
		}

		SubjectReaderWriter.Instance.RecordSubjects();
		Debug.Log("loading end menu!");
		SceneManager.LoadScene(2);
	}

	public void Quit(){
#if !UNITY_WEBPLAYER
		SubjectReaderWriter.Instance.RecordSubjects();
#endif
		Application.Quit();
	}

	void OnApplicationQuit(){
		Debug.Log("On Application Quit!");
#if !UNITY_WEBPLAYER
		SubjectReaderWriter.Instance.RecordSubjects();
#endif
	}
}
