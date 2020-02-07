using UnityEngine;
using System.Collections;

public class AnswerSelector : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	//bool shouldCheckForInput = false;

	bool resetToRandomPosition = true;

	public Transform[] positionTransforms; //should be put in order of left to right
	public GameObject selectorVisuals;
	public ColorChanger yesExplanationColorChanger;
	public ColorChanger maybeExplanationColorChanger;
	public ColorChanger noExplanationColorChanger;

	public Color selectedColor;
	public Color deselectedColor;

	public AudioSource selectionSwitchAudio;

	int currPositionIndex = 0;

	void Awake(){

	}

	// Use this for initialization
	void Start () {
		ResetSelectorPosition ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void SetShouldCheckForInput(bool shouldCheck){
		if (shouldCheck) {
			ResetSelectorPosition ();
			StartCoroutine (GetSelectionInput ());
		} else {
			StopCoroutine (GetSelectionInput ());
		}
	}

	void ResetSelectorPosition(){
		int resetIndex = 0; //first index
		if (resetToRandomPosition) {
			resetIndex = Random.Range(0, positionTransforms.Length);
		}

		if (positionTransforms.Length >= 0) {
			selectorVisuals.transform.position = positionTransforms[resetIndex].position;
			currPositionIndex = resetIndex;
			exp.trialController.LogAnswerSelectorPositionChanged (GetMemoryState());

			if(Config_CoinTask.isJuice){
				StopCoroutine(selectorVisuals.GetComponent<TextMeshColorCycler>().CycleColors());
				StartCoroutine(selectorVisuals.GetComponent<TextMeshColorCycler>().CycleColors());
			}
		}
		SetExplanationColors ();
	}
		

	//MODIFIED FROM BOXSWAPPER.CS
	IEnumerator GetSelectionInput(){
		bool isInput = false;
		float delayTime = 0.3f;
		float currDelayTime = 0.0f;

		float refreshTimer = 0f;
		float maxRefreshTime = 1f;

		float prevPosX = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch).x;
		while (true) {

			if(refreshTimer < maxRefreshTime)
			{
				refreshTimer += Time.deltaTime;
			}
			else
			{
				refreshTimer = 0f;
				prevPosX = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch).x;
			}
			if (!isInput) {
				//float horizAxisInput = Input.GetAxis (Config_CoinTask.HorizontalAxisName);
				float horizAxisInput = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch).x - prevPosX;
				if (horizAxisInput > 0) {
					Move (1);
					isInput = true;
				} 
				else if (horizAxisInput < 0) {
					Move (-1);
					isInput = true;
				} 
				else if (horizAxisInput == 0) {
					isInput = false;
				}
				
			}
			
			else{
				if(currDelayTime < delayTime){
					currDelayTime += Time.deltaTime;
				}
				else{
					currDelayTime = 0.0f;
					isInput = false;
				}
				
			}
			
			yield return 0;
		}
	}

	public Config_CoinTask.MemoryState GetMemoryState(){
		if (currPositionIndex == 0) {
			return Config_CoinTask.MemoryState.yes;
		} else if (currPositionIndex == 1) {
			return Config_CoinTask.MemoryState.maybe;
		} else {
			return Config_CoinTask.MemoryState.no;
		}
	}

	void Move(int indicesToMove){
		int oldPositionIndex = currPositionIndex;

		bool isMoved = true;

		currPositionIndex += indicesToMove;

		if (currPositionIndex < 0) {
			currPositionIndex = 0;
			isMoved = false;
		}
		else if (currPositionIndex > positionTransforms.Length - 1){
			currPositionIndex = positionTransforms.Length - 1;
			isMoved = false;
		}

		//play audio if the selector moved
		if (isMoved) {
			AudioController.PlayAudio(selectionSwitchAudio);

			SetExplanationColors();
		}

		//TODO: make nice smooth movement with a coroutine.
		selectorVisuals.transform.position = positionTransforms [currPositionIndex].position;

		if (currPositionIndex != oldPositionIndex) {
			exp.trialController.LogAnswerSelectorPositionChanged (GetMemoryState());
		}
			
	}

	void SetExplanationColors(){
		float lerpTime = 0.5f;

		switch(currPositionIndex){
		case 0:
			StartCoroutine(yesExplanationColorChanger.LerpChangeColor(selectedColor, lerpTime));
			StartCoroutine(maybeExplanationColorChanger.LerpChangeColor(deselectedColor, lerpTime));
			StartCoroutine(noExplanationColorChanger.LerpChangeColor(deselectedColor, lerpTime));
			break;
			
		case 1:
			StartCoroutine(yesExplanationColorChanger.LerpChangeColor(deselectedColor, lerpTime));
			StartCoroutine(maybeExplanationColorChanger.LerpChangeColor(selectedColor, lerpTime));
			StartCoroutine(noExplanationColorChanger.LerpChangeColor(deselectedColor, lerpTime));
			break;
			
		case 2:
			StartCoroutine(yesExplanationColorChanger.LerpChangeColor(deselectedColor, lerpTime));
			StartCoroutine(maybeExplanationColorChanger.LerpChangeColor(deselectedColor, lerpTime));
			StartCoroutine(noExplanationColorChanger.LerpChangeColor(selectedColor, lerpTime));
			break;
		}
	}

	/*void SetExplanationText(float colorLerpTime){
		if(GetMemoryState()){
			StartCoroutine(SetYesExplanationActive(colorLerpTime));
		}
		else if(IsNoPosition()){
			StartCoroutine(SetNoExplanationActive(colorLerpTime));
		}
	}*/

	//TODO: combine these next two methods.
	/*IEnumerator SetYesExplanationActive(float colorLerpTime){
		//TODO: REFACTOR.
		if(yesExplanationText && noExplanationText && yesExplanationColorChanger && noExplanationColorChanger){
			yesExplanationColorChanger.StopLerping();
			noExplanationColorChanger.StopLerping();

			yield return 0;

			StartCoroutine(yesExplanationColorChanger.LerpChangeColor( new Color(yesExplanationText.color.r, yesExplanationText.color.g, yesExplanationText.color.b, 1.0f), colorLerpTime));
			StartCoroutine(noExplanationColorChanger.LerpChangeColor( new Color(noExplanationText.color.r, noExplanationText.color.g, noExplanationText.color.b, 0.0f), colorLerpTime));
		}
	}

	IEnumerator SetNoExplanationActive(float colorLerpTime){
		//TODO: REFACTOR.
		if(yesExplanationText && noExplanationText && yesExplanationColorChanger && noExplanationColorChanger){
			yesExplanationColorChanger.StopLerping();
			noExplanationColorChanger.StopLerping();

			yield return 0;

			StartCoroutine(yesExplanationColorChanger.LerpChangeColor( new Color(yesExplanationText.color.r, yesExplanationText.color.g, yesExplanationText.color.b, 0.0f), colorLerpTime));
			StartCoroutine(noExplanationColorChanger.LerpChangeColor( new Color(noExplanationText.color.r, noExplanationText.color.g, noExplanationText.color.b, 1.0f), colorLerpTime));
		}
	}*/
}
