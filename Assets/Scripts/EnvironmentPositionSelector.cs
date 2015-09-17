using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnvironmentPositionSelector : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public GameObject PositionSelector;
	public GameObject PositionSelectorVisuals;
	public GameObject CorrectPositionIndicator;
	public ArcGenerator myArc;
	public TextMesh RadiusPointText;

	public PositionSelectorLogTrack logTrack;

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
		EnableSelection(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (shouldSelect) {

			GetRadiusSelectionInput();

			if(currentRadiusType != SelectionRadiusType.none){
				GetMovementInput();
				//myArc.AdjustArc( exp.player.transform.position - Vector3.up*exp.player.controls.startPositionTransform1.position.y, PositionSelector.transform.position ); //move it down a bit...
			}
		}
	}

	void GetMovementInput(){
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
	}

	void GetRadiusSelectionInput(){
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

	//TODO: put these size change functions in a separate script so that the visuals control their own size -- will be easier for duplication of indicators
	void ChangeRadiusSize(){
		if (currentRadiusType == SelectionRadiusType.none) { //none --> big
			SetRadiusSizeBig();
		}
		else if(currentRadiusType == SelectionRadiusType.big){ //big --> small
			SetRadiusSizeSmall();
		}
		else if (currentRadiusType == SelectionRadiusType.small){ //small --> none
			SetRadiusSizeNone();
		}

	}

	public void SetRadiusSize (SelectionRadiusType newRadiusType){
		if (newRadiusType == SelectionRadiusType.none) {
			SetRadiusSizeNone();
		} 
		else if (newRadiusType == SelectionRadiusType.small) {
			SetRadiusSizeSmall();
		} 
		else if (newRadiusType == SelectionRadiusType.big) {
			SetRadiusSizeBig();
		}
	}

	void SetRadiusSizeBig(){

		float radiusSize = 0.0f;

		PositionSelectorVisuals.GetComponent<VisibilityToggler>().TurnVisible(true);
		myArc.GetComponent<VisibilityToggler>().TurnVisible(true);
		radiusSize = Config_CoinTask.bigSelectionSize;
		currentRadiusType = SelectionRadiusType.big;
		SetRadiusSize (radiusSize);
		
		SetRadiusText();
	}

	void SetRadiusSizeSmall(){

		float radiusSize = 0.0f;

		PositionSelectorVisuals.GetComponent<VisibilityToggler>().TurnVisible(true);
		myArc.GetComponent<VisibilityToggler>().TurnVisible(true);
		radiusSize = Config_CoinTask.smallSelectionSize;
		currentRadiusType = SelectionRadiusType.small;
		SetRadiusSize (radiusSize);
		
		SetRadiusText();
	}

	void SetRadiusSizeNone(){

		PositionSelectorVisuals.GetComponent<VisibilityToggler>().TurnVisible(false);
		myArc.GetComponent<VisibilityToggler>().TurnVisible(false);
		currentRadiusType = SelectionRadiusType.none;
		
		SetRadiusText();
	}

	void SetRadiusSize( float size ){
		PositionSelectorVisuals.transform.localScale = new Vector3 (size, PositionSelectorVisuals.transform.localScale.y, size);
	}

	void SetRadiusText(){

		if (!shouldSelect) {
			RadiusPointText.text = "";
		}

		else{
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
	}

	public bool GetRadiusOverlap(Vector3 correctPosition){
		float distance = (correctPosition - PositionSelector.transform.position).magnitude;
		float positionSelectorRadius = PositionSelectorVisuals.transform.localScale.x / 2.0f;
		float correctRadius = CorrectPositionIndicator.transform.localScale.x / 2.0f;
		if (distance < positionSelectorRadius + correctRadius) {
			return true;
		}

		return false;
	}

	public Vector3 GetSelectorPosition(){
		return PositionSelector.transform.position;
	}
	
	public void EnableSelection(bool shouldEnable){
		shouldSelect = shouldEnable;
		EnableSelectionIndicator (shouldEnable);
	}

	void EnableSelectionIndicator(bool shouldEnable){
		if (currentRadiusType != SelectionRadiusType.none) {
			PositionSelectorVisuals.GetComponent<VisibilityToggler> ().TurnVisible (shouldEnable);
			myArc.GetComponent<VisibilityToggler> ().TurnVisible (shouldEnable);
		}
		SetRadiusText ();
	}
	
}
