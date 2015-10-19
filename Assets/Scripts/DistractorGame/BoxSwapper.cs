using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxSwapper : MonoBehaviour {

	public GameObject boxSelector;
	Quaternion boxSelectorOrigRot;
	int selectedBoxIndex = 0;
	bool shouldSelect = false;

	public GameObject rewardObject;
	int boxRewardIndex = 0;

	public GameObject[] boxes; //BE THREE BOXES PLEASE.
	public Transform[] boxStartPositions;

	float startBoxHeight = 2.0f;

	// Use this for initialization
	void Start () {
		boxSelectorOrigRot = boxSelector.transform.rotation;
	}

	public void Init(){
		boxSelector.SetActive (false);
		boxSelector.transform.rotation = boxSelectorOrigRot;
		InitBoxPositions();
		InitRewardPosition();
	}

	void InitBoxPositions(){
		boxes[0].transform.position = boxStartPositions[0].position + Vector3.up*startBoxHeight;
		boxes[1].transform.position = boxStartPositions[1].position + Vector3.up*startBoxHeight;
		boxes[2].transform.position = boxStartPositions[2].position + Vector3.up*startBoxHeight;

		boxes [0].GetComponent<BoxMover> ().SetBoxLocationIndex(0);
		boxes [1].GetComponent<BoxMover> ().SetBoxLocationIndex(1);
		boxes [2].GetComponent<BoxMover> ().SetBoxLocationIndex(2);
	}

	void InitRewardPosition(){
		int randomPosIndex = Random.Range(0, boxStartPositions.Length);
		boxRewardIndex = randomPosIndex;
		rewardObject.transform.position = boxStartPositions[randomPosIndex].transform.position;
	}

	public IEnumerator LowerBoxes(){
		float currTime = 0.0f;
		float moveDownTime = 1.0f;
		
		float moveAmount = -startBoxHeight / moveDownTime;
		
		while(currTime < moveDownTime){
			currTime += Time.deltaTime;
			for(int i = 0; i < boxes.Length; i++){
				boxes[i].transform.position += (Vector3.up*moveAmount*Time.deltaTime);
			}
			yield return 0;
		}

	}

	//TODO: combine with LowerBoxes() because they're basically the same function.
	public IEnumerator RaiseBoxes(){
		float currTime = 0.0f;
		float moveDownTime = 1.0f;
		
		float moveAmount = startBoxHeight / moveDownTime;
		
		while(currTime < moveDownTime){
			currTime += Time.deltaTime;
			for(int i = 0; i < boxes.Length; i++){
				boxes[i].transform.position += (Vector3.up*moveAmount*Time.deltaTime);
			}
			yield return 0;
		}

		float liftPause = 1.0f;
		yield return new WaitForSeconds (liftPause);
		
		InitBoxPositions();
	}

	// Update is called once per frame
	void Update () {
		if(shouldSelect){
			GetSelectionInput();
		}
	}

	void GetSelectionInput(){
		if(Input.GetKeyDown(KeyCode.RightArrow)){
			selectedBoxIndex++;
			if(selectedBoxIndex > boxStartPositions.Length - 1){
				selectedBoxIndex = boxStartPositions.Length - 1;
			}
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow)){
			selectedBoxIndex--;
			if(selectedBoxIndex < 0){
				selectedBoxIndex = 0;
			}
		}
		boxSelector.transform.position = boxStartPositions[selectedBoxIndex].transform.position;
	}

	public IEnumerator WaitForBoxSelection(){
		bool actionButtonPressed = false;
		shouldSelect = true;

		boxSelector.SetActive (true);

		while(!actionButtonPressed){
			if(Input.GetKeyDown(KeyCode.Space)){
				actionButtonPressed = true;
				shouldSelect = false;
			}
			yield return 0;
		}

		yield return 0;
		if(IsSelectedBoxCorrect()){
			Debug.Log("You got the reward!");
			Experiment_CoinTask.Instance.scoreController.AddBoxSwapperPoints();
			StartCoroutine(SpinSelector());
		}

		rewardObject.transform.position = boxes[boxRewardIndex].transform.position;
		rewardObject.SetActive(true);
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

		for(int swapNum = 0; swapNum < numTimes; swapNum++){
			List<Vector3> boxPositions = new List<Vector3>();
			boxPositions.Add(boxStartPositions[0].position);
			boxPositions.Add(boxStartPositions[1].position);
			boxPositions.Add(boxStartPositions[2].position);

			List<BoxMover.MoveType> moveTypes = new List<BoxMover.MoveType>();
			moveTypes.Add(BoxMover.MoveType.moveOverArc);
			moveTypes.Add(BoxMover.MoveType.moveStraight);
			moveTypes.Add(BoxMover.MoveType.moveUnderArc);

			for(int i = 0; i < boxes.Length; i++){
				int randomPosIndex = Random.Range(0, boxPositions.Count);
				int randomMoveTypeIndex = Random.Range(0, moveTypes.Count);
				BoxMover currBoxMover = boxes[i].GetComponent<BoxMover>();

				int newLocationIndex = 0;
				if(boxPositions[randomPosIndex] == boxStartPositions[0].position){
					newLocationIndex = 0;
				}
				else if(boxPositions[randomPosIndex] == boxStartPositions[1].position){
					newLocationIndex = 1;
				}
				else if(boxPositions[randomPosIndex] == boxStartPositions[2].position){
					newLocationIndex = 2;
				}

				//set movetype
				currBoxMover.SetMoveType(moveTypes[randomMoveTypeIndex]);
				moveTypes.RemoveAt(randomMoveTypeIndex);

				//move first, based on current location index
				currBoxMover.Move(boxPositions[randomPosIndex]);
				//now that we've moved, set the location index for the new, moved to position
				currBoxMover.SetBoxLocationIndex(newLocationIndex);
				boxPositions.RemoveAt(randomPosIndex);
			}

			yield return new WaitForSeconds(Config_CoinTask.boxMoveTime);
			yield return 0; //wait an extra frame to make sure the boxes get into the correct positions
		}

		rewardObject.transform.parent = rewardObject.transform.parent.parent; //set it equal to the box's parent.
	}


	IEnumerator SpinSelector(){
		float angle = 6.0f;
		while (true) {
			boxSelector.transform.RotateAround(boxSelector.transform.position, Vector3.up, angle);
			yield return 0;
		}
	}
}
