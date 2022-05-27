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
		//Application.LoadLevel(3);
	}


	// Use this for initialization
	void Start () {

		DontDestroyOnLoad (this.gameObject);
		
	}


	// Update is called once per frame
	void Update () {

	}

	public void LoadWorthlessValuable()
	{
		//SceneManager.LoadScene("WorthlessValQues", LoadSceneMode.Single);
		Application.LoadLevel(8);
	}

	public void LoadPessOpt()
	{
		//SceneManager.LoadScene("WorthlessValQues", LoadSceneMode.Single);
		Application.LoadLevel(4);
	}

	public void LoadApaMot()
	{
		//SceneManager.LoadScene("WorthlessValQues", LoadSceneMode.Single);
		Application.LoadLevel(5);
	}
	public void LoadGuilProu()
	{
		//SceneManager.LoadScene("WorthlessValQues", LoadSceneMode.Single);
		Application.LoadLevel(6);
	}
	public void LoadNumbInter()
	{
		//SceneManager.LoadScene("WorthlessValQues", LoadSceneMode.Single);
		Application.LoadLevel(7);
	}

	public void LoadWithdrawnWelcoming()
	{
		//SceneManager.LoadScene("WorthlessValQues", LoadSceneMode.Single);
		Application.LoadLevel(9);
	}
	public void LoadHopelessful()
	{
		//SceneManager.LoadScene("WorthlessValQues", LoadSceneMode.Single);
		Application.LoadLevel(10);
	}
	public void LoadTenseRel()
	{
		//SceneManager.LoadScene("WorthlessValQues", LoadSceneMode.Single);
		Application.LoadLevel(11);
	}
	public void LoadWorriedUnt()
	{
		//SceneManager.LoadScene("WorthlessValQues", LoadSceneMode.Single);
		Application.LoadLevel(12);
	}
	public void LoadFearfulless()
	{
		//SceneManager.LoadScene("WorthlessValQues", LoadSceneMode.Single);
		Application.LoadLevel(13);
	}
	public void LoadAnxiousPeaceful()
	{
		//SceneManager.LoadScene("WorthlessValQues", LoadSceneMode.Single);
		Application.LoadLevel(14);
	}
	public void LoadRestlessCalm()
	{
		//SceneManager.LoadScene("WorthlessValQues", LoadSceneMode.Single);
		Application.LoadLevel(15);
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
			Application.LoadLevel (1);
		}
	}

	void LoadExperimentLevel(){
		if (ExperimentSettings_CoinTask.currentSubject.trials < Config_CoinTask.GetTotalNumTrials ()) {
			Debug.Log ("loading experiment!");
			Application.LoadLevel (1);
		} else {
			Debug.Log ("Subject has already finished all blocks! Loading end menu.");
			Application.LoadLevel (2);
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
