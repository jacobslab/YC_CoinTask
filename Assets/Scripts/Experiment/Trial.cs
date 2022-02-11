using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//FOR USE IN TRIALCONTROLLER
public class Trial {
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public bool isStim;

	public Vector3 avatarStartPos;
	public Quaternion avatarStartRot;
	public Vector3 avatarTowerPos;
	public Quaternion avatarTowerRot;
	public int numSpecialObjects;
	public List<Vector2> DefaultObjectLocationsXZ;
	public List<Vector2> SpecialObjectLocationsXZ;

    public Vector2 FlagLocationXZ;

	public ExperimentSettings_CoinTask.RecallType trialRecallType;
	private static int fourNum = 0;
	private static int fiveNum=0;
	private static int totalTrials=0;
	public List<string> ManualSpecialObjectNames { get { return manualSpecialObjectNames; } }
	List<string> manualSpecialObjectNames;
	private static int totalTHR = 0;
	private static int totalTH = 0;

	public Trial(){
		DefaultObjectLocationsXZ = new List<Vector2> ();
		SpecialObjectLocationsXZ = new List<Vector2> ();
	}

	public Trial(int numSpecial,int numFoils, bool shouldStim, bool isRightAngle){

		numSpecialObjects = numSpecial;

		manualSpecialObjectNames = new List<string> (); //may or may not be filled in manually.

        //Debug.Log("NUM SPECIAL: " + numSpecialObjects);


        //have a fixed start and tower position for all trials
        avatarStartPos = exp.player.controls.startPositionTransform1.position;
        avatarStartRot = exp.player.controls.startPositionTransform1.rotation;

        avatarTowerPos = exp.player.controls.towerPositionTransform1.position;
        avatarTowerRot = exp.player.controls.towerPositionTransform1.rotation;
        //avatarStartPos = exp.environmentController.GetRandomStartPosition (Config_CoinTask.radiusBuffer);
        //avatarStartPos = new Vector3 (avatarStartPos.x, exp.player.controls.startPositionTransform1.position.y, avatarStartPos.z);


        //we're still using the same old two random rotation for starting perspectives
//        int fiftyFiftyChance = Random.Range (0, 2); //will pick 1 or 0
//		if (fiftyFiftyChance == 0) {
////			trialRecallType = ExperimentSettings_CoinTask.RecallType.Location;
////			totalTH++;
//			avatarStartRot = exp.player.controls.startPositionTransform1.rotation;//Quaternion.Euler (0, exp.player.controls.startPositionTransform1.rotation, 0);
//		}
//		else {
////			trialRecallType = ExperimentSettings_CoinTask.RecallType.Object;
		////	totalTHR++;
		//	avatarStartRot = exp.player.controls.startPositionTransform2.rotation;
		//}



		//fiftyFiftyChance = Random.Range (0, 2); //will pick 1 or 0
		//if (fiftyFiftyChance == 0) {
		//	avatarTowerPos = exp.player.controls.towerPositionTransform1.position;
		//	avatarTowerRot = exp.player.controls.towerPositionTransform1.rotation;
		//}
		//else {
		//	avatarTowerPos = exp.player.controls.towerPositionTransform2.position;
		//	avatarTowerRot = exp.player.controls.towerPositionTransform2.rotation;
		//}


		int numDefaultObjects = 0;
		numDefaultObjects = numSpecial + numFoils;
//		if (trialRecallType == ExperimentSettings_CoinTask.RecallType.Object)
//			numDefaultObjects = numSpecial + numFoils;
//		else
//			numDefaultObjects = Config_CoinTask.numDefaultObjects;

		totalTrials++;
		/*
		switch (numDefaultObjects) {
		case 4:
			fourNum++;
			break;
		case 5:
			fiveNum++;
			break;
		}

		Debug.Log ("Four: " + fourNum);
		Debug.Log ("Five: " + fiveNum);
		*/
		Debug.Log ("total: " + totalTrials);

        //Debug.Log ("number of special objects is: " + numSpecial);
        //Debug.Log ("number of foil objects is: " + numFoils);

        //init default and special locations
        Vector2 startPosition = Vector2.zero;
        DefaultObjectLocationsXZ = exp.objectController.GenerateTriangularDefaultObjectPositions(numDefaultObjects, avatarStartPos, isRightAngle, out startPosition);
        //DefaultObjectLocationsXZ = exp.objectController.GenerateOrderedDefaultObjectPositions (numDefaultObjects, avatarStartPos);
        //FlagLocationXZ = exp.objectController.GenerateFlagPosition(DefaultObjectLocationsXZ,avatarStartPos);
        FlagLocationXZ = startPosition;
        //FoilObjectLocationsXZ = exp.objectController.GenerateFoilPositions (numFoils, DefaultObjectLocationsXZ, avatarStartPos);
        //Debug.Log ("number of default positions is: " + DefaultObjectLocationsXZ.Count+"/"+numDefaultObjects);
        //Debug.Log ("number of foil positions is: " + FoilObjectLocationsXZ.Count);
        SpecialObjectLocationsXZ = exp.objectController.GenerateSpecialObjectPositions (DefaultObjectLocationsXZ, numSpecialObjects);
	}

	public void AddToManualSpecialObjectNames(string specialObjectName){
		manualSpecialObjectNames.Add (specialObjectName);
	}

	public void ClearSpecialObjectNames(){
		manualSpecialObjectNames.Clear ();
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
	
	public Trial GetCounterSelf(bool shouldStim){
		Trial counterTrial = new Trial ();

		counterTrial.isStim = shouldStim;

		counterTrial.numSpecialObjects = numSpecialObjects;

		//counter the avatar

		Vector3 newAvatarStartPos = GetReflectedPositionXZ (avatarStartPos);
		counterTrial.avatarStartPos = newAvatarStartPos;

		//we're still using the same old two random rotation for starting perspectives
			counterTrial.avatarStartRot = exp.player.controls.startPositionTransform1.rotation;
		
		//flip the tower positions
			counterTrial.avatarTowerPos = exp.player.controls.towerPositionTransform1.position;
			counterTrial.avatarTowerRot = exp.player.controls.towerPositionTransform1.rotation;
	


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

		//int numDefaultObjects = counterTrial.DefaultObjectLocationsXZ.Count;
		totalTrials++;
		/*
		switch (numDefaultObjects) {
		case 4:
			fourNum++;
			break;
		case 5:
			fiveNum++;
			break;
		}

		Debug.Log ("Four: " + fourNum);
		Debug.Log ("Five: " + fiveNum);
		*/
		Debug.Log ("total: " + totalTrials);
//		Debug.Log("th1: " + totalTH.ToString() + "thr: " +totalTHR.ToString());

		return counterTrial;
	}
}