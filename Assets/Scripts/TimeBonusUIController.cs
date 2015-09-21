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

	SimpleTimer trialTimer { get { return Experiment_CoinTask.Instance.trialController.trialTimer; } }

	float minTimeBonusTime { get { return ScoreController.TimeBonusTimeSmall; } }
	float medTimeBonusTime { get { return ScoreController.TimeBonusTimeMed; } }
	float maxTimeBonusTime { get { return ScoreController.TimeBonusTimeBig; } }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	//TODO: pulse colors? lerp between colors?
	void Update () {
		int seconds = trialTimer.GetSeconds ();
		if (seconds < minTimeBonusTime) {
			//best color
			timeLessThanPanel.color = timeBonusPanelColor1;
		}
		else if (seconds > minTimeBonusTime && seconds < medTimeBonusTime){
			//lerp between best color and 2nd best color
			timeLessThanPanel.color = timeBonusPanelColor2;
		}
		else if (seconds > medTimeBonusTime && seconds < maxTimeBonusTime){
			//lerp between 2nd best color and default color
			timeLessThanPanel.color = timeBonusPanelColor3;
		}
		else if (seconds > maxTimeBonusTime){
			//normal color -- no time bonus
			timeLessThanPanel.color = timeBonusPanelColorDefault;
		}
	}


}
