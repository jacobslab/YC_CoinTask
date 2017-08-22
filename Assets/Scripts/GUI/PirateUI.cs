using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PirateUI : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public CanvasGroup myCanvasGroup;
	public Text speechText;

	public enum PirateTextType{
		start,
		encouraging,
		end
	}

	float fadeInTime = 0.2f;
	float fadeOutTime = 0.5f;

	// Use this for initialization
	void Start () {
		myCanvasGroup.alpha = 0.0f;
	}
	
	IEnumerator SetVisible(bool shouldBeVisible){
		float totalTime = fadeInTime;
		int visibilityMult = 1;
		
		if (!shouldBeVisible) {
			totalTime = fadeOutTime;
			visibilityMult = -1;
		}
		
		
		float currentTime = 0.0f;
		
		while(currentTime < totalTime){
			currentTime += Time.deltaTime;
			
			float increment = (1.0f / totalTime) * Time.deltaTime;
			
			myCanvasGroup.alpha += (increment*visibilityMult);
			yield return 0;
		}
	}
	
	public IEnumerator Play(PirateTextType typeOfText, bool shouldAutoStop){
		SetText (typeOfText);

		yield return StartCoroutine (SetVisible (true));
		
		//if (typeOfText == PirateTextType.start) {
		yield return new WaitForSeconds (3f);
		//}
		if (shouldAutoStop) {
			Stop ();
		} 
		else {
			//wait for action button
			while(Input.GetAxis(Config_CoinTask.ActionButtonName) <= 0){
				yield return 0;
			}

			Stop();
		}
	}
	
	public void Stop(){
		StartCoroutine (SetVisible (false));
	}

	
	void SetText(PirateTextType typeOfText){
		if (typeOfText == PirateTextType.start) {
			speechText.text = exp.currInstructions.pirateWelcomeText;
		} 
		else if (typeOfText == PirateTextType.encouraging) {
			int randomIndex = Random.Range(0, exp.currInstructions.pirateEncouragingText.Length);
			speechText.text = exp.currInstructions.pirateEncouragingText[randomIndex];
		} 
		else if (typeOfText == PirateTextType.end) {
			speechText.text = exp.currInstructions.pirateEndText;
		}
	}

}
