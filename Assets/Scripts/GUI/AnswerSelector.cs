using UnityEngine;
using System.Collections;

public class AnswerSelector : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	bool shouldCheckForInput = false;

	public Transform[] positionTransforms; //should be put in order of left to right
	public GameObject selectorVisuals;

	public AudioSource selectionSwitchAudio;

	int currPositionIndex = 0;
	int yesIndex = 0;
	int noIndex = 1;

	// Use this for initialization
	void Start () {
		ResetSelectorPosition ();
	}
	
	// Update is called once per frame
	void Update () {
		if (shouldCheckForInput) {
			CheckForInput();
		}
	}

	public void SetShouldCheckForInput(bool shouldCheck){
		shouldCheckForInput = shouldCheck;
		if (shouldCheck) {
			ResetSelectorPosition ();
		}
	}

	void ResetSelectorPosition(){
		exp.trialController.LogAnswerSelectorPositionChanged (IsYesPosition ());
		if (positionTransforms.Length > 0) {
			selectorVisuals.transform.position = positionTransforms[0].position;
			currPositionIndex = 0;

			if(Config_CoinTask.isJuice){
				StopCoroutine(selectorVisuals.GetComponent<TextMeshColorCycler>().CycleColors());
				StartCoroutine(selectorVisuals.GetComponent<TextMeshColorCycler>().CycleColors());
			}
		}
	}

	void CheckForInput(){
		//keyboard input
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			Move (1);
		}
		else if (Input.GetKeyDown (KeyCode.LeftArrow)){
			Move (-1);
		}
		else {
			//joystick input
			if (Input.GetAxis ("Horizontal") > 0.3f) {
				Move(1);
			}
			else if (Input.GetAxis ("Horizontal") < -0.3f){
				Move (-1);
			}
		}
	}

	public bool IsYesPosition(){
		if (currPositionIndex == yesIndex) {
			return true;
		}
		return false;
	}

	public bool IsNoPosition(){
		if (currPositionIndex == noIndex) {
			return true;
		}
		return false;
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
		}

		//TODO: make nice smooth movement with a coroutine.
		selectorVisuals.transform.position = positionTransforms [currPositionIndex].position;

		if (currPositionIndex != oldPositionIndex) {
			exp.trialController.LogAnswerSelectorPositionChanged (IsYesPosition ());
		}
	}
}
