using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxSwapper : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } } 

	public AudioSource swapSound;
	public AudioSource selectorSwitchSound;
	public AudioSource correctAnswerSound;
	public AudioSource wrongAnswerSound;


	public GameObject boxSelector;
	public GameObject boxSelectorVisuals;
	public TextMesh boxSelectorText;

	Quaternion boxSelectorVisualsOrigRot;
	int selectedBoxIndex = 0;
	bool shouldSelect = false;
	bool showingFeedback = false;

	public GameObject rewardObject;
	int boxRewardIndex = 0;

	public GameObject[] boxes; //BE THREE BOXES PLEASE.
	public Transform[] boxStartPositions;

	float boxScale = 1.0f; // set in start
	float startBoxHeightMult = 2.0f;


	float raiseAndLowerTime = 0.75f;


	// Use this for initialization
	void Start () {
		//Set box scale
		boxScale = boxes [0].transform.parent.localScale.y;

		boxSelectorVisualsOrigRot = boxSelectorVisuals.transform.rotation;
	}

	public void Init(){
		showingFeedback = false;

		boxSelector.GetComponent<VisibilityToggler> ().TurnVisible (false);
		boxSelectorText.text = "";
		boxSelectorVisuals.transform.rotation = boxSelectorVisualsOrigRot;
		InitBoxPositions();
		InitRewardPosition();
	}

	void InitBoxPositions(){
		boxes[0].transform.position = boxStartPositions[0].position + Vector3.up*startBoxHeightMult*boxScale;
		boxes[1].transform.position = boxStartPositions[1].position + Vector3.up*startBoxHeightMult*boxScale;
		boxes[2].transform.position = boxStartPositions[2].position + Vector3.up*startBoxHeightMult*boxScale;

		boxes [0].GetComponent<BoxMover> ().SetBoxLocationIndex(0);
		boxes [1].GetComponent<BoxMover> ().SetBoxLocationIndex(1);
		boxes [2].GetComponent<BoxMover> ().SetBoxLocationIndex(2);
	}

	void InitRewardPosition(){
		int randomPosIndex = Random.Range(0, boxStartPositions.Length);
		boxRewardIndex = randomPosIndex;
		rewardObject.transform.position = boxStartPositions[randomPosIndex].transform.position;
	}

	public IEnumerator RaiseOrLowerBoxes(int direction, bool initBoxPositions){
		float currTime = 0.0f;
		
		float moveAmount = (direction*startBoxHeightMult*boxScale) / raiseAndLowerTime;
		
		while(currTime < raiseAndLowerTime){
			currTime += Time.deltaTime;
			for(int i = 0; i < boxes.Length; i++){
				//boxes[i].transform.position += (Vector3.up*moveAmount*Time.deltaTime);
				boxes[i].transform.localPosition += (boxes[i].transform.up*moveAmount*Time.deltaTime);
			}
			yield return 0;
		}

		if (initBoxPositions) {
			InitBoxPositions ();
		}
	}

	// Update is called once per frame
	void Update () {

	}

	IEnumerator GetSelectionInput(){
		bool isInput = false;
		float delayTime = 0.3f;
		float currDelayTime = 0.0f;
		float prevPosX = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch).x;

		while (shouldSelect) {

			if (!isInput) {
				//if (Input.GetAxis (Config_CoinTask.HorizontalAxisName) > 0) {
				if(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch).x - prevPosX> 0.1f) { 
					MoveSelector (1);
					isInput = true;
				} 
			//else if (Input.GetAxis (Config_CoinTask.HorizontalAxisName) < 0) {
			else if(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch).x -prevPosX < -0.1f) { 
					MoveSelector (-1);
					isInput = true;
				} else if (Input.GetAxis (Config_CoinTask.HorizontalAxisName) == 0) {
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

	void MoveSelector(int amount){

		bool moved = true;

		selectedBoxIndex += amount;
		if (selectedBoxIndex > boxStartPositions.Length - 1) {
			selectedBoxIndex = boxStartPositions.Length - 1;
			moved = false;
		}
		else if (selectedBoxIndex < 0) {
			selectedBoxIndex = 0;
			moved = false;
		}

		if (moved) {
			AudioController.PlayAudio(selectorSwitchSound);
		}

		boxSelector.transform.position = boxStartPositions [selectedBoxIndex].transform.position;

	}

	public IEnumerator WaitForBoxSelection(){
		bool actionButtonPressed = false;
		shouldSelect = true;

		StartCoroutine (GetSelectionInput ());

		boxSelector.GetComponent<VisibilityToggler> ().TurnVisible (true);

#if MRIVERSION
		yield return StartCoroutine(exp.trialController.WaitForMRITimeout(Config_CoinTask.maxBoxAnswerTime));
		shouldSelect = false;
#else
		while(!actionButtonPressed){
			
			if(Input.GetAxis(Config_CoinTask.ActionButtonName) != 0f){
				actionButtonPressed = true;
				shouldSelect = false;
			}
			
			yield return 0;
		}
#endif

		yield return 0;

		showingFeedback = true;

		if (IsSelectedBoxCorrect ()) {
			Debug.Log ("You got the reward!");
			AudioController.PlayAudio(correctAnswerSound);

			Experiment_CoinTask.Instance.scoreController.AddBoxSwapperPoints ();
			StartCoroutine (SpinSelector ());
			boxSelectorText.text = "+" + ScoreController.BoxSwapperPoints + "!";

		} else {
			AudioController.PlayAudio(wrongAnswerSound);

			boxSelectorText.text = "+" + ScoreController.BoxSwapperNegPoints;

			Experiment_CoinTask.Instance.scoreController.RemoveBoxSwapperPoints ();
		}

		rewardObject.transform.position = boxes[boxRewardIndex].transform.position;
	}

	bool IsSelectedBoxCorrect(){
		int minDistanceBoxIndex = 0;
		float minDistance = 0;
		for (int i = 0; i < boxes.Length; i++) {
			float distance = (boxSelector.transform.position - boxes[i].transform.position).magnitude;
			if(i == 0){
				minDistance = distance;
			}
			else if(minDistance > distance){
				minDistance = distance;
				minDistanceBoxIndex = i;
			}
			
		}
		if (minDistanceBoxIndex == boxRewardIndex) {
			return true;
		}
		return false;
	}


	public IEnumerator SwapBoxes(int numTimes){
		rewardObject.transform.parent = boxes[boxRewardIndex].transform;
		rewardObject.transform.position = rewardObject.transform.parent.position;

		for(int swapNum = 0; swapNum < numTimes; swapNum++){
			int stationaryBoxIndex = -1; //a bool to make sure no more than one box stays stationary at one time.

			List<Vector3> boxPositions = new List<Vector3>();
			//boxPositions.Add(boxStartPositions[0].position);
			//boxPositions.Add(boxStartPositions[1].position);
			//boxPositions.Add(boxStartPositions[2].position);
			boxPositions.Add(boxStartPositions[0].localPosition);
			boxPositions.Add(boxStartPositions[1].localPosition);
			boxPositions.Add(boxStartPositions[2].localPosition);

			List<BoxMover.MoveType> moveTypes = new List<BoxMover.MoveType>();
			moveTypes.Add(BoxMover.MoveType.moveOverArc);
			moveTypes.Add(BoxMover.MoveType.moveStraight);
			moveTypes.Add(BoxMover.MoveType.moveUnderArc);

			List<int> newLocationIndices = new List<int>();
			List<Vector3> moveToPositions = new List<Vector3>();

			for(int i = 0; i < boxes.Length; i++){
				AudioController.PlayAudio(swapSound);

				int randomPosIndex = Random.Range(0, boxPositions.Count);
				BoxMover currBoxMover = boxes[i].GetComponent<BoxMover>();

				int newLocationIndex = GetLocationIndex(boxPositions[randomPosIndex]);

				//set movetype of any stationary box first!
				if(currBoxMover.BoxLocationIndex == newLocationIndex && stationaryBoxIndex == -1){ //allow for one box to be stationary
					stationaryBoxIndex = i;
					currBoxMover.SetMoveType(BoxMover.MoveType.moveStraight);
					moveTypes.Remove(BoxMover.MoveType.moveStraight);
				}
				else if(currBoxMover.BoxLocationIndex == newLocationIndex && stationaryBoxIndex != -1){ //a box is already stationary!
					//NOTE: this assumes ONLY 3 boxes total! This case would only happen on the second box, thus, allowing us to know that there were only 2 indices (0,1) available anyway.
						//...Only happens on the second box because if two boxes are stationary, then the third MUST be stationary as well.
					if(randomPosIndex == 0){
						randomPosIndex = 1;
					}
					else{
						randomPosIndex = 0;
					}

					newLocationIndex = GetLocationIndex(boxPositions[randomPosIndex]);

				}

				newLocationIndices.Add(newLocationIndex);
				moveToPositions.Add(boxPositions[randomPosIndex]);
				boxPositions.RemoveAt(randomPosIndex);
			}

			//go back through boxes and set the rest of the move types
			for(int i = 0; i < boxes.Length; i++){
				BoxMover currBoxMover = boxes[i].GetComponent<BoxMover>();
				if(stationaryBoxIndex != i){
					int randomMoveTypeIndex = Random.Range(0, moveTypes.Count);

					currBoxMover.SetMoveType(moveTypes[randomMoveTypeIndex]);
					moveTypes.RemoveAt(randomMoveTypeIndex);
				}

				//move first, based on current location index
				currBoxMover.Move(moveToPositions[i]);
				//now that we've moved, set the location index for the new, moved to position
				currBoxMover.SetBoxLocationIndex(newLocationIndices[i]);
			}

			yield return new WaitForSeconds(Config_CoinTask.boxMoveTime);
			yield return 0; //wait an extra frame to make sure the boxes get into the correct positions
		}
		rewardObject.transform.position = rewardObject.transform.parent.position;
		rewardObject.transform.parent = rewardObject.transform.parent.parent; //set it equal to the box's parent.
	}

	int GetLocationIndex(Vector3 position){
		//if(position == boxStartPositions[0].position){
		if(position == boxStartPositions[0].localPosition){
			return 0;
		}
		//else if(position == boxStartPositions[1].position){
		if(position == boxStartPositions[1].localPosition){
			return 1;
		}
		//else if(position == boxStartPositions[2].position){
		if(position == boxStartPositions[2].localPosition){
			return 2;
		}

		return -1;
	}
	
	
	IEnumerator SpinSelector(){
		if (Config_CoinTask.isJuice) {
			float angle = 6.0f;
			while (showingFeedback) {
				boxSelectorVisuals.transform.RotateAround (boxSelectorVisuals.transform.position, Vector3.up, angle);
				yield return 0;
			}
		}
	}
}
