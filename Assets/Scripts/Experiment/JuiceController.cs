using UnityEngine;
using System.Collections;

public class JuiceController : MonoBehaviour {

	public delegate void JuiceTogglerDelegate(bool isJuicy);
	public JuiceTogglerDelegate ToggleJuice;

	//ENVIRONMENT
	//skybox
	public Material juicySkybox;
	public Material defaultSkybox;

	//arena & models
	public GameObject juicyEnvironment;
	public GameObject defaultEnvironment;

	//soundtrack
	public SoundtrackController soundtrackController;

	//SOUND
	//fx
	//special object sound
	//treasure chest open sound
	//correct distractor sound
	//box swap sounds
	//answer selector sounds

	//PARTICLES
	//special object particles
	//feedback particles & explosion
	//fireworks and coins in GUI

	//ANIMATIONS
	//treasure chest opening
	//object small to large

	//MAKE SURE THIS HAPPENS BEFORE THE FIRST LOG & OTHER CALCULATIONS.
	//CALLED IN EXPERIMENT --> AWAKE()
	public void Init(){
		ToggleJuice += Toggle;
		ToggleJuice (Config_CoinTask.isJuicy);
	}

	public void Toggle (bool isJuicy){
		if (!isJuicy) {
			SetSkybox (defaultSkybox);
		} else {
			SetSkybox(juicySkybox);
		}
		
		SetEnvironmentJuicy(isJuicy);
		SetSoundtrackJuicy (isJuicy);
	}

	void SetSkybox(Material skyMat){
		RenderSettings.skybox = skyMat;
	}

	void SetEnvironmentJuicy(bool isJuicy){
		juicyEnvironment.SetActive (isJuicy);
		defaultEnvironment.SetActive (!isJuicy);
	}

	void SetSoundtrackJuicy(bool isJuicy){
		soundtrackController.enabled = isJuicy;
	}



}
