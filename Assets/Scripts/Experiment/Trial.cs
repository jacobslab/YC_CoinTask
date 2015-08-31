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
	public List<Vector2> DefaultObjectGridIndices;
	public List<Vector2> SpecialObjectIndices;
	

	public Trial(){
		DefaultObjectGridIndices = new List<Vector2> ();
		SpecialObjectIndices = new List<Vector2> ();
	}

	public Trial(bool shouldBeStim){
		isStim = shouldBeStim;

		avatarStartPos = new Vector3 (Config_CoinTask.AvatarStartX, exp.avatar.transform.position.y, Config_CoinTask.AvatarStartZ);
		avatarStartRot = Quaternion.Euler (0, Config_CoinTask.AvatarStartRotY, 0);

		int numDefaultObjects = 0;
		int numSpecialObjects = 0;

		if (Config_CoinTask.difficultySetting == Config_CoinTask.DifficultySetting.easy) {
			numDefaultObjects = Config_CoinTask.numDefaultObjectsEasy;
			numSpecialObjects = Config_CoinTask.numSpecialObjectsEasy;
		} 
		else if (Config_CoinTask.difficultySetting == Config_CoinTask.DifficultySetting.medium) {
			numDefaultObjects = Config_CoinTask.numDefaultObjectsMedium;
			numSpecialObjects = Config_CoinTask.numSpecialObjectsMedium;
		} 
		else if (Config_CoinTask.difficultySetting == Config_CoinTask.DifficultySetting.hard) {
			numDefaultObjects = Config_CoinTask.numDefaultObjectsHard;
			numSpecialObjects = Config_CoinTask.numSpecialObjectsHard;
		}

		//init default and special locations
		DefaultObjectGridIndices = new List<Vector2> ();
		SpecialObjectIndices = new List<Vector2> ();

		DefaultObjectGridIndices = envGrid.GenerateDefaultObjectConfiguraiton (numDefaultObjects);

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


		//counter the avatar
		counterTrial.avatarStartPos = GetReflectedPositionXZ (avatarStartPos);
		counterTrial.avatarStartRot = GetReflectedRotation (avatarStartRot);

		//counter the object positions
		for (int i = 0; i < DefaultObjectGridIndices.Count; i++) {
			Vector2 swappedIndices = DefaultObjectGridIndices[i];
			//swap the indices
			float tempIndex = swappedIndices.x; //(all of these are ints though, because they're indices...)
			swappedIndices.x = swappedIndices.y;
			swappedIndices.y = tempIndex;

			counterTrial.DefaultObjectGridIndices.Add ( swappedIndices );
		}

		for (int i = 0; i < SpecialObjectIndices.Count; i++) {
			Vector2 swappedIndices = SpecialObjectIndices[i];
			//swap the indices
			float tempIndex = swappedIndices.x; //(all of these are ints though, because they're indices...)
			swappedIndices.x = swappedIndices.y;
			swappedIndices.y = tempIndex;
			
			counterTrial.SpecialObjectIndices.Add ( swappedIndices );
		}
		
		return counterTrial;
	}
}