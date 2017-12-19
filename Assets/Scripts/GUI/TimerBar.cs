using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour {

	public CanvasGroup HurryUpText;

	public SimpleTimer myTimer;

	public RectTransform topBar;
	public RectTransform midBar;
	//public RectTransform bottomBar;

	public RectTransform indicator;
	public RectTransform topBonusUI;
	public RectTransform topMidBonusUI;
	public RectTransform bottomMidBonusUI;
	//public RectTransform bottomBonusUI;


	Vector3[] barPositions;
	float[] barWidths;
	float[] barTimes;


	enum TimeState {
		none,
		minTime,
		medTime,
		maxTime,
		overTime
	}

	TimeState timeState = TimeState.none;

	// Use this for initialization
	void Start () {

			HurryUpText.gameObject.GetComponent<Text> ().text= Experiment_CoinTask.Instance.currInstructions.hurryUpText;
		myTimer.myResetDelegate += Reset;

		barPositions = new Vector3[2];
		RectTransform topParent = topBar.transform.parent.GetComponent<RectTransform> ();
		RectTransform midParent = topBar.transform.parent.GetComponent<RectTransform> ();
		barPositions [0] = topBar.position;
		barPositions [1] = midBar.position;
		//barPositions [2] = bottomBar.position;

		barWidths = new float[2];
		barWidths [0] = topBar.rect.width;
		barWidths [1] = midBar.rect.width;
		//barWidths [2] = bottomBar.rect.width;

		barTimes = new float[2];
		barTimes [0] = ScoreController.TimeBonusTimeMin;
		barTimes [1] = ScoreController.TimeBonusTimeMed - ScoreController.TimeBonusTimeMin;
		//barTimes [2] = ScoreController.TimeBonusTimeBig - ScoreController.TimeBonusTimeMed;

		Reset ();
	}

	// Update is called once per frame
	void Update () {
		if (myTimer.IsRunning) {

			if (myTimer.GetSecondsInt () < ScoreController.TimeBonusTimeMin) { //make top bar smaller
				if(timeState != TimeState.minTime){
					SetState(TimeState.minTime, topBonusUI.GetComponentInChildren<Text> ().text, topBonusUI.position);
				}
				MoveBar (topBar, 0);
			} 
			else if (myTimer.GetSecondsInt () < ScoreController.TimeBonusTimeMed) { //make mid bar smaller
				if(timeState != TimeState.medTime){
					SetState(TimeState.medTime, topMidBonusUI.GetComponentInChildren<Text> ().text, topMidBonusUI.position);
				}
				MoveBar (midBar, 1);
				EnableLowTimeText();
			} 
			/*else if (myTimer.GetSecondsInt () < ScoreController.TimeBonusTimeBig) { //make bottom bar smaller
				if(timeState != TimeState.maxTime){
					SetState(TimeState.maxTime, bottomMidBonusUI.GetComponentInChildren<Text> ().text, bottomMidBonusUI.position);
				}
				EnableLowTimeText();
				MoveBar (bottomBar, 2);
			}*/
			else{
				if(timeState != TimeState.overTime){
					//SetState(TimeState.overTime, bottomBonusUI.GetComponentInChildren<Text> ().text, bottomBonusUI.position);
					SetState(TimeState.overTime, bottomMidBonusUI.GetComponentInChildren<Text> ().text, bottomMidBonusUI.position);
				}
			}

		}
		else{
			if(!ExperimentSettings_CoinTask.isReplay){
				DisableLowTimeText();
			}
		}
	}

	void EnableLowTimeText(){
		HurryUpText.alpha = 1.0f;
	}

	void DisableLowTimeText(){
		HurryUpText.alpha = 0.0f;
	}

	void SetState(TimeState newState, string newBonusText, Vector3 indicatorPosition){
		timeState = newState;
		indicator.position = indicatorPosition;
	}
	float amountMoveTotal = 0;
	void MoveBar(RectTransform bar, int barIndex){
		float totalMoveDistance = barWidths [barIndex];
		float moveAmount = ( totalMoveDistance / barTimes[barIndex] ) * Time.deltaTime * bar.transform.lossyScale.x;

		bar.position -= bar.transform.right * moveAmount;// += Vector3.left * moveAmount;

		if (barIndex == 0) {
			amountMoveTotal += moveAmount;
		}
	}

	void Reset(){

		DisableLowTimeText ();

		timeState = TimeState.none;

		indicator.position = topBonusUI.position;

		topBar.position = topBar.transform.parent.position;//barPositions [0];
		midBar.position = midBar.transform.parent.position;//barPositions [1];
		//bottomBar.position = barPositions [2];

	}

}
