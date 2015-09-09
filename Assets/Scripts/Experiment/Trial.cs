using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//FOR USE IN TRIALCONTROLLER
public class Trial {
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }
	EnvironmentGrid envGrid { get { return Experiment_CoinTask.Instance.environmentController.myGrid; } }


	public bool isStim = false;

	public Vector3 avatarStartPos;
	public Quaternion avatarStartRot;
	public Vector3 avatarTowerPos;
	public Quaternion avatarTowerRot;
	public List<Vector2> DefaultObjectGridIndices;
	public List<Vector2> SpecialObjectIndices;

	public DifficultySetting trialDifficulty;


	public static DifficultySetting practiceDifficulty = DifficultySetting.easy;
	public enum DifficultySetting {
		easy,
		medium,
		hard
	}


	public Trial(){
		DefaultObjectGridIndices = new List<Vector2> ();
		SpecialObjectIndices = new List<Vector2> ();
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
		DefaultObjectGridIndices = new List<Vector2> ();
		SpecialObjectIndices = new List<Vector2> ();

		DefaultObjectGridIndices = envGrid.GenerateDefaultObjectConfiguration (numDefaultObjects);

		SpecialObjectIndices = envGrid.GenerateSpecialObjectConfiguration (DefaultObjectGridIndices, numSpecialObjects);


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


		//TODO: counter the avatar?
		//counterTrial.avatarStartPos = GetReflectedPositionXZ (avatarStartPos);
		//counterTrial.avatarStartRot = GetReflectedRotation (avatarStartRot);
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


		int maxRow = exp.environmentController.myGrid.Rows;
		int maxCol = exp.environmentController.myGrid.Columns;
		//counter the object positions
		for (int i = 0; i < DefaultObjectGridIndices.Count; i++) {
			Vector2 counteredIndices = new Vector3(DefaultObjectGridIndices[i].x, DefaultObjectGridIndices[i].y);
			//counter the indices
			counteredIndices.x = maxRow - counteredIndices.x - 1; // - 1 because indexing starts at zero
			counteredIndices.y = maxCol - counteredIndices.y - 1; // - 1 because indexing starts at zero

			counterTrial.DefaultObjectGridIndices.Add ( counteredIndices );
		}

		for (int i = 0; i < SpecialObjectIndices.Count; i++) {
			Vector2 counteredIndices = new Vector3(SpecialObjectIndices[i].x, SpecialObjectIndices[i].y);
			//counter the indices
			counteredIndices.x = maxRow - counteredIndices.x - 1; // - 1 because indexing starts at zero
			counteredIndices.y = maxCol - counteredIndices.y - 1; // - 1 because indexing starts at zero
			
			counterTrial.SpecialObjectIndices.Add ( counteredIndices );
		}
		
		return counterTrial;
	}
}