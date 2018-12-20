using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreRecapUI : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } } 


	public ParticleSystem funParticles;
	public TextMesh[] ObjectLocationScores;
	public TextMesh[] ObjectNames;
	public Vector3 ObjectVisualOffset;
	public TextMesh TimeBonusText;
	public TextMesh TemporalScoreText;
	public TextMesh BoxSwapText;
	public TextMesh TotalTrialScoreText;
	public TextMesh TrialNumText;

	public Transform ObjectScoreContent; //this may have to be moved/realigned depending on how many objects were in the trial

	Vector3 centralContentOrigPos;

	// Use this for initialization
	void Start () {
		Enable (false);
		centralContentOrigPos = ObjectScoreContent.localPosition;//ObjectScoreContent.position;
	}

	// Update is called once per frame
	void Update () {

	}

	public void SetBoxSwapBonusText(int boxSwapBonus){
		BoxSwapText.text = FormatScore(boxSwapBonus);
	}

	public void Play(int numTrialsComplete, int currentTrialScore, int maxNumTrials, List<int> objectScores, List<GameObject> specialObjects, int pathIntegrationBonus, int temporalScore){
		Enable (true);

		Reset();

		PlayJuice ();

		if (objectScores.Count > ObjectLocationScores.Length) {
			Debug.Log ("TOO MANY OBJECTS WERE FOUND. NOT ENOUGH TEXT MESHES.");
		}
		else {
			int trialScore = 0;
			for (int i = 0; i < objectScores.Count; i++) {

				//set object score text & object names
				//string currObjectScore = FormatScore(objectScores[i]);
				//ObjectLocationScores [ObjectLocationScores.Length - 1 - i].text = currObjectScore;
				//ObjectNames [ObjectNames.Length - 1 - i].text = specialObjects [i].GetComponent<SpawnableObject>().GetDisplayName () + ":";

				trialScore += objectScores [i];
			}

			//adjust positioning of the central content based on how many objects there were. or weren't.
			//if(ObjectLocationScores.Length > 2){
			//	float distanceBetweenObjectText = ObjectLocationScores[0].transform.position.y - ObjectLocationScores[1].transform.position.y;
			//	int spaceToMoveMult = ObjectLocationScores.Length - objectScores.Count;
			//	ObjectScoreContent.transform.position += Vector3.up * ( Mathf.Abs(distanceBetweenObjectText) * spaceToMoveMult );
			//}

			//TimeBonusText.text = FormatScore(timeBonus);

			//TemporalScoreText.text = FormatScore (temporalScore);


            TrialNumText.text = "Sie haben " +(numTrialsComplete)+ " von " + maxNumTrials.ToString() + " Runden absolviert.";

            TotalTrialScoreText.text = FormatScore(trialScore + pathIntegrationBonus);

		}

	}

	void PlayJuice(){
		if (Config_CoinTask.isJuice) {
			JuiceController.PlayParticles (funParticles);
		}
	}

	public void Stop(){
		Enable (false);
	}

	string FormatScore(int score){
		string scoreText = score.ToString ();
		if(score > 0){
			scoreText = "+" + scoreText;
		}

		return scoreText;
	}

	void Reset(){
		//ObjectScoreContent.position = centralContentOrigPos;
		ObjectScoreContent.localPosition = centralContentOrigPos;

		for (int i = 0; i < ObjectLocationScores.Length; i++) {
			ObjectLocationScores[i].text = "";
			ObjectNames[i].text = "";
		}

	}

	void Enable(bool shouldEnable){
		GetComponent<EnableChildrenLogTrack>().LogChildrenEnabled(shouldEnable);

		UsefulFunctions.EnableChildren( transform, shouldEnable );
	}
}
