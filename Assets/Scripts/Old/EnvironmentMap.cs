using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnvironmentMap : MonoBehaviour {

	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public Camera MapCamera;
	public bool IsActive{ get { return MapCamera.enabled; } }


	//oculus map canvas //TODO: make an oculus controller?
	public GameObject OculusMapPanel;
	public Camera OculusMapCamera;

	//map visuals
	public LineRenderer XPosLine;
	public LineRenderer XNegLine;
	public LineRenderer ZPosLine;
	public LineRenderer ZNegLine;

	public GameObject AvatarVisual;
	public GameObject ObjectVisual;

	public GameObject SmallScoreRing;
	public GameObject MediumScoreRing;
	public GameObject BigScoreRing;

	//lock the y position of the object & avatar markers?
	bool shouldLockAvatarVisYPos = true;
	bool shouldLockObjectVisYPos = true;

	//wall points
	float wallPointHeight = 2.0f;
	Vector3 topLeftPoint;
	Vector3 topRightPoint;
	Vector3 bottomLeftPoint;
	Vector3 bottomRightPoint;


	// Use this for initialization
	void Start () {
		CalculatePoints();

		InitLines();

		TurnOff();
	}

	void CalculatePoints(){
		topLeftPoint = new Vector3(exp.environmentController.WallsXNeg.position.x, wallPointHeight, exp.environmentController.WallsZPos.position.z);
		topRightPoint = new Vector3(exp.environmentController.WallsXPos.position.x, wallPointHeight, exp.environmentController.WallsZPos.position.z);
		bottomLeftPoint = new Vector3(exp.environmentController.WallsXNeg.position.x, wallPointHeight, exp.environmentController.WallsZNeg.position.z);
		bottomRightPoint = new Vector3(exp.environmentController.WallsXPos.position.x, wallPointHeight, exp.environmentController.WallsZNeg.position.z);
	}

	void InitLines(){
		XPosLine.SetPosition(0, topRightPoint);
		XPosLine.SetPosition(1, bottomRightPoint);
		
		ZPosLine.SetPosition(0, topLeftPoint);
		ZPosLine.SetPosition(1, topRightPoint);
		
		XNegLine.SetPosition(0, bottomLeftPoint);
		XNegLine.SetPosition(1, topLeftPoint);
		
		ZNegLine.SetPosition(0, bottomRightPoint);
		ZNegLine.SetPosition(1, bottomLeftPoint);
	}

	public void TurnOn(){

		if(ExperimentSettings_CoinTask.isOculus){
			OculusMapPanel.SetActive(true);
			OculusMapCamera.enabled = true;
		}
		else{
			MapCamera.enabled = true;
		}

		/*XPosLine.renderer.enabled = true;
		ZPosLine.renderer.enabled = true;
		XNegLine.renderer.enabled = true;
		ZNegLine.renderer.enabled = true;

		AvatarVisual.SetActive(true);
		ObjectVisual.SetActive(true);*/
	}

	public void TurnOff(){
		MapCamera.enabled = false;

		OculusMapPanel.SetActive(false); //should be off no matter what when TurnOff is called
		OculusMapCamera.enabled = false;

		/*XPosLine.renderer.enabled = false;
		ZPosLine.renderer.enabled = false;
		XNegLine.renderer.enabled = false;
		ZNegLine.renderer.enabled = false;

		AvatarVisual.SetActive(false);
		ObjectVisual.SetActive(false);*/
	}

	public void SetAvatarVisualPosition(Vector3 newPosition){
		if(shouldLockAvatarVisYPos){
			AvatarVisual.transform.position = new Vector3( newPosition.x, AvatarVisual.transform.position.y, newPosition.z);
		}
		else{
			AvatarVisual.transform.position = newPosition;
		}
	}

	public void SetObjectVisualPosition(Vector3 newPosition){
		if(shouldLockObjectVisYPos){
			ObjectVisual.transform.position = new Vector3( newPosition.x, ObjectVisual.transform.position.y, newPosition.z);
		}
		else{
			ObjectVisual.transform.position = newPosition;
		}
	}
}
