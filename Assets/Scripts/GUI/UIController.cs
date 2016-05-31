using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public PirateController pirateController;
	public QuestionUI doYouRememberUI;
	//public QuestionUI areYouSureUI;
	public BlockCompleteUI blockCompletedUI;
	public ScoreRecapUI scoreRecapUI;
	public CanvasGroup PauseUI;
	//public Text PauseInstruction;
	public CanvasGroup ConnectionUI;
	public GoText goText;


	public Transform InSceneUIParent;
	public Transform OculusInSceneUIPos;


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
}
