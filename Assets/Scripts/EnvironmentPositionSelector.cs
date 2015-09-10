using UnityEngine;
using System.Collections;

public class EnvironmentPositionSelector : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public GameObject PositionSelector;
	public GameObject CorrectPositionIndicator;
	public ArcGenerator myArc;

	public enum SelectionRadiusType{
		big,
		small
	}
	public SelectionRadiusType currentRadiusType;

	bool shouldSelect;
	float selectionMovementSpeed = 4.0f;

	// Use this for initialization
	void Start () {
		//TODO: allow player to choose either big or small radius.
		float currentRadius = Config_CoinTask.smallSelectionRadius;
		currentRadiusType = Config_CoinTask.currentSelectionRadiusType; //TODO: TAKE THIS SELECTION OUT OF MAIN MENU/CONFIG
		if (currentRadiusType == SelectionRadiusType.big) {
			currentRadius = Config_CoinTask.bigSelectionRadius;
		}
		Debug.Log (currentRadius);
		PositionSelector.transform.localScale = new Vector3 (currentRadius, PositionSelector.transform.localScale.y, currentRadius);

		DisableMovement ();
		EnableSelectionIndicator (false);
		EnableCorrectIndicator (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (shouldSelect) {
			GetInput();
			myArc.GenerateArc( exp.player.transform.position - Vector3.up*exp.player.controls.startPositionTransform1.position.y ); //move it down a bit...
		}
	}

	void GetInput(){
		float verticalAxisInput = Input.GetAxis ("Vertical");
		float horizontalAxisInput = Input.GetAxis ("Horizontal");

		float epsilon = 0.1f;
		bool positionCloseToTower1 = CheckPositionsClose (epsilon, exp.player.transform.position, exp.player.controls.towerPositionTransform1.position);
		bool positionCloseToTower2 = CheckPositionsClose (epsilon, exp.player.transform.position, exp.player.controls.towerPositionTransform1.position);

		if (positionCloseToTower1) {
			Move (verticalAxisInput * selectionMovementSpeed, horizontalAxisInput * selectionMovementSpeed);
		} 
		else if (positionCloseToTower2) {
			Move (-verticalAxisInput * selectionMovementSpeed, -horizontalAxisInput * selectionMovementSpeed);
		}
	}

	bool CheckPositionsClose(float epsilon, Vector3 pos1, Vector3 pos2){
		float distance = (pos1 - pos2).magnitude;
		if (distance < epsilon) {
			return true;
		}
		return false;
	}

	void Move(float amountVertical, float amountHorizontal){
		Vector3 vertAmountVec = PositionSelector.transform.forward * amountVertical;
		Vector3 horizAmountVec = PositionSelector.transform.right * amountHorizontal;

		bool wouldBeInWallsVert = exp.environmentController.CheckWithinWalls (PositionSelector.transform.position + (vertAmountVec), Config_CoinTask.objectToWallBuffer);
		bool wouldBeInWallsHoriz = exp.environmentController.CheckWithinWalls (PositionSelector.transform.position + (horizAmountVec), Config_CoinTask.objectToWallBuffer); 

		if (wouldBeInWallsVert) {
			PositionSelector.transform.position += vertAmountVec;
		}
		if (wouldBeInWallsHoriz) {
			PositionSelector.transform.position += horizAmountVec;
		}
	}

	public bool GetRadiusOverlap(Vector3 position){
		float distance = (position - PositionSelector.transform.position).magnitude;
		if (distance < PositionSelector.transform.localScale.x) {
			return true;
		}

		return false;
	}

	public Vector3 GetSelectorPosition(){
		return PositionSelector.transform.position;
	}

	//enable or disable selection
	public void EnableSelection(){
		shouldSelect = true;
		myArc.gameObject.SetActive(true);
		PositionSelector.SetActive (true);
	}

	public void EnableCorrectIndicator(bool isEnabled){
		CorrectPositionIndicator.SetActive (isEnabled);
	}

	public void EnableSelectionIndicator(bool isEnabled){
		PositionSelector.SetActive (isEnabled);
	}

	public void DisableMovement(){
		myArc.gameObject.SetActive(false);
		shouldSelect = false;
	}
}
