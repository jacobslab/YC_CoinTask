using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour {

	public SimpleTimer myTimer;

	public RectTransform topBar;
	public RectTransform midBar;
	public RectTransform bottomBar;

	public RectTransform indicator;
	public RectTransform topBonusUI;
	public RectTransform topMidBonusUI;
	public RectTransform bottomMidBonusUI;
	public RectTransform bottomBonusUI;


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
		myTimer.myResetDelegate += Reset;

		barPositions = new Vector3[3];
		barPositions [0] = topBar.position;
		barPositions [1] = midBar.position;
		barPositions [2] = bottomBar.position;

		barWidths = new float[3];
		barWidths [0] = topBar.rect.width;
		barWidths [1] = midBar.rect.width;
		barWidths [2] = bottomBar.rect.width;

		barTimes = new float[3];
		barTimes [0] = ScoreController.TimeBonusTimeMin;
		barTimes [1] = ScoreController.TimeBonusTimeMed - ScoreController.TimeBonusTimeMin;
		barTimes [2] = ScoreController.TimeBonusTimeBig - ScoreController.TimeBonusTimeMed;

		Reset ();
	}
	
	// Update is called once per frame
	void Update () {
		if (myTimer.IsRunning) {

			if (myTimer.GetSeconds () < ScoreController.TimeBonusTimeMin) { //make top bar smaller
				if(timeState != TimeState.minTime){
					SetState(TimeState.minTime, topBonusUI.GetComponentInChildren<Text> ().text, topBonusUI.position);
				}
				ResizeBar (topBar, 0);
			} 
			else if (myTimer.GetSeconds () < ScoreController.TimeBonusTimeMed) { //make mid bar smaller
				if(timeState != TimeState.medTime){
					SetState(TimeState.medTime, topMidBonusUI.GetComponentInChildren<Text> ().text, topMidBonusUI.position);
				}
				ResizeBar (midBar, 1);
			} 
			else if (myTimer.GetSeconds () < ScoreController.TimeBonusTimeBig) { //make bottom bar smaller
				if(timeState != TimeState.maxTime){
					SetState(TimeState.maxTime, bottomMidBonusUI.GetComponentInChildren<Text> ().text, bottomMidBonusUI.position);
				}
				ResizeBar (bottomBar, 2);
			}
			else{
				if(timeState != TimeState.overTime){
					SetState(TimeState.overTime, bottomBonusUI.GetComponentInChildren<Text> ().text, bottomBonusUI.position);
				}
			}

		} 
	}

	void SetState(TimeState newState, string newBonusText, Vector3 indicatorPosition){
		timeState = newState;
		indicator.position = indicatorPosition;
	}

	void ResizeBar(RectTransform bar, int barIndex){
		float widthDecrement = ( barWidths[barIndex] / barTimes[barIndex] ) * Time.deltaTime;

		//0.7 is a trial & error value. when the bar scales, it scales from both sides. thus, we want to reposition the bar so it looks like it's shrinking from only one side.
		float moveAmount = 0.6f * widthDecrement;
		
		bar.sizeDelta = bar.rect.size - new Vector2(widthDecrement, 0);
		bar.position += Vector3.left * moveAmount;
	}

	void Reset(){
		timeState = TimeState.none;

		indicator.position = topBonusUI.position;

		topBar.position = barPositions [0];
		midBar.position = barPositions [1];
		bottomBar.position = barPositions [2];

		topBar.sizeDelta = new Vector2 (barWidths [0], topBar.rect.height);
		midBar.sizeDelta = new Vector2 (barWidths [1], topBar.rect.height);
		bottomBar.sizeDelta = new Vector2 (barWidths [2], topBar.rect.height);

	}

}
