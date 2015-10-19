using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxSwapper : MonoBehaviour {

	public GameObject boxSelector;
	int selectedBoxIndex = 0;

	public GameObject rewardObject;
	int boxRewardIndex = 0;

	public GameObject[] boxes; //BE THREE BOXES PLEASE.
	public Transform[] boxStartPositions;

	float startBoxHeight = 2.0f;

	bool shouldSelect = false;

	// Use this for initialization
	void Start () {

	}

	public void Init(){
		InitBoxPositions();
		InitRewardPosition();
	}

	void InitBoxPositions(){
		boxes[0].transform.position = boxStartPositions[0].position + Vector3.up*startBoxHeight;
		boxes[1].transform.position = boxStartPositions[1].position + Vector3.up*startBoxHeight;
		boxes[2].transform.position = boxStartPositions[2].position + Vector3.up*startBoxHeight;
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

			for(int i = 0; i < boxes.Length; i++){
				int randomPosIndex = Random.Range(0, boxPositions.Count);
				BoxMover currBoxMover = boxes[i].GetComponent<BoxMover>();
				SetBoxMoveType(currBoxMover);

				currBoxMover.Move(boxPositions[randomPosIndex]);
				boxPositions.RemoveAt(randomPosIndex);
			}

			yield return new WaitForSeconds(1.0f);
		}

		rewardObject.transform.parent = rewardObject.transform.parent.parent; //set it equal to the box's parent.
	}

	void SetBoxMoveType(BoxMover box){
		BoxMover boxMover = box.GetComponent<BoxMover>();
		if(box.transform.position == boxStartPositions[0].position){
			boxMover.myMoveType = BoxMover.MoveType.moveUnderArc;
		}
		else if(box.transform.position == boxStartPositions[1].position){
			boxMover.myMoveType = BoxMover.MoveType.moveStraight;
		}
		else if(box.transform.position == boxStartPositions[2].position){
			boxMover.myMoveType = BoxMover.MoveType.moveOverArc;
		}
	}
}
