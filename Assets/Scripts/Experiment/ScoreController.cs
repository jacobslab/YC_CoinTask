using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {
	
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public int score = 0;
	public Text scoreText;

	//SCORE VARIABLES
	int timeBonusSmall = 10;
	int timeBonusMed = 15;
	int timeBonusBig = 20;
	
	/*int scorePerfect = 50;
	int scoreClose = 30;
	int scoreFar = 15;
	int scoreWrong = 0;*/
	int memoryScoreBest = 50;
	int memoryScoreMedium = 30;

	int defaultObjectPoints = 1;
	int specialObjectPoints = 10;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void AddToScore(int amountToAdd){
		score += amountToAdd;
		ExperimentSettings_CoinTask.currentSubject.score = score;
		UpdateScoreText ();
	}

	void UpdateScoreText(){
		scoreText.text = "SCORE: " + score;
	}

	public void AddDefaultPoints(){
		AddToScore(defaultObjectPoints);
	}

	public void AddSpecialPoints(){
		AddToScore(specialObjectPoints);
	}

	//small radius is more impressive, more points
	public int AddMemoryPointsSmallRadius(){
		AddToScore (memoryScoreBest);
		return memoryScoreBest;
	}

	//big radius is less impressive, fewer points
	public int AddMemoryPointsLargeRadius(){
		AddToScore (memoryScoreMedium);
		return memoryScoreMedium;
	}

	public int CalculateMemoryPoints (Vector3 correctPosition){
		if (exp.environmentController.myPositionSelector.GetRadiusOverlap (correctPosition)) {
			if(exp.environmentController.myPositionSelector.currentRadiusType == EnvironmentPositionSelector.SelectionRadiusType.small){
				AddToScore(memoryScoreBest);
				return memoryScoreBest;
			}
			else{
				AddToScore(memoryScoreMedium);
				return memoryScoreMedium;
			}
		}

		return 0;
	}

	/*public int CalculateMemoryPoints(Vector2 correctGridIndices, Vector2 chosenGridIndices){
		int xDiff = (int) Mathf.Abs (correctGridIndices.x - chosenGridIndices.x);
		int yDiff = (int) Mathf.Abs (correctGridIndices.y - chosenGridIndices.y);

		if (xDiff == 0 && yDiff == 0) {
			AddToScore(scorePerfect);
			return scorePerfect;
		}
		else if ( xDiff <= 1 && yDiff <= 1) {
			AddToScore(scoreClose);
			return scoreClose;
		}
		else if (xDiff <= 2 && yDiff <= 2) {
			AddToScore(scoreFar);
			return scoreFar;
		}
		else{
			return scoreWrong;
		}
	}*/

	public int CalculateTimeBonus(float secondsToCompleteTrial){
		if (secondsToCompleteTrial < 40) {
			AddToScore(timeBonusBig);
			return timeBonusBig;
		} 
		else if (secondsToCompleteTrial < 60) {
			AddToScore(timeBonusMed);
			return timeBonusMed;
		} 
		else if (secondsToCompleteTrial < 80) {
			AddToScore(timeBonusSmall);
			return timeBonusSmall;
		} 

		return 0;

	}

	/*public int CalculatePoints(GameObject desiredObject){
		Vector2 avatarXYPos = new Vector2( exp.player.transform.position.x, exp.player.transform.position.z);
		Vector2 objectXYPos = new Vector2( desiredObject.transform.position.x, desiredObject.transform.position.z);

		float distanceFromObject = (avatarXYPos - objectXYPos).magnitude;

		//calculate point ranges based on the visual circles from the map
		float minRange = exp.environmentMap.SmallScoreRing.GetComponent<Renderer>().bounds.extents.x;
		//Debug.Log("min range: " + minRange);
		float midRange = exp.environmentMap.MediumScoreRing.GetComponent<Renderer>().bounds.extents.x;
		//Debug.Log("mid range: " + midRange);
		float maxRange = exp.environmentMap.BigScoreRing.GetComponent<Renderer>().bounds.extents.x;
		//Debug.Log("max range: " + maxRange);

		if(distanceFromObject < minRange){
			score += 100;
			return 100;
		}
		else if(distanceFromObject < midRange){
			score += 50;
			return 50;
		}
		else if(distanceFromObject < maxRange){
			score += 25;
			return 25;
		}
		else{
			score += 10;
			return 10;
		}
	}*/
}
