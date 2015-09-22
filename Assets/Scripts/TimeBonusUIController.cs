using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeBonusUIController : MonoBehaviour {

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
		
	}
	
	// Update is called once per frame
	void Update () {
		int seconds = trialTimer.GetSeconds ();

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
