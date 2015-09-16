using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//FOR USE IN TRIALCONTROLLER
public class Trial {
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }


	public bool isStim = false;

	public Vector3 avatarStartPos;
	public Quaternion avatarStartRot;
	public Vector3 avatarTowerPos;
	public Quaternion avatarTowerRot;
	public List<Vector2> DefaultObjectLocationsXZ;
	public List<Vector2> SpecialObjectLocationsXZ;

	public DifficultySetting trialDifficulty;


	public static DifficultySetting practiceDifficulty = DifficultySetting.easy;
	public enum DifficultySetting {
		easy,
		medium,
		hard
	}


	public Trial(){
		DefaultObjectLocationsXZ = new List<Vector2> ();
		SpecialObjectLocationsXZ = new List<Vector2> ();
	}

	public Trial(bool shouldBeStim, DifficultySetting difficulty){
		isStim = shouldBeStim;

		int fiftyFiftyChance = Random.Range (0, 2); //will pick 1 or 0
		if (fiftyFiftyChance == 0) {
			avatarStartPos = exp.player.controls.startPositionTransform1.position;//new Vector3 (exp.player.controls.startPositionTransform1.position.x, exp.player.transform.position.y, exp.player.controls.startPositionTransform1.z);
			avatarStartRot = exp.player.controls.startPositionTransform1.rotation;//Quaternion.Euler (0, exp.player.controls.startPositionTransform1.rotation, 0);
		}
		else {
			avatarStartPos = exp.player.controls.startPositionTransform2.position;
			avatarStartRot = exp.player.controls.startPositionTransform2.rotation;
		}



		fiftyFiftyChance = Random.Range (0, 2); //will pick 1 or 0
		if (fiftyFiftyChance == 0) {
			Debug.Log("FIFTY FIFTY 0");
			avatarTowerPos = exp.player.controls.towerPositionTransform1.position;
			avatarTowerRot = exp.player.controls.towerPositionTransform1.rotation;
		}
		else {
			avatarTowerPos = exp.player.controls.towerPositionTransform2.position;
			avatarTowerRot = exp.player.controls.towerPositionTransform2.rotation;
		}

		int numDefaultObjects = 0;
		int numSpecialObjects = 0;

		if (difficulty == DifficultySetting.easy) {
			//numDefaultObjects = Config_CoinTask.numDefaultObjectsEasy;
			numSpecialObjects = Config_CoinTask.numSpecialObjectsEasy;
		} 
		else if (difficulty == DifficultySetting.medium) {
			//numDefaultObjects = Config_CoinTask.numDefaultObjectsMedium;
			numSpecialObjects = Config_CoinTask.numSpecialObjectsMedium;
		} 
		else if (difficulty == DifficultySetting.hard) {
			//numDefaultObjects = Config_CoinTask.numDefaultObjectsHard;
			numSpecialObjects = Config_CoinTask.numSpecialObjectsHard;
		}
		//TODO: change back to difficulty levels?
		numDefaultObjects = Config_CoinTask.numDefaultObjects;

		//init default and special locations
		DefaultObjectLocationsXZ = exp.objectController.GenerateDefaultObjectPositions (numDefaultObjects);
		SpecialObjectLocationsXZ = exp.objectController.GenerateSpecialObjectPositions (DefaultObjectLocationsXZ, numSpecialObjects);

	}
	
	//get reflected rotation
	public Quaternion GetReflectedRotation(Quaternion rot){
		Vector3 newRot = rot.eulerAngles;
		newRot = new Vector3(newRot.x, newRot.y + 180, newRot.z);
		return Quaternion.Euler(newRot);
	}
	
	//get reflected position from the environment controller!
	//Given the center of the environment, calculate a reflected position in the environment
	public Vector3 GetReflectedPositionXZ(Vector3 pos){
		
		Vector3 envCenter = exp.environmentController.GetEnvironmentCenter ();
		
		Vector3 distanceFromCenter = pos - envCenter;
		
		float reflectedPosX = envCenter.x - distanceFromCenter.x;
		float reflectedPosZ = envCenter.z - distanceFromCenter.z;
		
		Vector3 reflectedPos = new Vector3 ( reflectedPosX, pos.y, reflectedPosZ );
		
		return reflectedPos;
	}
	
	public Trial GetCounterSelf(){
		Trial counterTrial = new Trial ();

		counterTrial.isStim = !isStim;


		//counter the avatar
		if (avatarStartPos == exp.player.controls.startPositionTransform1.position) {
			counterTrial.avatarStartPos = exp.player.controls.startPositionTransform2.position;
			counterTrial.avatarStartRot = exp.player.controls.startPositionTransform2.rotation;
		} 
		else {
			counterTrial.avatarStartPos = exp.player.controls.startPositionTransform1.position;
			counterTrial.avatarStartRot = exp.player.controls.startPositionTransform1.rotation;
		}

		//flip the tower positions
		if (avatarStartPos == exp.player.controls.towerPositionTransform1.position) {
			counterTrial.avatarTowerPos = exp.player.controls.towerPositionTransform2.position;
			counterTrial.avatarTowerRot = exp.player.controls.towerPositionTransform2.rotation;
		}
		else {
			counterTrial.avatarTowerPos = exp.player.controls.towerPositionTransform1.position;
			counterTrial.avatarTowerRot = exp.player.controls.towerPositionTransform1.rotation;
		}


		counterTrial.DefaultObjectLocationsXZ = new List<Vector2> ();
		counterTrial.SpecialObjectLocationsXZ = new List<Vector2> ();
		//counter the object positions
		for (int i = 0; i < DefaultObjectLocationsXZ.Count; i++) {
			Vector3 currPosition = new Vector3( DefaultObjectLocationsXZ[i].x, 0, DefaultObjectLocationsXZ[i].y );
			Vector3 counteredPosition = GetReflectedPositionXZ( currPosition );

			Vector2 counteredPositionXZ = new Vector2(counteredPosition.x, counteredPosition.z);
			counterTrial.DefaultObjectLocationsXZ.Add(counteredPositionXZ);
		}
		
		for (int i = 0; i < SpecialObjectLocationsXZ.Count; i++) {
			Vector3 currPosition = new Vector3( SpecialObjectLocationsXZ[i].x, 0, SpecialObjectLocationsXZ[i].y );
			Vector3 counteredPosition = GetReflectedPositionXZ( currPosition );
			
			Vector2 counteredPositionXZ = new Vector2(counteredPosition.x, counteredPosition.z);
			counterTrial.SpecialObjectLocationsXZ.Add(counteredPositionXZ);
		}
		
		return counterTrial;
	}
}