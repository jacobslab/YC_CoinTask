using UnityEngine;
using System.Collections;

public class SceneController : MonoBehaviour { //there can be a separate scene controller in each scene


	//SINGLETON
	private static SceneController _instance;
	
	public static SceneController Instance{
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
		Application.LoadLevel(0);
	}

	public void LoadExperiment(){
		//should be no new data to record for the subject
		if(Experiment_CoinTask.Instance != null){
			Experiment_CoinTask.Instance.OnExit();
		}

		if (ExperimentSettings_CoinTask.currentSubject != null) {
			if (ExperimentSettings_CoinTask.currentSubject.trials < Config_CoinTask.GetTotalNumTrials ()) {
				Debug.Log ("loading experiment!");
				Application.LoadLevel (1);
			} else {
				Debug.Log ("Subject has already finished all blocks! Loading end menu.");
				Application.LoadLevel (2);
			}
		} 
		else if (ExperimentSettings_CoinTask.isReplay) {
			Debug.Log ("loading experiment!");
			Application.LoadLevel (1);
		}
	}

	public void LoadEndMenu(){
		if(Experiment_CoinTask.Instance != null){
			Experiment_CoinTask.Instance.OnExit();
		}

		SubjectReaderWriter.Instance.RecordSubjects();
		Debug.Log("loading end menu!");
		Application.LoadLevel(2);
	}

	public void Quit(){
		SubjectReaderWriter.Instance.RecordSubjects();
		Application.Quit();
	}

	void OnApplicationQuit(){
		Debug.Log("On Application Quit!");
		SubjectReaderWriter.Instance.RecordSubjects();
	}
}
