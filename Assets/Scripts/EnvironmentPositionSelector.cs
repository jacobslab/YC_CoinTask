using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnvironmentPositionSelector : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public GameObject PositionSelector;
	public GameObject PositionSelectorVisuals;
	public GameObject AllVisuals; //includes radius & arc --> for visibility toggling
	public GameObject CorrectPositionIndicator;
	public ArcGenerator myArc;
	public TextMesh RadiusPointText;

	public enum SelectionRadiusType{
		big,
		small,
		none
	}
	public SelectionRadiusType currentRadiusType;

	bool shouldSelect;
	float selectionMovementSpeed = 4.0f;

	// Use this for initialization
	void Start () {
		DisableMovement ();
		EnableSelectionIndicator (false);
		EnableCorrectIndicator (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (shouldSelect) {
			GetInput();
			myArc.AdjustArc( PositionSelector.transform.position, exp.player.transform.position - Vector3.up*exp.player.controls.startPositionTransform1.position.y ); //move it down a bit...
		}
	}

	void GetInput(){
		float verticalAxisInput = Input.GetAxis ("Vertical");
		float horizontalAxisInput = Input.GetAxis ("Horizontal");

		float epsilon = 0.1f;
		bool positionCloseToTower1 = CheckPositionsClose (epsilon, exp.player.transform.position, exp.player.controls.towerPositionTransform1.position);
		bool positionCloseToTower2 = CheckPositionsClose (epsilon, exp.player.transform.position, exp.player.controls.towerPositionTransform2.position);

		if (positionCloseToTower1) {
			Move (verticalAxisInput * selectionMovementSpeed, horizontalAxisInput * selectionMovementSpeed);
		} 
		else if (positionCloseToTower2) {
			Move (-verticalAxisInput * selectionMovementSpeed, -horizontalAxisInput * selectionMovementSpeed);
		}

		//Get Radius Selection Input

		if (Input.GetButtonDown("A Button")) {
			ChangeRadiusSize();
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
		float epsilon = 0.01f;

		Vector3 vertAmountVec = PositionSelector.transform.forward * amountVertical;
		Vector3 horizAmountVec = PositionSelector.transform.right * amountHorizontal;

		bool wouldBeInWallsVert = exp.environmentController.CheckWithinWallsVert (PositionSelector.transform.position + (vertAmountVec), Config_CoinTask.objectToWallBuffer);
		bool wouldBeInWallsHoriz = exp.environmentController.CheckWithinWallsHoriz (PositionSelector.transform.position + (horizAmountVec), Config_CoinTask.objectToWallBuffer); 

		if (amountVertical != 0) {
			int a = 0;
		}
		if (amountHorizontal != 0) {
			int a = 0;
		}

		if (wouldBeInWallsVert) {
			PositionSelector.transform.position += vertAmountVec;
		} else {
			//move to edge
			if( amountVertical < -epsilon ){

				float vertDist = exp.environmentController.GetDistanceFromEdge( PositionSelector, Config_CoinTask.objectToWallBuffer, -PositionSelector.transform.forward);
				PositionSelector.transform.position -= PositionSelector.transform.forward*vertDist;
			}
			else if(amountVertical > epsilon ){
				float vertDist = exp.environmentController.GetDistanceFromEdge( PositionSelector, Config_CoinTask.objectToWallBuffer, PositionSelector.transform.forward);
				PositionSelector.transform.position -= PositionSelector.transform.forward*vertDist;
			}
		}
		if (wouldBeInWallsHoriz) {
			PositionSelector.transform.position += horizAmountVec;
		} else {
			//move to edge
			if( amountHorizontal < -epsilon ){
				float horizDist = exp.environmentController.GetDistanceFromEdge( PositionSelector, Config_CoinTask.objectToWallBuffer, -PositionSelector.transform.right);
				PositionSelector.transform.position -= PositionSelector.transform.right*horizDist;
			}
			else if( amountHorizontal > epsilon ){
				float horizDist = exp.environmentController.GetDistanceFromEdge( PositionSelector, Config_CoinTask.objectToWallBuffer, PositionSelector.transform.right);
				PositionSelector.transform.position -= PositionSelector.transform.right*horizDist;
			}
		}
	}


	void ChangeRadiusSize(){
		float radiusSize = 0.0f;
		if (currentRadiusType == SelectionRadiusType.none) { //none --> big
			AllVisuals.GetComponent<VisibilityToggler>().SetAlpha(1.0f);
			radiusSize = Config_CoinTask.bigSelectionSize;
			currentRadiusType = SelectionRadiusType.big;
			SetRadiusSize (radiusSize);

			SetRadiusText();
		}
		else if(currentRadiusType == SelectionRadiusType.big){ //big --> small
			radiusSize = Config_CoinTask.smallSelectionSize;
			currentRadiusType = SelectionRadiusType.small;
			SetRadiusSize (radiusSize);

			SetRadiusText();
		}
		else if (currentRadiusType == SelectionRadiusType.small){ //small --> none
			AllVisuals.GetComponent<VisibilityToggler>().SetAlpha(0.0f);
			currentRadiusType = SelectionRadiusType.none;

			/*radiusSize = Config_CoinTask.smallSelectionSize;
			currentRadiusType = SelectionRadiusType.small;
			SetRadiusSize (radiusSize);
*/
			SetRadiusText();
		}

	}

	void SetRadiusSize( float size ){
		PositionSelectorVisuals.transform.localScale = new Vector3 (size, PositionSelectorVisuals.transform.localScale.y, size);
	}

	void SetRadiusText(){
		string pointsText = " POINTS";

		if (currentRadiusType == SelectionRadiusType.none) { //none
			RadiusPointText.text = ScoreController.memoryScoreNoChoice + pointsText;
		}
		else if(currentRadiusType == SelectionRadiusType.big){ //big
			RadiusPointText.text = ScoreController.memoryScoreMedium + pointsText;
		}
		else if (currentRadiusType == SelectionRadiusType.small){ //small
			RadiusPointText.text = ScoreController.memoryScoreBest + pointsText;
		}
	}

	public bool GetRadiusOverlap(Vector3 position){
		float distance = (position - PositionSelector.transform.position).magnitude;
		if (distance < PositionSelectorVisuals.transform.localScale.x) {
			return true;
		}

		return false;
	}

	public Vector3 GetSelectorPosition(){
		return PositionSelector.transform.position;
	}
	
	public void EnableSelection(){
		shouldSelect = true;
		SetRadiusText ();
		myArc.gameObject.SetActive(true);
		PositionSelectorVisuals.SetActive (true);
	}

	public void EnableCorrectIndicator(bool shouldEnable){
		CorrectPositionIndicator.SetActive (shouldEnable);
	}

	public void EnableSelectionIndicator(bool shouldEnable){
		if (!shouldEnable) {
			RadiusPointText.text = "";
		} else {
			SetRadiusText();
		}
		PositionSelectorVisuals.SetActive (shouldEnable);
	}

	public void DisableMovement(){
		myArc.gameObject.SetActive(false);
		shouldSelect = false;
	}
}
