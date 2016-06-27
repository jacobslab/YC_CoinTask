using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }


	public PirateController pirateController;
	public QuestionUI doYouRememberUI;
	//public QuestionUI areYouSureUI;
	public BlockCompleteUI blockCompletedUI;
	public ScoreRecapUI scoreRecapUI;
	public CanvasGroup PauseUI;
	public CanvasGroup ConnectionUI;
	public GoText goText;

	public Transform InSceneUIParent;
	public Transform OculusInSceneUIPos;


	//FOR LOCALIZATION
	public Text pauseInstruction;
	public Text gamePausedText;

	public TextMesh boxSwapTitle;
	public TextMesh boxSwapExplanation;

	public Text yesExplanationInstruction;
	public Text maybeExplanationInstruction;
	public Text noExplanationInstruction;

	public Text yesText;
	public Text maybeText;
	public Text noText;

	public TextMesh yesAnswerText;
	public TextMesh maybeAnswerText;
	public TextMesh noAnswerText;

	public TextMesh yesExplanationText;
	public TextMesh maybeExplanationText;
	public TextMesh noExplanationText;

	public TextMesh doYouRememberText;
	public TextMesh recallPressToSelectText;

	public TextMesh trialCompleteTitle;
	public TextMesh timeBonusTitle;
	public TextMesh boxSwapBonusTitle;
	public TextMesh trialScoreTitle;
	public TextMesh scoreScreenPressToContinue;

	public TextMesh blockCompleteText;
	public TextMesh blockTitle1;
	public TextMesh blockTitle2;
	public TextMesh blockTitle3;
	public TextMesh blockTitle4;
	public TextMesh blockTitle5;
	public TextMesh blockScreenPressToContinue;


	//MRI VERSION
	public CanvasGroup WaitingForMRIUI;
	public CanvasGroup FixationRestUI;
	public CanvasGroup MRITimerUI;

	void Start(){
		#if MRIVERSION
		//turn off pause instruction in main version, as the player cannot pause!
		if(!Config_CoinTask.isPractice){
			GameObject PauseInstruction = GameObject.Find ("Pause Instruction Text");
			PauseInstruction.GetComponent<Text> ().text = "";

			//TODO: CHANGE ^^^^ TO A REFERENCE AND SET THE TEXT TO BLANK
			//PauseInstruction.text = ""; //get rid of pause instruction if in real task
		}
		#endif

		if (ExperimentSettings_CoinTask.isOculus) {
			InSceneUIParent.position = OculusInSceneUIPos.position;
			InSceneUIParent.parent = OculusInSceneUIPos;
			InSceneUIParent.rotation = OculusInSceneUIPos.rotation;
		}
	}

	public void InitText(){
		pauseInstruction.text = exp.currInstructions.pauseText;
		gamePausedText.text = exp.currInstructions.gamePausedText;

		//box swap screen
		boxSwapTitle.text = exp.currInstructions.boxSwapBonusTitle;
		boxSwapExplanation.text = exp.currInstructions.boxExplanation;

		//instructions
		yesText.text = exp.currInstructions.yesAnswerText;
		maybeText.text = exp.currInstructions.maybeAnswerText;
		noText.text = exp.currInstructions.noAnswerText;

		yesExplanationInstruction.text = exp.currInstructions.yesExplanationInstruction;
		maybeExplanationInstruction.text = exp.currInstructions.maybeExplanationInstruction;
		noExplanationInstruction.text = exp.currInstructions.noExplanationInstruction;

		//answer selector
		yesAnswerText.text = exp.currInstructions.yesAnswerText;
		maybeAnswerText.text = exp.currInstructions.maybeAnswerText;
		noAnswerText.text = exp.currInstructions.noAnswerText;

		yesExplanationText.text = exp.currInstructions.yesPointsText;
		maybeExplanationText.text = exp.currInstructions.maybePointsText;
		noExplanationText.text = exp.currInstructions.noPointsText;

		//do you remember
		doYouRememberText.text = exp.currInstructions.doYouRememberText;
		recallPressToSelectText.text = exp.currInstructions.pressToSelect;

		//trial score screen
		trialCompleteTitle.text = exp.currInstructions.trialCompleteText;
		timeBonusTitle.text = exp.currInstructions.timeBonusText + ":";
		boxSwapBonusTitle.text = exp.currInstructions.boxSwapBonusText + ":";
		trialScoreTitle.text = exp.currInstructions.trialScoreText + ":";
		scoreScreenPressToContinue.text = exp.currInstructions.pressToContinue;

		//block screen
		blockCompleteText.text = exp.currInstructions.blockCompletedText;
		blockTitle1.text = exp.currInstructions.blockUpper + " 1 " + exp.currInstructions.score + ":";
		blockTitle2.text = exp.currInstructions.blockUpper + " 2 " + exp.currInstructions.score + ":";
		blockTitle3.text = exp.currInstructions.blockUpper + " 3 " + exp.currInstructions.score + ":";
		blockTitle4.text = exp.currInstructions.blockUpper + " 4 " + exp.currInstructions.score + ":";
		blockTitle5.text = exp.currInstructions.blockUpper + " 5 " + exp.currInstructions.score + ":";
		blockScreenPressToContinue.text = exp.currInstructions.pressToContinue;
	}

}
