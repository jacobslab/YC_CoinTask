using UnityEngine;
using System.Collections;

public class EnvironmentPositionSelector : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public GameObject PositionSelector;
	public GameObject PositionRadius;
	public GameObject CorrectPositionIndicator;
	public ArcGenerator myArc;
	
	float smallRadius = 4.0f;
	float bigRadius = 8.0f;
	float currentSetRadius; //TODO: allow player to choose either big or small radius.

	bool shouldSelect;
	float selectionMovementSpeed = 4.0f;

	// Use this for initialization
	void Start () {
		currentSetRadius = smallRadius;
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

		if (exp.player.transform.position == exp.player.controls.towerPositionTransform1.position) {
			Move (verticalAxisInput * selectionMovementSpeed, horizontalAxisInput * selectionMovementSpeed);
		} 
		else if (exp.player.transform.position == exp.player.controls.towerPositionTransform2.position) {
			Move (-verticalAxisInput * selectionMovementSpeed, -horizontalAxisInput * selectionMovementSpeed);
		}
	}

	void Move(float amountVertical, float amountHorizontal){
		Vector3 vertAmountVec = PositionSelector.transform.forward * amountVertical;
		Vector3 horizAmountVec = PositionSelector.transform.right * amountHorizontal;

		bool wouldBeInWallsVert = exp.environmentController.CheckWithinWalls (PositionSelector.transform.position + (vertAmountVec), Config_CoinTask.objectToWallBuffer);
		bool wouldBeInWallsHoriz = exp.environmentController.CheckWithinWalls (PositionSelector.transform.position + (horizAmountVec) , Config_CoinTask.objectToWallBuffer); 

		if ( wouldBeInWallsVert ){
			PositionSelector.transform.position += vertAmountVec;
		}
		if( wouldBeInWallsHoriz ){
			PositionSelector.transform.position += horizAmountVec;
		}
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
