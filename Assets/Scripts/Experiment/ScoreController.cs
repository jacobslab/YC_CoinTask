using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {
	
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public int score = 0;
	public Text scoreText;
	public ScoreLogTrack scoreLogger;
	public TimeBonusUIController timeBonusUI;

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

	static int memoryScoreWrong = -30;
	public static int MemoryScoreWrong { get { return memoryScoreWrong; } }

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

	void AddToScore(int amountToAdd, bool isTimeBonus){
		StartCoroutine (UpdateScoreText(score, amountToAdd, isTimeBonus)); //pass in the original score, before adding the amount

		score += amountToAdd;
		ExperimentSettings_CoinTask.currentSubject.score = score;
	}

	IEnumerator UpdateScoreText(int initialScore, int amountToAdd, bool isTimeBonus){
		float timeBetweenUpdates = 0.05f;

		if (isTimeBonus) {
			yield return StartCoroutine( timeBonusUI.MoveBonusText () );
		}

		int increment = 1;
		if (amountToAdd < 0) {
			increment = -1;
		}

		amountToAdd = Mathf.Abs (amountToAdd);
		while (amountToAdd > 0) {

			initialScore += increment;
			scoreText.text = "$ " + (initialScore);
			amountToAdd--;
			yield return new WaitForSeconds(timeBetweenUpdates);
		}

		yield return 0;
	}

	public void AddDefaultPoints(){
		AddToScore(defaultObjectPoints, false);
		scoreLogger.LogTreasureOpenScoreAdded (defaultObjectPoints);
	}

	public void AddSpecialPoints(){
		AddToScore(specialObjectPoints, false);
		scoreLogger.LogTreasureOpenScoreAdded (specialObjectPoints);
	}

	public int CalculateMemoryPoints (Vector3 correctPosition, bool doubledDown){
		int memoryPoints = 0;
		if (exp.environmentController.myPositionSelector.GetRadiusOverlap (correctPosition)) {
			if(exp.environmentController.myPositionSelector.currentRadiusType == EnvironmentPositionSelector.SelectionRadiusType.small){
				memoryPoints = memoryScoreBest;
			}
			else if(exp.environmentController.myPositionSelector.currentRadiusType == EnvironmentPositionSelector.SelectionRadiusType.big){
				memoryPoints =  memoryScoreMedium;
			}
		}
		else if(exp.environmentController.myPositionSelector.currentRadiusType == EnvironmentPositionSelector.SelectionRadiusType.none){
			memoryPoints = memoryScoreNoChoice;
		}
		else{ //wrong
			memoryPoints = memoryScoreWrong;
		}

		if (doubledDown) {
			memoryPoints *= 2; //double the score if we doubled down!
		}

		AddToScore(memoryPoints, false);
		scoreLogger.LogMemoryScoreAdded (memoryPoints);

		return memoryPoints;
	}

	public int CalculateTimeBonus(float secondsToCompleteTrial){
		int timeBonusScore = 0;
		if (secondsToCompleteTrial < timeBonusTimeMin) {
			timeBonusScore = timeBonusBig;
		} 
		else if (secondsToCompleteTrial < timeBonusTimeMed) {
			timeBonusScore = timeBonusMed;
		} 
		else if (secondsToCompleteTrial < timeBonusTimeMax) {
			timeBonusScore = timeBonusSmall;
		} 

		AddToScore (timeBonusScore, true);
		scoreLogger.LogTimeBonusAdded (timeBonusScore);

		return timeBonusScore;

	}

}
