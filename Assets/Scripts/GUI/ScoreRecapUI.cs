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

	// Use this for initialization
	void Start () {
		Enable (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//TODO: play this from the trial controller!
	public void Play(int currentTrialIndex, int currentTrialScore, int maxNumTrials, List<SpawnableObject> objectFoundVisuals, List<int> objectScores, int timeBonus, float time){
		Enable (true);

		ResetText ();

		if (objectScores.Count > ObjectLocationScores.Length) {
			Debug.Log ("TOO MANY OBJECTS WERE FOUND. NOT ENOUGH TEXT MESHES.");
		}
		else {
			int trialScore = 0;
			for (int i = 0; i < objectScores.Count; i++) {
				//put objects in the right places
				objectFoundVisuals [i].gameObject.transform.position = ObjectLocationScores [i].transform.position + ObjectVisualOffset;

				//set object score text & object names
				ObjectLocationScores [i].text = objectScores [i].ToString ();
				ObjectNames [i].text = objectFoundVisuals [i].GetName () + ":";

				trialScore += objectScores [i];
			}

			TimeBonusLabel.text = time + "seconds:";
			TimeBonusText.text = timeBonus.ToString ();

			int objectPickupScore = ObjectLocationScores.Length * ScoreController.SpecialObjectPoints;
			ObjectPickupScore.text = objectPickupScore.ToString();
			ObjectPickupScoreLabel.text = "Objects x" + ObjectLocationScores.Length + ":";

			TrialNumText.text = "trial " + (currentTrialIndex + 1) + "/" + maxNumTrials + " completed";

			TotalTrialScoreText.text = (trialScore + timeBonus + objectPickupScore).ToString();

		}

	}

	public void Stop(){
		Enable (false);
	}

	void ResetText(){
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
