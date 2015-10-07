using UnityEngine;
using System.Collections;

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

	bool barsResized = false;

	// Use this for initialization
	void Start () {
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
	}
	
	// Update is called once per frame
	void Update () {
		if (myTimer.IsRunning) {

			barsResized = true;

			if (myTimer.GetSeconds () < ScoreController.TimeBonusTimeMin) { //make top bar smaller
				indicator.position = topBonusUI.position;
				ResizeBar (topBar, 0);
			} 
			else if (myTimer.GetSeconds () < ScoreController.TimeBonusTimeMed) { //make mid bar smaller
				indicator.position = topMidBonusUI.position;
				ResizeBar (midBar, 1);
			} 
			else if (myTimer.GetSeconds () < ScoreController.TimeBonusTimeBig) { //make bottom bar smaller
				indicator.position = bottomMidBonusUI.position;
				ResizeBar (bottomBar, 2);
			}
			else{
				indicator.position = bottomBonusUI.position;
			}

		} 
		else if(barsResized) {
			ResetBars();
		}
	}

	void ResizeBar(RectTransform bar, int barIndex){
		float widthDecrement = ( barWidths[barIndex] / barTimes[barIndex] ) * Time.deltaTime;
		float moveAmount = 0.5f * widthDecrement;
		
		bar.sizeDelta = bar.rect.size - new Vector2(widthDecrement, 0);
		bar.position += Vector3.left * moveAmount;
	}

	void ResetBars(){
		indicator.position = topBonusUI.position;

		topBar.position = barPositions [0];
		midBar.position = barPositions [1];
		bottomBar.position = barPositions [2];

		topBar.sizeDelta = new Vector2 (barWidths [0], topBar.rect.height);
		midBar.sizeDelta = new Vector2 (barWidths [1], topBar.rect.height);
		bottomBar.sizeDelta = new Vector2 (barWidths [2], topBar.rect.height);

		barsResized = false;
	}
}
