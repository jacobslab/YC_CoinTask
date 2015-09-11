using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnvironmentPositionSelector : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public GameObject PositionSelector;
	public GameObject CorrectPositionIndicator;
	public GameObject RadiusButtonUI;
	public Text RadiusButtonUIText; //should say "select special object" or something of the sort.
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
		bool positionCloseToTower2 = CheckPositionsClose (epsilon, exp.player.transform.position, exp.player.controls.towerPositionTransform2.position);

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
		float epsilon = 0.01f;

		Vector3 vertAmountVec = PositionSelector.transform.forward * amountVertical;
		Vector3 horizAmountVec = PositionSelector.transform.right * amountHorizontal;

		bool wouldBeInWallsVert = exp.environmentController.CheckWithinWalls (PositionSelector.transform.position + (vertAmountVec), Config_CoinTask.objectToWallBuffer);
		bool wouldBeInWallsHoriz = exp.environmentController.CheckWithinWalls (PositionSelector.transform.position + (horizAmountVec), Config_CoinTask.objectToWallBuffer); 

		if (wouldBeInWallsVert) {
			PositionSelector.transform.position += vertAmountVec;
		} /*else {
			//move to edge
			if( amountVertical < -epsilon ){
				float vertDist = exp.environmentController.GetDistanceFromEdge( PositionSelector.transform.position, Config_CoinTask.objectToWallBuffer, -PositionSelector.transform.forward);
				PositionSelector.transform.position += PositionSelector.transform.forward*vertDist;
			}
			else if(amountVertical > epsilon ){
				float vertDist = exp.environmentController.GetDistanceFromEdge( PositionSelector.transform.position, Config_CoinTask.objectToWallBuffer, PositionSelector.transform.forward);
				PositionSelector.transform.position += PositionSelector.transform.forward*vertDist;
			}
		}*/
		if (wouldBeInWallsHoriz) {
			PositionSelector.transform.position += horizAmountVec;
		} /*else {
			//move to edge
			if( amountHorizontal < -epsilon ){
				float horizDist = exp.environmentController.GetDistanceFromEdge( PositionSelector.transform.position, Config_CoinTask.objectToWallBuffer, PositionSelector.transform.right);
				PositionSelector.transform.position += PositionSelector.transform.right*horizDist;
			}
			else if( amountHorizontal > epsilon ){
				float horizDist = exp.environmentController.GetDistanceFromEdge( PositionSelector.transform.position, Config_CoinTask.objectToWallBuffer, -PositionSelector.transform.right);
				PositionSelector.transform.position -= PositionSelector.transform.right*horizDist;
			}
		}*/
	}

	bool hasSelectedRadius = false;
	public IEnumerator WaitForRadiusSelection( string displayText ){
		EnableRadiusButtonUI(true);
		hasSelectedRadius = false;
		RadiusButtonUIText.text = displayText;
		while(!hasSelectedRadius){
			yield return 0;
		}
		EnableRadiusButtonUI(false);
	}

	void EnableRadiusButtonUI(bool shouldEnable){
		RadiusButtonUI.SetActive(shouldEnable);
	}

	public void SetBigRadius(){
		float currentRadius = Config_CoinTask.bigSelectionRadius;
		PositionSelector.transform.localScale = new Vector3 (currentRadius, PositionSelector.transform.localScale.y, currentRadius);
		hasSelectedRadius = true;
	}

	public void SetSmallRadius(){
		float currentRadius = Config_CoinTask.smallSelectionRadius;
		PositionSelector.transform.localScale = new Vector3 (currentRadius, PositionSelector.transform.localScale.y, currentRadius);
		hasSelectedRadius = true;
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

	public void EnableCorrectIndicator(bool shouldEnable){
		CorrectPositionIndicator.SetActive (shouldEnable);
	}

	public void EnableSelectionIndicator(bool shouldEnable){
		PositionSelector.SetActive (shouldEnable);
	}

	public void DisableMovement(){
		myArc.gameObject.SetActive(false);
		shouldSelect = false;
	}
}
