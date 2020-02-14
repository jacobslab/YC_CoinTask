using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnvironmentPositionSelector : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public GameObject PositionSelector;
	public GameObject PositionSelectorVisuals;
	public GameObject CorrectPositionIndicator;

	public GameObject raycastCube;
	public GameObject ground;
	public LayerMask layerMask;
	public GameObject markerSphere;

	public PositionSelectorLogTrack logTrack;


	public Color VisualsDefaultColor;
	public Color VisualsSelectColor;

	float currDelayTime = 0f;
	float delayTime = 0.3f;

	float prevPositionVert = 0f;
	float prevPositionHorz = 0f;
	bool shouldSelect;
	float selectionMovementSpeed = 250.0f;

	void Awake(){
		PositionSelectorVisuals.transform.localScale = new Vector3 (Config_CoinTask.selectionDiameter, PositionSelectorVisuals.transform.localScale.y, Config_CoinTask.selectionDiameter);
	}

	// Use this for initialization
	void Start () {
		EnableSelection(false);
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

	void GetMovementInput(){
		
			//float verticalAxisInput = Input.GetAxis (Config_CoinTask.VerticalAxisName);
			//	float horizontalAxisInput = Input.GetAxis (Config_CoinTask.HorizontalAxisName);
			float verticalAxisInput = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch).y - prevPositionVert;
		float horizontalAxisInput = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch).x - prevPositionHorz;
		/*
		if (currDelayTime < delayTime)
		{
			currDelayTime += Time.deltaTime;
		}
		else
		{
			currDelayTime = 0.0f;
			prevPositionVert = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch).y;
			prevPositionHorz = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch).x;
			
		}
		*/
		float epsilon = 0.1f;
		bool positionCloseToTower1 = CheckPositionsClose (epsilon, exp.player.transform.position, exp.player.controls.towerPositionTransform1.position);
		bool positionCloseToTower2 = CheckPositionsClose (epsilon, exp.player.transform.position, exp.player.controls.towerPositionTransform2.position);

		if (positionCloseToTower1) {
			Move (verticalAxisInput * selectionMovementSpeed * Time.deltaTime, horizontalAxisInput * selectionMovementSpeed * Time.deltaTime);
		} 
		else if (positionCloseToTower2) {
			Move (-verticalAxisInput * selectionMovementSpeed * Time.deltaTime, -horizontalAxisInput * selectionMovementSpeed * Time.deltaTime);
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

		Ray ray = new Ray(raycastCube.transform.position,
						raycastCube.transform.forward);

		RaycastHit rayHitInfo = new RaycastHit();
		Vector3 pos = Vector3.zero;
		if (Physics.Raycast(ray, out rayHitInfo, 1000f, layerMask.value))
		{
			pos = new Vector3(rayHitInfo.point.x, rayHitInfo.point.y,rayHitInfo.point.z);
		}
		bool wouldBeInWallsVert = exp.environmentController.CheckWithinWallsVert(pos, Config_CoinTask.objectToWallBuffer);
		bool wouldBeInWallsHoriz = exp.environmentController.CheckWithinWallsHoriz (pos, Config_CoinTask.objectToWallBuffer); 
		Debug.Log("pos is " + pos.ToString());
		markerSphere.transform.position = pos;

		//bool wouldBeInWallsVert = exp.environmentController.CheckWithinWallsVert (PositionSelector.transform.position + (vertAmountVec), Config_CoinTask.objectToWallBuffer);
		//bool wouldBeInWallsHoriz = exp.environmentController.CheckWithinWallsHoriz (PositionSelector.transform.position + (horizAmountVec), Config_CoinTask.objectToWallBuffer); 


		if (wouldBeInWallsVert) {
			PositionSelector.transform.position = new Vector3(PositionSelector.transform.position.x,PositionSelector.transform.position.y,pos.z);
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
			PositionSelector.transform.position= new Vector3(pos.x, PositionSelector.transform.position.y, PositionSelector.transform.position.z);
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
		if(shouldEnable)
		{
			prevPositionHorz = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch).x;
			prevPositionVert = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch).y;
			Debug.Log("reset HORZ and VERT position");
		}
		shouldSelect = shouldEnable;
		EnableSelectionIndicator (shouldEnable);
	}

	void EnableSelectionIndicator(bool shouldEnable){
		PositionSelectorVisuals.GetComponent<VisibilityToggler> ().TurnVisible (shouldEnable);
	}
	
}
