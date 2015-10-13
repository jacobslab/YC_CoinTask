using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreRecapUI : MonoBehaviour {
	
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } } 

	public TextMesh[] ObjectLocationScores;
	public TextMesh[] ObjectNames;
	public TextMesh ObjectPickupScore;
	public TextMesh ObjectPickupScoreLabel;
	public Vector3 ObjectVisualOffset;
	public TextMesh TimeBonusText;
	public TextMesh TimeBonusLabel;
	public TextMesh TotalTrialScoreText;
	public TextMesh TrialNumText;

	public Transform CentralContent;

	Vector3 centralContentOrigPos;

	// Use this for initialization
	void Start () {
		Enable (false);
		centralContentOrigPos = CentralContent.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Play(int numTrialsComplete, int currentTrialScore, int maxNumTrials, List<int> objectScores, List<GameObject> specialObjects, int timeBonus, float time){
		Enable (true);

		Reset();

		if (objectScores.Count > ObjectLocationScores.Length) {
			Debug.Log ("TOO MANY OBJECTS WERE FOUND. NOT ENOUGH TEXT MESHES.");
		}
		else {
			int trialScore = 0;
			for (int i = 0; i < objectScores.Count; i++) {
				//put objects in the right places
				//objectFoundVisuals [i].gameObject.transform.position = ObjectLocationScores [i].transform.position + ObjectVisualOffset;

				//set object score text & object names
				string currObjectScore = FormatScore(objectScores[i]);
				ObjectLocationScores [ObjectLocationScores.Length - 1 - i].text = currObjectScore;
				ObjectNames [ObjectNames.Length - 1 - i].text = specialObjects [i].GetComponent<SpawnableObject>().GetName () + ":";

				trialScore += objectScores [i];
			}

			//adjust positioning of the central content based on how many objects there were. or weren't.
			if(ObjectLocationScores.Length > 2){
			float distanceBetweenObjectText = ObjectLocationScores[0].transform.position.y - ObjectLocationScores[1].transform.position.y;
				int spaceToMoveMult = ObjectLocationScores.Length - objectScores.Count;
				CentralContent.transform.position += Vector3.up * ( Mathf.Abs(distanceBetweenObjectText) * spaceToMoveMult );
			}

			TimeBonusLabel.text = time.ToString("0.00") + " seconds:"; //the "0.00" parameter should format it to a two decimal place number
			TimeBonusText.text = FormatScore(timeBonus);

			int objectPickupScore = objectScores.Count * ScoreController.SpecialObjectPoints;
			ObjectPickupScore.text = FormatScore(objectPickupScore);
			ObjectPickupScoreLabel.text = "Objects x" + objectScores.Count + ":";

			TrialNumText.text = "trial " + (numTrialsComplete) + "/" + maxNumTrials + " completed";

			TotalTrialScoreText.text = FormatScore(trialScore + timeBonus + objectPickupScore);

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
		CentralContent.position = centralContentOrigPos;

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
