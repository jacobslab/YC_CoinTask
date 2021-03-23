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
	public Text ConnectionText; //changed in TrialController from "connecting..." to "press start..." etc.
	public GoText goText;

	public Transform InSceneUIParent;
	public Transform OculusInSceneUIPos;

	public CanvasGroup exitPanel;


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

    //free recall panel
    public CanvasGroup freeRecallInstructionGroup;
    public CanvasGroup freeRecallFixationGroup;

    public CanvasGroup microphoneIndicatorGroup;
    public Image microphoneUseImage;

    public CanvasGroup verbalFRGroup;
    public CanvasGroup mathDistractorGroup;
    public Text verbalFRWordText;

	//recency retrieval

	public GameObject recencyRetrievalPanel;
	public TextMesh itemAText;
	public TextMesh itemBText;
	public Transform itemAPosition;
	public Transform itemBPosition;

	public CanvasGroup recencyCanvasGroup;

	//door instruction panel
	public CanvasGroup doorInstructionPanel;

	//MRI VERSION
	public CanvasGroup WaitingForMRIUI;
	public CanvasGroup FixationRestUI;
	public CanvasGroup MRITimerUI;

	//retrieval type
	public CanvasGroup retrievalTypePanel;
	public Text retrievalTypeText;

    public CanvasGroup travellingBetween;
    public CanvasGroup feedbackRecencyPanel;


	private void Awake()
    {
        verbalFRGroup.transform.parent.gameObject.SetActive(false);
    }

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
		/* OCULUS is same as main camera now
		if (ExperimentSettings_CoinTask.isOculus) {
			InSceneUIParent.position = OculusInSceneUIPos.position;
			InSceneUIParent.parent = OculusInSceneUIPos;
			InSceneUIParent.rotation = OculusInSceneUIPos.rotation;
		}
		*/
	}

	public void InitText(){
		pauseInstruction.text = exp.currInstructions.pauseText;
		gamePausedText.text = exp.currInstructions.gamePausedText;

		//box swap screen
		boxSwapTitle.text = exp.currInstructions.boxSwapBonusTitle;
		boxSwapExplanation.text = exp.currInstructions.boxSwapExplanation;

		//instructions
		yesText.text = "[" + exp.currInstructions.yesAnswerText + "]";
	//	maybeText.text = "[" + exp.currInstructions.maybeAnswerText + "]";
		noText.text = "[" + exp.currInstructions.noAnswerText + "]";

		//answer selector
		yesAnswerText.text = exp.currInstructions.yesAnswerText;
	//	maybeAnswerText.text = exp.currInstructions.maybeAnswerText;
		noAnswerText.text = exp.currInstructions.noAnswerText;
		if(ExperimentSettings_CoinTask.myLanguage == ExperimentSettings_CoinTask.LanguageSetting.Spanish){
			yesAnswerText.fontSize = 250;
			maybeAnswerText.fontSize = 250;
			noAnswerText.fontSize = 250;
		}

		yesExplanationText.text = splitUpLongText (exp.currInstructions.yesPointsText, 28, '/');
	//	maybeExplanationText.text = splitUpLongText (exp.currInstructions.maybePointsText, 28, '/');
		noExplanationText.text = splitUpLongText (exp.currInstructions.noPointsText, 28, '/');

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
		blockTitle1.text = exp.currInstructions.block1Text;
		blockTitle2.text = exp.currInstructions.block2Text;
		blockTitle3.text = exp.currInstructions.block3Text;
		blockTitle4.text = exp.currInstructions.block4Text;
		blockTitle5.text = exp.currInstructions.block5Text;
		blockScreenPressToContinue.text = exp.currInstructions.pressToContinue;
	}

	string splitUpLongText(string inputString, int maxLength, char splitChar){
		string output = "";
		if (inputString.Length > maxLength) {
			string[] splitString = inputString.Split (splitChar);
			for (int i = 0; i < splitString.Length; i++) {
				if (i != 0) {
					output += splitChar;
					output += "\n";
				}
				output += splitString [i];
			}
		} else {
			return inputString;
		}

		return output;
	}


    public void EnableMicAvailability(bool shouldEnable)
    {
        if(shouldEnable)
        {
            microphoneIndicatorGroup.alpha = 1f;
        }
        else
        {
            microphoneIndicatorGroup.alpha = 0f;
        }
    }
    public void ShowMicUsage(bool canUse)
    {
        if (canUse)
        {
            microphoneUseImage.color = Color.green;
        }
        else
        {
            microphoneUseImage.color = Color.red;
        }
    }

	public IEnumerator ShowRecencyRetrieval(GameObject objA, GameObject objB)
    {
		recencyRetrievalPanel.gameObject.SetActive(true);
		yield return StartCoroutine(recencyRetrievalPanel.gameObject.GetComponent<QuestionUI>().Play());

		bool isFlipped = false;
		GameObject leftObj = null;
		GameObject rightObj = null;
		if(Random.value > 0.5f)
        {
			isFlipped = true;
			leftObj = objB;
			rightObj = objA;
        }
        else
        {
			isFlipped = false;
			leftObj = objA;
			rightObj = objB;
        }

		itemAText.text = leftObj.gameObject.GetComponent<SpawnableObject>().GetDisplayName();
		itemBText.text = rightObj.gameObject.GetComponent<SpawnableObject>().GetDisplayName();

		GameObject itemA = Instantiate(leftObj, itemAPosition.transform.position, itemAPosition.transform.rotation) as GameObject;
		itemA.transform.parent = itemAPosition.transform;
		if (itemA.gameObject.name.Contains("pineapple") || itemA.gameObject.name.Contains("lamp"))
		{
			itemA.transform.localScale = new Vector3(30f, 30f, 30f);
		}
		else

        {
			itemA.transform.localScale = new Vector3(8f, 8f, 8f);
        }
		GameObject itemB = Instantiate(rightObj, itemBPosition.transform.position, itemBPosition.transform.rotation) as GameObject;
		itemB.transform.parent = itemBPosition.transform;
		if (itemB.gameObject.name.Contains("pineapple") || itemB.gameObject.name.Contains("lamp"))
		{
			itemB.transform.localScale = new Vector3(30f, 30f, 30f);
		}
		else

		{
			itemB.transform.localScale = new Vector3(8f,8f,8f);
		}

		itemA.GetComponent<VisibilityToggler>().TurnVisible(true);
		itemB.GetComponent<VisibilityToggler>().TurnVisible(true);
		while (!Input.GetKeyDown(KeyCode.X))
        {
			yield return 0;
        }

		recencyRetrievalPanel.gameObject.SetActive(false);
		Destroy(itemA);
		Destroy(itemB);
		yield return null;
    }

	public void ShowDoorInstruction(bool shouldEnable)
    {
		
		doorInstructionPanel.alpha = (shouldEnable)? 1f : 0f;
    }

}
