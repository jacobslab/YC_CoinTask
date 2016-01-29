using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {

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

	void Start(){
		if (ExperimentSettings_CoinTask.isOculus) {
			InSceneUIParent.position = OculusInSceneUIPos.position;
			InSceneUIParent.parent = OculusInSceneUIPos;
			InSceneUIParent.rotation = OculusInSceneUIPos.rotation;
		}
	}
}
