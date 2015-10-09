using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {
	
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public ScoreLogTrack scoreLogger;

	public int score = 0;
	public Text scoreText;
	int scoreTextScore = 0;
	int amountLeftToAdd = 0; //amount left to add to the score text

	//SCORE VARIABLES -- don't want anyone to change them, so make public getters, no setters.
	static int timeBonusSmall = 10;
	public static int TimeBonusSmall { get { return timeBonusSmall; } }

	static int timeBonusMed = 20;
	public static int TimeBonusMed { get { return timeBonusMed; } }

	static int timeBonusBig = 30;
	public static int TimeBonusBig { get { return timeBonusBig; } }
	

	static int memoryScoreRight = 50;
	public static int MemoryScoreRight { get { return memoryScoreRight; } }

	static int memoryScoreWrong = -16;
	public static int MemoryScoreWrong { get { return memoryScoreWrong; } }

	static int memoryScoreDoubleDown = 150;
	public static int MemoryScoreDoubleDown { get { return memoryScoreDoubleDown; } }

	static int memoryScoreWrongDoubleDown = -116;
	public static int MemoryScoreWrongDoubleDown { get { return memoryScoreWrongDoubleDown; } }

	int defaultObjectPoints = 0;
	int specialObjectPoints = 50;




	//Time bonus time variables!
	static int timeBonusTimeMin = 30;
	public static int TimeBonusTimeMin { get { return timeBonusTimeMin; } }
	
	static int timeBonusTimeMed = 45;
	public static int TimeBonusTimeMed { get { return timeBonusTimeMed; } }
	
	static int timeBonusTimeMax = 60;
	public static int TimeBonusTimeBig { get { return timeBonusTimeMax; } }



	// Use this for initialization
	void Start () {
		StartCoroutine (UpdateScoreText());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void AddToScore(int amountToAdd){
		amountLeftToAdd += amountToAdd;
		score += amountToAdd;
		ExperimentSettings_CoinTask.currentSubject.score = score;
	}

	IEnumerator UpdateScoreText(){
		float timeBetweenUpdates = 0.05f;

		int increment = 1;

		while (true) {

			int absAmountLeftToAdd = Mathf.Abs (amountLeftToAdd);
			while (absAmountLeftToAdd > 0) {

				if (amountLeftToAdd < 0) {
					increment = -1;
				}
				else{
					increment = 1;
				}

				scoreTextScore += increment;
				scoreText.text = "$ " + (scoreTextScore);
				amountLeftToAdd -= increment;

				absAmountLeftToAdd = Mathf.Abs (amountLeftToAdd);

				yield return new WaitForSeconds (timeBetweenUpdates);
			}

			yield return 0;

		}
	}

	public void AddDefaultPoints(){
		AddToScore(defaultObjectPoints);
		scoreLogger.LogTreasureOpenScoreAdded (defaultObjectPoints);
	}

	public void AddSpecialPoints(){
		AddToScore(specialObjectPoints);
		scoreLogger.LogTreasureOpenScoreAdded (specialObjectPoints);
	}

	public int CalculateMemoryPoints (Vector3 correctPosition, bool doubledDown){
		int memoryPoints = 0;
		if (exp.environmentController.myPositionSelector.GetRadiusOverlap (correctPosition)) {
			if(!doubledDown){
				memoryPoints = memoryScoreRight;
			}
			else{
				memoryPoints = memoryScoreDoubleDown;
			}
		}
		else{ //wrong
			if(!doubledDown){
				memoryPoints = memoryScoreWrong;
			}
			else{
				memoryPoints = memoryScoreWrongDoubleDown;
			}
		}

		AddToScore(memoryPoints);
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

		AddToScore (timeBonusScore);
		scoreLogger.LogTimeBonusAdded (timeBonusScore);

		return timeBonusScore;

	}

	public void Reset(){
		score = 0;
		scoreTextScore = 0;
		amountLeftToAdd = 0;
		scoreText.text = "$ " + 0;

		scoreLogger.LogScoreReset ();
	}

}
