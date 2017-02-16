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
	
	//Time bonus time variables!
	public static int timeBonusTimeMin = 22;
	public static int TimeBonusTimeMin { get { return timeBonusTimeMin; } }
	
	public static int timeBonusTimeMed = 44;
	public static int TimeBonusTimeMed { get { return timeBonusTimeMed; } }
	
	//static int timeBonusTimeMax = 52;
	//public static int TimeBonusTimeBig { get { return timeBonusTimeMax; } }




	//SCORE VARIABLES -- don't want anyone to change them, so make public getters, no setters.
	//static int timeBonusSmall = 100;
	//public static int TimeBonusSmall { get { return timeBonusSmall; } }

	static int timeBonusMed = 100;
	public static int TimeBonusMed { get { return timeBonusMed; } }

	static int timeBonusBig = 200;
	public static int TimeBonusBig { get { return timeBonusBig; } }
	

	static int memoryScoreMaybeRight = 100;
	public static int MemoryScoreMaybeRight { get { return memoryScoreMaybeRight; } }

	static int memoryScoreMaybeWrong = -50;
	public static int MemoryScoreMaybeWrong { get { return memoryScoreMaybeWrong; } }

	static int memoryScoreNoRight = 50;
	public static int MemoryScoreNoRight { get { return memoryScoreNoRight; } }
	
	static int memoryScoreNoWrong = 0;
	public static int MemoryScoreNoWrong { get { return memoryScoreNoWrong; } }

	static int memoryScoreYesRight = 200;
	public static int MemoryScoreYesRight { get { return memoryScoreYesRight; } }

	static int memoryScoreYesWrong = -350;
	public static int MemoryScoreYesWrong { get { return memoryScoreYesWrong; } }

	static int specialObjectPoints = 0;
	public static int SpecialObjectPoints { get { return specialObjectPoints; } }

	static int boxSwapperPoints = 200;
	public static int BoxSwapperPoints { get { return boxSwapperPoints; } }

	static int boxSwapperNegPoints = 0;
	public static int BoxSwapperNegPoints { get { return boxSwapperNegPoints; } }

	static int lastBoxSwapScore = 0;
	public static int LastBoxSwapScore { get { return lastBoxSwapScore; } }


	public TextMesh yesScoreExplanation;
	public TextMesh maybeScoreExplanation;
	public TextMesh noScoreExplanation;

	public TimerBar timerBar;

	//trophies
	public GameObject goldTrophy;
	public GameObject silverTrophy;
	public GameObject bronzeTrophy;

	public Transform centralTransform;

	public Transform goldTrophyTransform;
	public Transform silverTrophyTransform;
	public Transform bronzeTrophyTransform;

	public bool giveBronze=false;
	public bool giveSilver=false;
	public bool giveGold=false;

	bool bronzeAwarded=false;
	bool silverAwarded=false;
	bool goldAwarded=false;

	public int silverProgress=0;
	public int goldProgress=0;

	public AudioSource tada;
	public Text trophyText;
	string goldText="Congrats! \n You have won a Gold Trophy \n for a perfect performance";
	string silverText="Congrats! \n You have won a Silver Trophy \n for a fantastic performance";
	string bronzeText="Congrats! \n You have won a Bronze Trophy \n for being on a winning streak";
	public GameObject trophyCanvas;

	// Use this for initialization
	void Start () {
		//yesScoreExplanation.text = "win " + memoryScoreYesRight + "/" + "lose " + memoryScoreYesWrong;
		//maybeScoreExplanation.text = "win " + memoryScoreMaybeRight + "/" + "lose " + memoryScoreMaybeWrong;
		//noScoreExplanation.text = "win " + memoryScoreNoRight + "/" + "lose " + memoryScoreNoWrong;
		bronzeTrophy.SetActive(false);
		goldTrophy.SetActive (false);
		silverTrophy.SetActive (false);
		trophyCanvas.SetActive (false);
		Reset ();
		StartCoroutine (UpdateScoreText());
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.G)) {
			giveBronze = true;
			giveSilver = true;
			giveGold = true;
			StartCoroutine ("GiveTrophies");
		}
	}

	void AddToScore(int amountToAdd){
		amountLeftToAdd += amountToAdd;
		score += amountToAdd;
#if !UNITY_WEBPLAYER
		ExperimentSettings_CoinTask.currentSubject.score = score;
#endif
	}

	IEnumerator UpdateScoreText(){
		float timeBetweenUpdates = 0.01f;

		int increment = 3;
		int incrementMult = 1;

		while (true) {

			int absAmountLeftToAdd = Mathf.Abs (amountLeftToAdd);
			while (absAmountLeftToAdd > 0) {

				if (amountLeftToAdd < 0) {
					incrementMult = -1;
				}
				else{
					incrementMult = 1;
				}

				if(absAmountLeftToAdd > increment){
					amountLeftToAdd -= (increment*incrementMult);
					scoreTextScore += (increment*incrementMult);
				} else {
					scoreTextScore += amountLeftToAdd;
					amountLeftToAdd = 0;
				}

				SetScoreText(scoreTextScore);

				absAmountLeftToAdd = Mathf.Abs (amountLeftToAdd);

				yield return new WaitForSeconds (timeBetweenUpdates);
			}

			yield return 0;

		}
	}



	void SetScoreText(int scoreToDisplay){
		//scoreText.text = "$ " + (scoreTextScore);
		scoreText.text = exp.currInstructions.pointsText + ": " + (scoreToDisplay);
	}

	//TODO: combine these two methods. add a bool as a parameter.
	public void AddBoxSwapperPoints(){
		lastBoxSwapScore = boxSwapperPoints;
		AddToScore(boxSwapperPoints);
		scoreLogger.LogBoxSwapperPoints(boxSwapperPoints);
		Experiment_CoinTask.Instance.uiController.scoreRecapUI.SetBoxSwapBonusText(boxSwapperPoints);
	}

	public void RemoveBoxSwapperPoints(){
		lastBoxSwapScore = boxSwapperNegPoints;
		AddToScore(boxSwapperNegPoints);
		scoreLogger.LogBoxSwapperPoints(boxSwapperNegPoints);
		Experiment_CoinTask.Instance.uiController.scoreRecapUI.SetBoxSwapBonusText(boxSwapperNegPoints);
	}

	public IEnumerator GiveTrophies()
	{
		if (giveBronze && !bronzeAwarded) {
			yield return StartCoroutine (TransformTrophy (bronzeTrophy));
			bronzeAwarded = true;
		}
		if (giveSilver && !silverAwarded) {
			yield return StartCoroutine (TransformTrophy (silverTrophy));
			silverAwarded = true;
		}
		if (giveGold && !goldAwarded) {
			yield return StartCoroutine (TransformTrophy (goldTrophy));
			goldAwarded = true;
		}
		yield return null;
	}
	public void ResetTrophies()
	{
		giveBronze = false;
		giveGold = false;
		giveSilver = false;

		bronzeAwarded = false;
		goldAwarded = false;
		silverAwarded = false;

		trophyCanvas.SetActive (false);

		bronzeTrophy.SetActive (false);
		silverTrophy.SetActive (false);
		goldTrophy.SetActive (false);
	}

	IEnumerator TransformTrophy(GameObject trophy)
	{
		trophy.transform.localPosition = centralTransform.localPosition;
		trophy.SetActive (true);
		switch (trophy.GetComponent<Trophy> ().trophyType) {
		case 1:
			trophyText.text = bronzeText;
			break;
		case 2:
			trophyText.text = silverText;
			break;
		case 3:
			trophyText.text = goldText;
			break;
		}
		trophyCanvas.SetActive (true);
		AudioController.PlayAudio (tada);
		yield return new WaitForSeconds (1f);
		float timer = 0f;
		while (timer < 1.5f) {
			//Debug.Log (timer);
			timer += Time.deltaTime;
			trophy.transform.localPosition = Vector3.Lerp (trophy.transform.localPosition, trophy.GetComponent<Trophy>().canvasTransform.localPosition, timer / 1.5f);
			yield return 0;
		}
		timer = 0f;
		trophyCanvas.SetActive (false);
		yield return null;
	}
	public void AddSpecialPoints(){
		AddToScore(specialObjectPoints);
		scoreLogger.LogTreasureOpenScoreAdded (specialObjectPoints);
	}

	public int CalculateMemoryPoints (Vector3 correctPosition, Config_CoinTask.MemoryState memoryState){//, bool doubledDown){
		int memoryPoints = 0;
		if (exp.environmentController.myPositionSelector.GetRadiusOverlap (correctPosition)) {
			switch(memoryState){
				case Config_CoinTask.MemoryState.yes:
					memoryPoints = memoryScoreYesRight;
					break;
				case Config_CoinTask.MemoryState.maybe:
					memoryPoints = memoryScoreMaybeRight;
					break;
				case Config_CoinTask.MemoryState.no:
					memoryPoints = memoryScoreNoRight;
				break;
			}
		}
		else{ //wrong
			switch(memoryState){
				case Config_CoinTask.MemoryState.yes:
					memoryPoints = memoryScoreYesWrong;
					break;
				case Config_CoinTask.MemoryState.maybe:
					memoryPoints = memoryScoreMaybeWrong;
					break;
				case Config_CoinTask.MemoryState.no:
					memoryPoints = memoryScoreNoWrong;
				break;
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
		//else if (secondsToCompleteTrial < timeBonusTimeMax) {
		//	timeBonusScore = timeBonusSmall;
		//} 

		AddToScore (timeBonusScore);
		scoreLogger.LogTimeBonusAdded (timeBonusScore);

		return timeBonusScore;

	}

	public void Reset(){
		score = 0;
		scoreTextScore = 0;
		amountLeftToAdd = 0;
		SetScoreText (0);

		scoreLogger.LogScoreReset ();
	}

}
