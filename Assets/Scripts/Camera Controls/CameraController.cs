using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	//Experiment exp { get { return Experiment.Instance; } }
	//ExperimentSettings expSettings { get { return ExperimentSettings.Instance; } }
	
	public GameObject AvatarStandardCameraRig;
	public GameObject OculusRig;

	public Transform AvatarOculusParent;
	public Transform InstructionsOculusParent;
	

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void SetInstructions(){
		//Debug.Log("hallo instructions cameras");

		TurnOffAllCameras();

		if(ExperimentSettings_CoinTask.isOculus){
			//EnableCameras(OculusRig, true);
			SetOculus(true);
		}
		else{
			EnableCameras(AvatarStandardCameraRig, true);
		}
	}

	public void SetInGame(){

		//Debug.Log("oh hey in game cameras");
		TurnOffAllCameras();

		if(ExperimentSettings_CoinTask.isOculus){
			//OculusRig.transform.position = AvatarOculusParent.transform.position;
			//OculusRig.transform.parent = AvatarOculusParent;
			if(!OculusRig.activeSelf){
				SetOculus(true);
			}
		}
		else{
			EnableCameras(AvatarStandardCameraRig, true);
		}


	}

	void TurnOffAllCameras(){
		EnableCameras(AvatarStandardCameraRig, false);

		if(!ExperimentSettings_CoinTask.isOculus){
			OculusRig.SetActive(false);
		}
	}


	void EnableCameras(GameObject cameraRig, bool setOn){
		Camera[] cameras = cameraRig.GetComponentsInChildren<Camera>();
		for(int i = 0; i < cameras.Length; i++){
			cameras[i].enabled = setOn;
		}
	}

	void SetOculus(bool isActive){
		/*Camera[] cameras = OculusRig.GetComponentsInChildren<Camera>();
		for(int i = 0; i < cameras.Length; i++){
			cameras[i].orthographic = false;
			//cameras[i].clearFlags = CameraClearFlags.Skybox;
		}*/
		OculusRig.SetActive (isActive);
	}
}
