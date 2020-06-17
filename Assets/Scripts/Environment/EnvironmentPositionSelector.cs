using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnvironmentPositionSelector : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public GameObject PositionSelector;
	public GameObject PositionSelectorVisuals;
	public GameObject CorrectPositionIndicator;
	public GameObject ObjectRecallIndicator;
	public GameObject ObjectSelectorVisuals;
	public GameObject FoilRecallIndicator;
	public PositionSelectorLogTrack logTrack;
	public RingChecker ringChecker;
	public LayerMask mask = -1;
	public Color VisualsDefaultColor;
	public Color VisualsSelectColor;
	
	bool shouldSelect;
	float selectionMovementSpeed = 80.0f;

	void Awake(){
		PositionSelectorVisuals.transform.localScale = new Vector3 (Config_CoinTask.selectionDiameter, PositionSelectorVisuals.transform.localScale.y, Config_CoinTask.selectionDiameter);
	}

	// Use this for initialization
	void Start () {
		EnableSelection(false);
		EnableVisibility (false);
		EnableFoilVisibility (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (shouldSelect) {
			GetMovementInput();
		}
	}

	public IEnumerator ChoosePosition(){
		//change position selector visual colors!
		yield return StartCoroutine (PositionSelectorVisuals.GetComponent<ColorChanger> ().LerpChangeColor(VisualsSelectColor, 0.2f));
		PositionSelectorVisuals.GetComponent<ColorChanger> ().ChangeColor (VisualsDefaultColor);
		
	}

	public void Reset(){
		PositionSelector.transform.position = GetStartPosition();
	}

	Vector3 GetStartPosition(){
		Vector3 envCenter = exp.environmentController.GetEnvironmentCenter ();
		Vector3 newStartPos = new Vector3 (envCenter.x, PositionSelector.transform.position.y, envCenter.z);

		return newStartPos;

	}

	public void MoveObjectIndicatorToPosition(Vector3 destinationPosition)
	{
		ObjectSelectorVisuals.transform.position = new Vector3(destinationPosition.x,PositionSelectorVisuals.transform.position.y,destinationPosition.z);
	}

	public void MoveToPosition(Vector3 destinationPosition)
	{
		PositionSelectorVisuals.transform.position = new Vector3(destinationPosition.x,PositionSelectorVisuals.transform.position.y,destinationPosition.z);
	}

	void GetMovementInput(){
		float verticalAxisInput = Input.GetAxis (Config_CoinTask.VerticalAxisName);
		float horizontalAxisInput = Input.GetAxis (Config_CoinTask.HorizontalAxisName);

		float epsilon = 0.1f;
		//bool positionCloseToTower1 = CheckPositionsClose (epsilon, exp.player.transform.position, exp.player.controls.towerPositionTransform1.position);
		//bool positionCloseToTower2 = CheckPositionsClose (epsilon, exp.player.transform.position, exp.player.controls.towerPositionTransform2.position);

	    Move (verticalAxisInput * selectionMovementSpeed * Time.deltaTime, horizontalAxisInput * selectionMovementSpeed * Time.deltaTime);
		
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

		if (ringChecker.CheckInsideSphere ()) {
			PositionSelector.transform.position += vertAmountVec;
			PositionSelector.transform.position += horizAmountVec;
			if (!ringChecker.CheckInsideSphere ()) {
				PositionSelector.transform.position -= vertAmountVec;
				PositionSelector.transform.position -= horizAmountVec;
			}
		} else {
			PositionSelector.transform.position = exp.environmentController.sphereWalls.ClosestPoint (PositionSelector.transform.position);
		}

	}



	public bool GetRadiusOverlap(Vector3 correctPosition){
		float distance = (correctPosition - PositionSelector.transform.position).magnitude;
		float positionSelectorRadius = PositionSelectorVisuals.transform.localScale.x / 2.0f;
		if (distance < positionSelectorRadius) {
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

	public void EnableFoilVisibility(bool foilCanBeSeen)
	{
		FoilRecallIndicator.GetComponent<VisibilityToggler> ().TurnVisible (foilCanBeSeen);
	}

	public void EnableVisibility(bool shouldBeVisible)
	{
		EnableSelectionIndicator (shouldBeVisible);
	}


	public void EnableObjectSelectorVisuals(bool shouldBeVisible)
	{
		ObjectSelectorVisuals.GetComponent<VisibilityToggler> ().TurnVisible (shouldBeVisible);
	}
	void EnableSelectionIndicator(bool shouldEnable){
		PositionSelectorVisuals.GetComponent<VisibilityToggler> ().TurnVisible (shouldEnable);
	}


	
}
