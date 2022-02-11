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
    static int timeBonusTimeMin = 44;
    public static int TimeBonusTimeMin { get { return timeBonusTimeMin; } }

    static int timeBonusTimeMed = 88;
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

    static int memoryScoreYesWrong = 0;
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


    // Use this for initialization
    void Start() {
        //yesScoreExplanation.text = "win " + memoryScoreYesRight + "/" + "lose " + memoryScoreYesWrong;
        //maybeScoreExplanation.text = "win " + memoryScoreMaybeRight + "/" + "lose " + memoryScoreMaybeWrong;
        //noScoreExplanation.text = "win " + memoryScoreNoRight + "/" + "lose " + memoryScoreNoWrong;

        Reset();
        StartCoroutine(UpdateScoreText());
    }

    // Update is called once per frame
    void Update() {

    }

    void AddToScore(int amountToAdd) {
        amountLeftToAdd += amountToAdd;
        score += amountToAdd;
#if !UNITY_WEBPLAYER
        ExperimentSettings_CoinTask.currentSubject.score = score;
#endif
    }

    IEnumerator UpdateScoreText() {
        float timeBetweenUpdates = 0.01f;

        int increment = 3;
        int incrementMult = 1;

        while (true) {

            int absAmountLeftToAdd = Mathf.Abs(amountLeftToAdd);
            while (absAmountLeftToAdd > 0) {

                if (amountLeftToAdd < 0) {
                    incrementMult = -1;
                }
                else {
                    incrementMult = 1;
                }

                if (absAmountLeftToAdd > increment) {
                    amountLeftToAdd -= (increment * incrementMult);
                    scoreTextScore += (increment * incrementMult);
                } else {
                    scoreTextScore += amountLeftToAdd;
                    amountLeftToAdd = 0;
                }

                SetScoreText(scoreTextScore);

                absAmountLeftToAdd = Mathf.Abs(amountLeftToAdd);

                yield return new WaitForSeconds(timeBetweenUpdates);
            }

            yield return 0;

        }
    }

    void SetScoreText(int scoreToDisplay) {
        //scoreText.text = "$ " + (scoreTextScore);
        scoreText.text = exp.currInstructions.pointsText + ": " + (scoreToDisplay);
    }

    //TODO: combine these two methods. add a bool as a parameter.
    public void AddBoxSwapperPoints() {
        lastBoxSwapScore = boxSwapperPoints;
        AddToScore(boxSwapperPoints);
        scoreLogger.LogBoxSwapperPoints(boxSwapperPoints);
        Experiment_CoinTask.Instance.uiController.scoreRecapUI.SetBoxSwapBonusText(boxSwapperPoints);
    }

    public void RemoveBoxSwapperPoints() {
        lastBoxSwapScore = boxSwapperNegPoints;
        AddToScore(boxSwapperNegPoints);
        scoreLogger.LogBoxSwapperPoints(boxSwapperNegPoints);
        Experiment_CoinTask.Instance.uiController.scoreRecapUI.SetBoxSwapBonusText(boxSwapperNegPoints);
    }


    public void AddSpecialPoints() {
        AddToScore(specialObjectPoints);
        scoreLogger.LogTreasureOpenScoreAdded(specialObjectPoints);
    }

    public int CalculateMemoryPoints(Vector3 correctPosition) {//, bool doubledDown){
        int memoryPoints = 0;
        if (exp.environmentController.myPositionSelector.GetRadiusOverlap(correctPosition)) {
            memoryPoints = memoryScoreYesRight;
        }
        else { //wrong
            memoryPoints = memoryScoreYesWrong;
        }

        AddToScore(memoryPoints);
        scoreLogger.LogMemoryScoreAdded(memoryPoints);

        return memoryPoints;
    }

    public int CalculateTimeBonus(float secondsToCompleteTrial) {
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

        AddToScore(timeBonusScore);
        scoreLogger.LogTimeBonusAdded(timeBonusScore);

        return timeBonusScore;

    }

    public int CalculatePathIntegrationScore(float pathIntegrationError)
    {
        int pathIntegrationScore = 100;

        //show path integration score
        if (pathIntegrationError < 0.25f * exp.environmentController.envRadius)
        {
            pathIntegrationScore = 400;
        }
        else if (pathIntegrationError < 0.5f * exp.environmentController.envRadius)
        {
            pathIntegrationScore = 200;
        }
        else
        {
            pathIntegrationScore = 100;
        }
        AddToScore(pathIntegrationScore);
        scoreLogger.LogTimeBonusAdded(pathIntegrationScore);

        return pathIntegrationScore;
    }

    public void CalculateSequenceBonus(int correctScore)
    {
        AddToScore(correctScore);
        scoreLogger.LogSequenceBonusAdded(correctScore);
    }

	public void Reset(){
		score = 0;
		scoreTextScore = 0;
		amountLeftToAdd = 0;
		SetScoreText (0);

		scoreLogger.LogScoreReset ();
	}

}
