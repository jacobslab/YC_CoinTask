using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeBonusUIController : MonoBehaviour {

	public RectTransform bonusTextMoveToTransform;
	Vector3 bonusTextOrigPosition;
	Vector3 bonusTextOrigScale;


	public Image timeLessThanPanel;
	public Image timeBonusTextPanel;
	public Text timeLessThanText;
	public Text timeBonusText;

	public Color defaultPanelColor;
	public Color panelColor0;
	public Color panelColor1;
	public Color panelColor2;
	public Color panelColor3;

	public Color timeBonusTextColorDefault;
	public Color timeBonusTextColor1;
	public Color timeBonusTextColor2;
	public Color timeBonusTextColor3;
	
	float colorLerpTime = 0.5f;

	//SimpleTimer trialTimer { get { return Experiment_CoinTask.Instance.trialController.trialTimer; } }

	int minTimeBonusTime { get { return ScoreController.TimeBonusTimeMin; } }
	int medTimeBonusTime { get { return ScoreController.TimeBonusTimeMed; } }
	//int maxTimeBonusTime { get { return ScoreController.TimeBonusTimeBig; } }


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
		bonusTextOrigScale = timeBonusText.rectTransform.localScale;
		bonusTextOrigPosition = timeBonusText.rectTransform.position;

	}
	
	// Update is called once per frame
	void Update () {

		//if (!ExperimentSettings_CoinTask.isReplay) {
		//	UpdateTimeState();
		//}
	}

	void UpdateTimeState(){
		//int seconds = trialTimer.GetSecondsInt ();
        int seconds = 1;
		if (seconds == 0) {
			ResetBonusText();
		}
		
		SetText (seconds);
		
		if (seconds < minTimeBonusTime) {
			if (timeState != TimeState.minTime) {
				timeState = TimeState.minTime;
				StartCoroutine (LerpPanelColor (timeLessThanPanel, timeLessThanPanel.color, panelColor1, colorLerpTime));
				StartCoroutine (LerpTextColor (timeBonusText, timeBonusText.color, timeBonusTextColor1, colorLerpTime));
			}
		} else if (seconds < medTimeBonusTime) {
			if (timeState != TimeState.medTime) {
				timeState = TimeState.medTime;
				StartCoroutine (LerpPanelColor (timeLessThanPanel, timeLessThanPanel.color, panelColor2, colorLerpTime));
				StartCoroutine (LerpTextColor (timeBonusText, timeBonusText.color, timeBonusTextColor2, colorLerpTime));
			}
/*		} else if (seconds < maxTimeBonusTime) {
			if (timeState != TimeState.maxTime) {
				timeState = TimeState.maxTime;
				StartCoroutine (LerpPanelColor (timeLessThanPanel, timeLessThanPanel.color, panelColor3, colorLerpTime));
				StartCoroutine (LerpTextColor (timeBonusText, timeBonusText.color, timeBonusTextColor3, colorLerpTime));
			}*/
		} else if (seconds >= medTimeBonusTime) {
			if (timeState != TimeState.overTime) {
				timeState = TimeState.overTime;
				StartCoroutine (LerpPanelColor (timeLessThanPanel, timeLessThanPanel.color, panelColor0, colorLerpTime));
				StartCoroutine (LerpTextColor (timeBonusText, timeBonusText.color, timeBonusTextColorDefault, colorLerpTime));
			}
		}
	}

	public IEnumerator MoveBonusText (){

		float timeToMove = 1.0f;

		Vector3 startPos = timeBonusText.rectTransform.position;
		Vector3 endPos = bonusTextMoveToTransform.position;

		float currTime = 0.0f;

		StartCoroutine( LerpPanelColor (timeBonusTextPanel, timeBonusTextPanel.color, Color.clear, timeToMove) );

		while (currTime < timeToMove) {
			timeBonusText.rectTransform.position = Vector3.Lerp (startPos, endPos, currTime);
			timeBonusText.rectTransform.localScale = Vector3.Lerp (bonusTextOrigScale, Vector3.zero, currTime/timeToMove);

			currTime  += Time.deltaTime;

			yield return 0;
		}
		timeBonusText.rectTransform.position = endPos;
		timeBonusText.rectTransform.localScale = Vector3.zero;
		

		yield return 0;
	}

	void ResetBonusText(){
		timeBonusText.rectTransform.position = bonusTextOrigPosition;
		timeBonusText.rectTransform.localScale = bonusTextOrigScale;

		//reset its background panel color
		timeBonusTextPanel.color = defaultPanelColor;
	}

	void SetText(int seconds){
		timeBonusText.text = "+";
		timeLessThanText.text = "<";

		if (seconds < minTimeBonusTime) {
			timeLessThanText.text += minTimeBonusTime;
			timeBonusText.text += ScoreController.TimeBonusBig;
		}
		else if (seconds < medTimeBonusTime) {
			timeLessThanText.text += medTimeBonusTime;
			timeBonusText.text += ScoreController.TimeBonusMed;
		}
		/*else if (seconds < maxTimeBonusTime) {
			timeLessThanText.text += maxTimeBonusTime;
			timeBonusText.text += ScoreController.TimeBonusSmall;
		}*/
		else {
			timeLessThanText.text = ">";
			timeLessThanText.text += medTimeBonusTime;
			timeBonusText.text += "0";
		}

		timeBonusText.text += "!";
		timeLessThanText.text += "s";
	}

	IEnumerator LerpPanelColor(Image panel, Color fromColor, Color toColor, float timeToLerp){

		float currentTime = 0.0f;
		float currentTimePercent = 0.0f;

		while (currentTime < timeToLerp) {
			currentTime += Time.deltaTime;
			currentTimePercent = currentTime / timeToLerp;

			panel.color = Color.Lerp (fromColor, toColor, currentTimePercent);
			yield return 0;
		}
	}

	IEnumerator LerpTextColor(Text text, Color fromColor, Color toColor, float timeToLerp){

		float currentTime = 0.0f;
		float currentTimePercent = 0.0f;
		
		while (currentTime < timeToLerp) {
			currentTime += Time.deltaTime;
			currentTimePercent = currentTime / timeToLerp;
			
			text.color = Color.Lerp (fromColor, toColor, currentTimePercent);
			yield return 0;
		}
	}


}
