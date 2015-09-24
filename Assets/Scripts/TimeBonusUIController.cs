using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeBonusUIController : MonoBehaviour {

	public RectTransform bonusTextMoveToTransform;
	Vector3 bonusTextOrigPosition;
	Vector3 bonusTextOrigScale;


	public Image timeLessThanPanel;
	public Text timeLessThanText;
	public Text timeBonusToAddText;

	public Color timeBonusPanelColorDefault;
	public Color timeBonusPanelColor1;
	public Color timeBonusPanelColor2;
	public Color timeBonusPanelColor3;

	float colorLerpTime = 0.5f;

	SimpleTimer trialTimer { get { return Experiment_CoinTask.Instance.trialController.trialTimer; } }

	int minTimeBonusTime { get { return ScoreController.TimeBonusTimeSmall; } }
	int medTimeBonusTime { get { return ScoreController.TimeBonusTimeMed; } }
	int maxTimeBonusTime { get { return ScoreController.TimeBonusTimeBig; } }


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
		bonusTextOrigScale = timeBonusToAddText.rectTransform.localScale;
		bonusTextOrigPosition = timeBonusToAddText.rectTransform.position;
	}
	
	// Update is called once per frame
	void Update () {
		int seconds = trialTimer.GetSeconds ();

		if (seconds == 0) {
			ResetBonusText();
		}

		SetText (seconds);

		if (seconds < minTimeBonusTime) {
			//best color
			if (timeState != TimeState.minTime) {
				timeState = TimeState.minTime;
				StartCoroutine (LerpPanelColor (timeLessThanPanel, timeBonusPanelColor1, timeLessThanPanel.color, colorLerpTime));
				StartCoroutine (LerpTextColor (timeBonusToAddText, timeBonusPanelColor1, timeBonusToAddText.color, colorLerpTime));
			}
		} else if (seconds < medTimeBonusTime) {
			if (timeState != TimeState.medTime) {
				timeState = TimeState.medTime;
				StartCoroutine (LerpPanelColor (timeLessThanPanel, timeBonusPanelColor2, timeBonusPanelColor1, colorLerpTime));
				StartCoroutine (LerpTextColor (timeBonusToAddText, timeBonusPanelColor2, timeBonusPanelColor1, colorLerpTime));
			}
		} else if (seconds < maxTimeBonusTime) {
			if (timeState != TimeState.maxTime) {
				timeState = TimeState.maxTime;
				StartCoroutine (LerpPanelColor (timeLessThanPanel, timeBonusPanelColor3, timeBonusPanelColor2, colorLerpTime));
				StartCoroutine (LerpTextColor (timeBonusToAddText, timeBonusPanelColor3, timeBonusPanelColor2, colorLerpTime));
			}
		} else if (seconds >= maxTimeBonusTime) {
			if (timeState != TimeState.overTime) {
				timeState = TimeState.overTime;
				StartCoroutine (LerpPanelColor (timeLessThanPanel, timeBonusPanelColorDefault, timeBonusPanelColor3, colorLerpTime));
				StartCoroutine (LerpTextColor (timeBonusToAddText, timeBonusPanelColorDefault, timeBonusPanelColor3, colorLerpTime));
			}
		}
	}

	public IEnumerator MoveBonusText (){
		//TODO: fade out panel behind the text...

		float timeToMove = 1.0f;

		Vector3 startPos = timeBonusToAddText.rectTransform.position;
		Vector3 endPos = bonusTextMoveToTransform.position;

		float currTime = 0.0f;

		while (currTime < timeToMove) {
			timeBonusToAddText.rectTransform.position = Vector3.Lerp (startPos, endPos, currTime);
			timeBonusToAddText.rectTransform.localScale = Vector3.Lerp (bonusTextOrigScale, Vector3.zero, currTime/timeToMove);

			currTime  += Time.deltaTime;

			yield return 0;
		}
		timeBonusToAddText.rectTransform.position = endPos;
		timeBonusToAddText.rectTransform.localScale = Vector3.zero;
		

		yield return 0;
	}

	void ResetBonusText(){
		//TODO: fade in panel behind the text...
		timeBonusToAddText.rectTransform.position = bonusTextOrigPosition;
		timeBonusToAddText.rectTransform.localScale = bonusTextOrigScale;
	}

	void SetText(int seconds){
		timeBonusToAddText.text = "+";
		timeLessThanText.text = "<";

		if (seconds < minTimeBonusTime) {
			timeLessThanText.text += minTimeBonusTime;
			timeBonusToAddText.text += ScoreController.TimeBonusBig;
		}
		else if (seconds < medTimeBonusTime) {
			timeLessThanText.text += medTimeBonusTime;
			timeBonusToAddText.text += ScoreController.TimeBonusMed;
		}
		else if (seconds < maxTimeBonusTime) {
			timeLessThanText.text += maxTimeBonusTime;
			timeBonusToAddText.text += ScoreController.TimeBonusSmall;
		}
		else {
			timeLessThanText.text = ">";
			timeLessThanText.text += maxTimeBonusTime;
			timeBonusToAddText.text += "0";
		}

		timeBonusToAddText.text += "!";
		timeLessThanText.text += "s";
	}

	IEnumerator LerpPanelColor(Image panel, Color upperBoundColor, Color lowerBoundColor, float timeToLerp){

		float currentTime = 0.0f;
		float currentTimePercent = 0.0f;

		while (currentTime < timeToLerp) {
			currentTime += Time.deltaTime;
			currentTimePercent = currentTime / timeToLerp;

			panel.color = Color.Lerp (lowerBoundColor, upperBoundColor, currentTimePercent);
			yield return 0;
		}
	}

	IEnumerator LerpTextColor(Text text, Color upperBoundColor, Color lowerBoundColor, float timeToLerp){

		Color opaqueUpperBoundColor = new Color (upperBoundColor.r, upperBoundColor.g, upperBoundColor.b, 1.0f);
		Color opaqueLowerBoundColor = new Color (lowerBoundColor.r, lowerBoundColor.g, lowerBoundColor.b, 1.0f);

		float currentTime = 0.0f;
		float currentTimePercent = 0.0f;
		
		while (currentTime < timeToLerp) {
			currentTime += Time.deltaTime;
			currentTimePercent = currentTime / timeToLerp;
			
			text.color = Color.Lerp (opaqueLowerBoundColor, opaqueUpperBoundColor, currentTimePercent);
			yield return 0;
		}
	}


}
