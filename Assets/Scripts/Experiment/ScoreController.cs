using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {
	
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public int score = 0;
	public Text scoreText;

	//SCORE VARIABLES -- don't want anyone to change them, so make public getters, no setters.
	static int timeBonusSmall = 10;
	public static int TimeBonusSmall { get { return timeBonusSmall; } }

	static int timeBonusMed = 15;
	public static int TimeBonusMed { get { return timeBonusMed; } }

	static int timeBonusBig = 20;
	public static int TimeBonusBig { get { return timeBonusBig; } }
	

	static int memoryScoreBest = 50;
	public static int MemoryScoreBest { get { return memoryScoreBest; } }

	static int memoryScoreMedium = 30;
	public static int MemoryScoreMedium { get { return memoryScoreMedium; } }

	static int memoryScoreNoChoice = 10;
	public static int MemoryScoreNoChoice { get { return memoryScoreNoChoice; } }

	int defaultObjectPoints = 1;
	int specialObjectPoints = 10;




	//Time bonus time variables!
	static int timeBonusTimeMin = 30;
	public static int TimeBonusTimeSmall { get { return timeBonusTimeMin; } }
	
	static int timeBonusTimeMed = 45;
	public static int TimeBonusTimeMed { get { return timeBonusTimeMed; } }
	
	static int timeBonusTimeMax = 60;
	public static int TimeBonusTimeBig { get { return timeBonusTimeMax; } }



	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void AddToScore(int amountToAdd){
		StartCoroutine (UpdateScoreText(score, amountToAdd)); //pass in the original score, before adding the amount

		score += amountToAdd;
		ExperimentSettings_CoinTask.currentSubject.score = score;
		//UpdateScoreText ();
	}

	IEnumerator UpdateScoreText(int initialScore, int amountToAdd){
		float timeBetweenUpdates = 0.05f;

		while (amountToAdd > 0) {
			initialScore++;
			scoreText.text = "SCORE: " + (initialScore);
			amountToAdd--;
			yield return new WaitForSeconds(timeBetweenUpdates);
		}

		yield return 0;
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
			else if(exp.environmentController.myPositionSelector.currentRadiusType == EnvironmentPositionSelector.SelectionRadiusType.big){
				AddToScore(memoryScoreMedium);
				return memoryScoreMedium;
			}
		}
		else if(exp.environmentController.myPositionSelector.currentRadiusType == EnvironmentPositionSelector.SelectionRadiusType.none){
			AddToScore(memoryScoreNoChoice);
			return memoryScoreNoChoice;
		}
		

		return 0;
	}

	public int CalculateTimeBonus(float secondsToCompleteTrial){
		if (secondsToCompleteTrial < timeBonusTimeMin) {
			AddToScore(timeBonusBig);
			return timeBonusBig;
		} 
		else if (secondsToCompleteTrial < timeBonusTimeMed) {
			AddToScore(timeBonusMed);
			return timeBonusMed;
		} 
		else if (secondsToCompleteTrial < timeBonusTimeMax) {
			AddToScore(timeBonusSmall);
			return timeBonusSmall;
		} 

		return 0;

	}

}
