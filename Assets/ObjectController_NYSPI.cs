using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
public class ObjectController_NYSPI : MonoBehaviour {


	//instantiated somewhere (hidden) in the scene at all times. for calculating default object bounds --> requires an active object, and we don't want to instantiate one every time we ask for the bounds.
	//this object does *not* need to be logged.
	//it should also have a special name in the scene.
	public GameObject InGameDefaultObject;

	public GameObject DefaultObject; //the prefab used to instantiate the other default objects.
	public GameObject ExplosiveObject;
	public List<GameObject> CurrentTrialSpecialObjects;
	List<GameObject>tempList;
	public List<GameObject> RecallObjectList;
	public List<GameObject> FoilObjects;
	public int CurrentTrialFoilObjects=0;
	public GameObject defaultFoilObject;
	//experiment singleton
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	//object array & list
	List<GameObject> gameObjectList_Spawnable_SetA;
	string[] setAList;
	string[] setBList;
	string[] setCList;
	List<GameObject> gameObjectList_Spawnable_SetB;
	List<GameObject> gameObjectList_Spawnable_SetC;
	//List<GameObject> gameObjectList_Spawned; //a list to keep track of the objects currently in the scene


	//to keep track of last default object so that helper arrows don't point to foil objects
	private GameObject previousDefaultObj;
    private int totalSetObjects=49;

	// Use this for initialization
	void Start () {
        //set object total per set
        totalSetObjects = Config_CoinTask.totalObjectsPerSet;
        gameObjectList_Spawnable_SetA = new List<GameObject> ();
		gameObjectList_Spawnable_SetB = new List<GameObject> ();
		gameObjectList_Spawnable_SetC = new List<GameObject> ();
		tempList=new List<GameObject>();
		CurrentTrialSpecialObjects = new List<GameObject> ();
		if ((!File.Exists (exp.subjectDirectory + "SetA\\SetAList.txt")))
			CreateSpecialObjectList ();
//		else
		ReadObjectLists ();
        
		setAList = new string[totalSetObjects];
		setBList = new string[totalSetObjects];
		setCList = new string[totalSetObjects];
	}

	public void CreateFoilObjects()
	{
		for (int i = 0; i < 2; i++) {
			GameObject temp = Instantiate (defaultFoilObject, Vector3.zero, Quaternion.identity) as GameObject;
			temp.GetComponent<SpawnableObject> ().TurnVisible (false);
			FoilObjects.Add (temp);
		}
	}
	// Update is called once per frame
	void Update () {
	
	}

	void ReadObjectLists()
	{

		gameObjectList_Spawnable_SetA.Clear();
		gameObjectList_Spawnable_SetB.Clear();
		gameObjectList_Spawnable_SetC.Clear();
		Object[] prefabs;
		prefabs = Resources.LoadAll("Prefabs/Objects_NYSPI");
        Debug.Log("all the loadable prefab count is: " + prefabs.Length);
		for(int i=0;i<prefabs.Length;i++)
			tempList.Add((GameObject)prefabs[i]);


		string[] listObj = new string[totalSetObjects];
		switch (Config_CoinTask.currentSetNumber) {
		case Config_CoinTask.SetNumber.A:
			listObj=System.IO.File.ReadAllLines (exp.subjectDirectory + "SetA\\SetAList.txt");
			break;
		case Config_CoinTask.SetNumber.B:
			listObj=System.IO.File.ReadAllLines (Application.dataPath + "SetB\\SetBList.txt");
			break;
		case Config_CoinTask.SetNumber.C:
			listObj=System.IO.File.ReadAllLines (exp.subjectDirectory+"SetA\\SetCList.txt");
			break;
		}

		List<GameObject> tempObjList = new List<GameObject> ();
		List<GameObject> spawnList = new List<GameObject> ();

        Debug.Log("list obj count is: " + listObj.Length);
        int index = 0;
		//find object in a list
		for (int i = 0; i < listObj.Length; i++) {
			//if object name is equal to name in list, then add that to the tempObjList
			for (int j = 0; j < tempList.Count; j++) {

				if ((tempList[j].gameObject.name.ToLower()== listObj [i].ToLower()) && (tempList[j].gameObject.name.Length == listObj[i].Length)) {
                    index++;
              //      Debug.Log(index + ":" + listObj[i]);
					tempObjList.Add ((GameObject)tempList [j]);
				}
			}
		}
        Debug.Log("the index is: " + index);

		Debug.Log ("temp obj  list count is: " + tempObjList.Count);

		//randomly select gameobjects from tempObjList and add to spawnList
		string contents="";
		for (int j = 0; j <totalSetObjects; j++) {
			
			GameObject tempObj = (GameObject) tempObjList[(totalSetObjects-1)- j];
			spawnList.Add (tempObj);
		//	Debug.Log (tempObj.name);
			contents += tempObj.name + "\n";
			tempObjList.Remove (tempObj);
		}

		//set spawnList to the current set's spawn list
		switch (Config_CoinTask.currentSetNumber) {
		case Config_CoinTask.SetNumber.A:
			gameObjectList_Spawnable_SetA = spawnList;
			System.IO.File.WriteAllText (exp.subjectDirectory + "SetA\\" + "ActualListOrder.txt",contents);
			break;
		case Config_CoinTask.SetNumber.B:
			gameObjectList_Spawnable_SetB = spawnList;
			System.IO.File.WriteAllText (exp.subjectDirectory + "SetB\\" + "ActualListOrder.txt",contents);
			break;
		case Config_CoinTask.SetNumber.C:
			gameObjectList_Spawnable_SetC = spawnList;
			System.IO.File.WriteAllText (exp.subjectDirectory + "SetC\\" + "ActualListOrder.txt",contents);
			break;
		}
	}

	void CreateSpecialObjectList(){
		gameObjectList_Spawnable_SetA.Clear();
		gameObjectList_Spawnable_SetB.Clear();
		gameObjectList_Spawnable_SetC.Clear();
		Object[] prefabs;
		List<string> firstList = new List<string> ();
		List<string> secondList = new List<string> ();
		List<string> thirdList = new List<string> ();
		string contents = "";
		#if MRIVERSION
		if(Config_CoinTask.isPractice){
			prefabs = Resources.LoadAll("Prefabs/MRIPracticeObjects");
		}
		else{
			prefabs = Resources.LoadAll("Prefabs/Objects");
		}
		#else
			prefabs = Resources.LoadAll("Prefabs/Objects_NYSPI");
		#endif

		for(int i=0;i<prefabs.Length;i++)
			tempList.Add((GameObject)prefabs[i]);
		while(gameObjectList_Spawnable_SetA.Count < totalSetObjects)
		{
			GameObject tempObj = (GameObject) tempList[Random.Range(0,tempList.Count)];
			if(!gameObjectList_Spawnable_SetA.Contains(tempObj))
			{
				gameObjectList_Spawnable_SetA.Add(tempObj);
				contents += tempObj.name + "\n";
				string tempString = tempObj.ToString ();
				int startIndex = tempString.IndexOf ("(");
				int endIndex = tempString.IndexOf (")");
				tempString=tempString.Remove (startIndex-1, endIndex - startIndex + 2);
				firstList.Add(tempString);
				tempList.Remove(tempObj);
			}
		}
		while (gameObjectList_Spawnable_SetB.Count < totalSetObjects) {
			for (int j = 0; j < totalSetObjects; j++) {
				GameObject tempObj = (GameObject)tempList [Random.Range(0,tempList.Count)];
				gameObjectList_Spawnable_SetB.Add (tempObj);
				string tempString = tempObj.ToString ();
				int startIndex = tempString.IndexOf ("(");
				int endIndex = tempString.IndexOf (")");
				tempString = tempString.Remove (startIndex - 1, endIndex - startIndex + 2);
				secondList.Add (tempString.ToString ());
				tempList.Remove (tempObj);
			}
		}
		for (int k = 0; k < tempList.Count; k++) {
			gameObjectList_Spawnable_SetC.Add ((GameObject)tempList [k]);
			string tempString = tempList [k].ToString ();
			int startIndex = tempString.IndexOf ("(");
			int endIndex = tempString.IndexOf (")");
			tempString = tempString.Remove (startIndex - 1, endIndex - startIndex + 2);
			thirdList.Add (tempString.ToString ());
		}

		for (int i = 0; i < firstList.Count; i++)
			setAList=firstList.ToArray();

		for (int i = 0; i < secondList.Count; i++)
			setBList = secondList.ToArray();


		for (int i = 0; i < thirdList.Count; i++)
			setCList = thirdList.ToArray();
		
		PrintObjects ();
		Debug.Log ("the number of Set A objects are : " + gameObjectList_Spawnable_SetA.Count);
		Debug.Log ("the number of Set B objects are : " + gameObjectList_Spawnable_SetB.Count);
		Debug.Log ("the number of Set C objects are : " + gameObjectList_Spawnable_SetC.Count);
		System.IO.File.WriteAllText (exp.subjectDirectory + "SetA\\" + "ActualListOrder.txt", contents);
		}

	void PrintObjects()
	{
		if (!File.Exists (exp.subjectDirectory + "SetA\\SetAList.txt")) {
			Debug.Log ("set A directory doesn't exist. creating");
			System.IO.File.WriteAllLines (exp.subjectDirectory+"SetA\\" + "SetAList.txt", setAList);
		}
		if (!File.Exists (exp.subjectDirectory + "SetB\\SetBList.txt")) {
			Debug.Log ("set B directory doesn't exist. creating");
			System.IO.File.WriteAllLines (exp.subjectDirectory+"SetB\\" + "SetBList.txt", setBList);
		}
		if (!File.Exists (exp.subjectDirectory + "SetC\\SetCList.txt")) {
			Debug.Log ("set C directory doesn't exist. creating");
			System.IO.File.WriteAllLines (exp.subjectDirectory+"SetC\\" + "SetCList.txt", setCList);
		}
	}

	//used in replay
	void CreateCompleteSpawnableList (List<SpawnableObject> spawnableListToFill){
		spawnableListToFill.Clear();
		Object[] specialPrefabs = Resources.LoadAll("Prefabs/Objects");
		Object[] otherPrefabs = Resources.LoadAll("Prefabs/OtherSpawnables");
		for (int i = 0; i < specialPrefabs.Length; i++) {
			SpawnableObject spawnable = ( (GameObject)specialPrefabs[i] ).GetComponent<SpawnableObject>();
			spawnableListToFill.Add(spawnable);
		}
		for (int i = 0; i < otherPrefabs.Length; i++) {
			SpawnableObject spawnable = ( (GameObject)otherPrefabs[i] ).GetComponent<SpawnableObject>();
			spawnableListToFill.Add(spawnable);
		}
	}

	//used in replay
	public GameObject ChooseSpawnableObject(string objectName){
		List<SpawnableObject> allSpawnables = new List<SpawnableObject>(); //note: this is technically getting instantiated twice now... as it's instantiated in CREATE as well.
		CreateCompleteSpawnableList (allSpawnables);

		for (int i = 0; i < allSpawnables.Count; i++) {
			if(allSpawnables[i].GetName() == objectName){
				return allSpawnables[i].gameObject;
			}
		}
		return null;
	}

	public IEnumerator ThrowExplosive(Vector3 from, Vector3 to, int explosiveID){
		GameObject newExplosive = Instantiate (ExplosiveObject, from, ExplosiveObject.transform.rotation) as GameObject;
		SpawnableObject newSpawnable = newExplosive.GetComponent<SpawnableObject> ();
		if (newSpawnable) {
			newSpawnable.SetNameID(newExplosive.transform, explosiveID);
		}
		yield return StartCoroutine( newExplosive.GetComponent<Explosive>().ThrowSelf(from, to) );
	}


	GameObject ChooseRandomObject(){
		GameObject chosenObject;
		//MAKE NYSPI related changes here
		if (Config_CoinTask.currentSetNumber == Config_CoinTask.SetNumber.A) {
			if (gameObjectList_Spawnable_SetA.Count == 0) {
				Debug.Log ("No MORE objects to pick! Recreating object list.");
				//CreateSpecialObjectList (); //IN ORDER TO REFILL THE LIST ONCE ALL OBJECTS HAVE BEEN USED
				if (gameObjectList_Spawnable_SetA.Count == 0) {
					Debug.Log ("No objects to pick at all!"); //if there are still no objects in the list, then there weren't any to begin with...
					return null;
				}
			}


			int randomObjectIndex =0; //setting to 0 because we want the randomization to be the same as the one printed in actualListOrder.txt
            chosenObject = gameObjectList_Spawnable_SetA [randomObjectIndex];
			gameObjectList_Spawnable_SetA.RemoveAt (randomObjectIndex);
		} else if (Config_CoinTask.currentSetNumber == Config_CoinTask.SetNumber.B) {
			if (gameObjectList_Spawnable_SetB.Count == 0) {
				Debug.Log ("No MORE objects to pick! Recreating object list.");
				//CreateSpecialObjectList (); //IN ORDER TO REFILL THE LIST ONCE ALL OBJECTS HAVE BEEN USED
				if (gameObjectList_Spawnable_SetB.Count == 0) {
					Debug.Log ("No objects to pick at all!"); //if there are still no objects in the list, then there weren't any to begin with...
					return null;
				}
			}


            int randomObjectIndex = 0; //setting to 0 because we want the randomization to be the same as the one printed in actualListOrder.txt
			//int randomObjectIndex = Random.Range (0, gameObjectList_Spawnable_SetB.Count);
			chosenObject = gameObjectList_Spawnable_SetB [randomObjectIndex];
			gameObjectList_Spawnable_SetB.RemoveAt (randomObjectIndex);
		} else if (Config_CoinTask.currentSetNumber == Config_CoinTask.SetNumber.C) {
			if (gameObjectList_Spawnable_SetC.Count == 0) {
				Debug.Log ("No MORE objects to pick! Recreating object list.");
				//CreateSpecialObjectList (); //IN ORDER TO REFILL THE LIST ONCE ALL OBJECTS HAVE BEEN USED
				if (gameObjectList_Spawnable_SetC.Count == 0) {
					Debug.Log ("No objects to pick at all!"); //if there are still no objects in the list, then there weren't any to begin with...
					return null;
				}
			}

            int randomObjectIndex = 0;//setting to 0 because we want the randomization to be the same as the one printed in actualListOrder.txt
			//int randomObjectIndex = Random.Range (0, gameObjectList_Spawnable_SetC.Count);
			chosenObject = gameObjectList_Spawnable_SetC [randomObjectIndex];
			gameObjectList_Spawnable_SetC.RemoveAt (randomObjectIndex);
		} else {
			chosenObject = null;
		}
		return chosenObject;
	}


	public float GenerateRandomRotationY(){
		float randomRotY = Random.Range (0, 360);
		return randomRotY;
	}


	//special positions get passed in so that the default object can get a special tag for later use for spawning special objects
	public void SpawnDefaultObjects(List<Vector2> defaultPositions, List<Vector2> specialPositions){
		Debug.Log ("IN SPAWN DEFAULT OBJECTS");
		for(int i = 0; i < defaultPositions.Count; i++){
			Vector2 currPos = defaultPositions[i];
			SpawnDefaultObject( currPos, specialPositions, i );
		}
	}

	//special positions get passed in so that the default object can get a special tag for later use for spawning special objects
	public GameObject SpawnDefaultObject (Vector2 positionXZ, List<Vector2> specialPositions, int index) {

		GameObject newObj;
		Vector3 objPos;
		if (specialPositions.Contains (positionXZ)) {
			objPos= new Vector3(positionXZ.x, DefaultObject.transform.position.y, positionXZ.y);
			newObj= Instantiate(DefaultObject, objPos, DefaultObject.transform.rotation) as GameObject;
			SpawnableObject newSpawnableObj = newObj.GetComponent<SpawnableObject>();
			newSpawnableObj.SetNameID(newObj.transform, index);
			newObj.tag = "DefaultSpecialObject";
			previousDefaultObj = newObj;
		} 
		else 
		{
			Debug.Log ("current foil objects index: " + CurrentTrialFoilObjects);
			newObj = FoilObjects [CurrentTrialFoilObjects++];
			objPos= new Vector3(positionXZ.x, DefaultObject.transform.position.y, positionXZ.y);
			newObj.transform.position = objPos;
			newObj.tag = "FoilObject";
			newObj.GetComponent<SpawnableObject> ().TurnVisible (false);
			exp.trialController.IncrementNumDefaultObjectsCollected ();
			newObj = previousDefaultObj;
		}

		return newObj;
	}

	
	
	//for more generic object spawning -- such as in Replay!
	public GameObject SpawnObject( GameObject objToSpawn, Vector3 spawnPos ){
		GameObject spawnedObj = Instantiate(objToSpawn, spawnPos, objToSpawn.transform.rotation) as GameObject;

		return spawnedObj;
	}

	//spawn random object at a specified location
	public GameObject SpawnSpecialObject (Vector3 spawnPos){
		GameObject objToSpawn;
		if (exp.trialController.currentTrial.ManualSpecialObjectNames != null) {
			if (exp.trialController.currentTrial.ManualSpecialObjectNames.Count > 0) {
				objToSpawn = ChooseSpawnableObject (exp.trialController.currentTrial.ManualSpecialObjectNames [0]);
				exp.trialController.currentTrial.ManualSpecialObjectNames.RemoveAt (0);
			} else {
				objToSpawn = ChooseRandomObject ();
			}
		}
		else{
			objToSpawn = ChooseRandomObject ();
		}

		if (objToSpawn != null) {

			GameObject newObject = Instantiate(objToSpawn, spawnPos, objToSpawn.transform.rotation) as GameObject;

			float randomRot = GenerateRandomRotationY();
			newObject.transform.RotateAround(newObject.transform.position, Vector3.up, randomRot);

			Debug.Log ("ADDED A SPECIAL OBJECT");
			CurrentTrialSpecialObjects.Add(newObject);

			//make object face the player -- MUST MAKE SURE OBJECT FACES Z-AXIS
			//don't want object to tilt downward at the player -- use object's current y position
			UsefulFunctions.FaceObject( newObject, exp.player.gameObject, false);

			return newObject;
		}
		else{
			return null;
		}
	}

	public List<Vector2> GenerateOrderedDefaultObjectPositions (int numDefaultObjects, Vector3 distancePos){ //ORDERED BY DISTANCE TO PLAYER START POS
		List<Vector2> defaultPositions = new List<Vector2> ();

		for (int i = 0; i < numDefaultObjects; i++) {
			bool objectsAreFarEnough = false;

			int numTries = 0;
			int maxNumTries = 1000; //ARBITRARY.

			Vector3 randomEnvPosition = Vector3.zero;
			Vector2 randomEnvPositionVec2 = Vector2.zero;

			float smallestDistance = 0.0f; //will get filled in by CheckObjectsFarEnoughXZ function
			float currentBiggestSmallestDistance = 0; //if we fail at positioning in the allotted number of tries, we want to position the treasure chest with the maximal distance to the closest neighbor chest.
			Vector2 smallestDistancePosition = Vector2.zero;

			while( !objectsAreFarEnough && numTries < maxNumTries){
				// generate a random position
				randomEnvPosition = exp.environmentController.GetRandomPositionWithinWallsXZ( Config_CoinTask.objectToWallBuffer );
				randomEnvPositionVec2 = new Vector2(randomEnvPosition.x, randomEnvPosition.z);

				//increment numTries
				numTries++;

				//check if the generated position is far enough from all other positions
				objectsAreFarEnough = CheckObjectsFarEnoughXZ( randomEnvPositionVec2, defaultPositions, out smallestDistance);

				//if not, and the smallest distance is larger than the currents largest small distance...
				if( !objectsAreFarEnough && smallestDistance > currentBiggestSmallestDistance ){
					currentBiggestSmallestDistance = smallestDistance;
					smallestDistancePosition = randomEnvPositionVec2;
				}
			}

			if(numTries == maxNumTries){
				//				Debug.Log("Tried " + maxNumTries + " times to place default objects!");
				//				Debug.Log("DISTANCE: " + currentBiggestSmallestDistance + " POSITION: " + smallestDistancePosition);
				defaultPositions.Add(smallestDistancePosition);
			}
			else{
				defaultPositions.Add(randomEnvPositionVec2);
			}

		}

		//		//insertion sort by distance
		//		Vector2 distancePosXZ = new Vector2 (distancePos.x, distancePos.z);
		//		defaultPositions = SortByNextClosest(defaultPositions, distancePosXZ);

		return defaultPositions;
	}


	public List<Vector2> GenerateFoilPositions (int numFoils, List<Vector2> existingPositions, Vector3 distancePos){ //ORDERED BY DISTANCE TO PLAYER START POS
		List<Vector2> foilPositions = new List<Vector2> ();
		List<Vector2> defaultPositions = existingPositions;
		for (int i = 0; i < numFoils; i++) {
			bool objectsAreFarEnough = false;

			int numTries = 0;
			int maxNumTries = 1000; //ARBITRARY.

			Vector3 randomEnvPosition = Vector3.zero;
			Vector2 randomEnvPositionVec2 = Vector2.zero;

			float smallestDistance = 0.0f; //will get filled in by CheckObjectsFarEnoughXZ function
			float currentBiggestSmallestDistance = 0; //if we fail at positioning in the allotted number of tries, we want to position the treasure chest with the maximal distance to the closest neighbor chest.
			Vector2 smallestDistancePosition = Vector2.zero;

			while( !objectsAreFarEnough && numTries < maxNumTries){
				// generate a random position
				randomEnvPosition = exp.environmentController.GetRandomPositionWithinWallsXZ( Config_CoinTask.objectToWallBuffer );
				randomEnvPositionVec2 = new Vector2(randomEnvPosition.x, randomEnvPosition.z);

				//increment numTries
				numTries++;

				//check if the generated position is far enough from all other positions
				objectsAreFarEnough = CheckObjectsFarEnoughXZ( randomEnvPositionVec2, defaultPositions, out smallestDistance);

				//if not, and the smallest distance is larger than the currents largest small distance...
				if( !objectsAreFarEnough && smallestDistance > currentBiggestSmallestDistance ){
					currentBiggestSmallestDistance = smallestDistance;
					smallestDistancePosition = randomEnvPositionVec2;
				}
			}

			if(numTries == maxNumTries){
				Debug.Log("Tried " + maxNumTries + " times to place default objects!");
				Debug.Log("DISTANCE: " + currentBiggestSmallestDistance + " POSITION: " + smallestDistancePosition);
				defaultPositions.Add(smallestDistancePosition);
				foilPositions.Add (smallestDistancePosition);
			}
			else{
				defaultPositions.Add(randomEnvPositionVec2);
				foilPositions.Add (randomEnvPositionVec2);
			}

		}

		//		//insertion sort by distance
		//		Vector2 distancePosXZ = new Vector2 (distancePos.x, distancePos.z);
		//		defaultPositions = SortByNextClosest(defaultPositions, distancePosXZ);

		return foilPositions;
	}

	List<Vector2> SortByNextClosest(List<Vector2> positions, Vector2 distancePos){

		List<Vector2> sortedPositions = new List<Vector2>();
		int numPositions = positions.Count;

		Vector2 closestPos = GetClosest (distancePos, positions);
		sortedPositions.Add (closestPos);
		positions.Remove (closestPos);
		for (int i = 1; i < numPositions; i++) {
			closestPos = GetClosest (closestPos, positions);
			sortedPositions.Add (closestPos);
			positions.Remove (closestPos);
		}

		return sortedPositions;
	}

	Vector2 GetClosest(Vector2 position, List<Vector2> otherPositions){
		float minDist = 0.0f;
		int minDistIndex = 0;
		for (int i = 0; i< otherPositions.Count; i++) {
			float dist = UsefulFunctions.GetDistance(position, otherPositions[i]);
			if(i == 0){
				minDist = dist;
				minDistIndex = i;
			}
			else if(dist < minDist){
				minDist = dist;
				minDistIndex = i;
			}
		}

		return otherPositions [minDistIndex];
	}

	//also fills the out float smallestDistance for informing how close the object is to overlap.
	public bool CheckObjectsFarEnoughXZ(Vector2 newObjectPos, List<Vector2> otherObjectPositions, out float smallestDistance){
		bool isFarEnough = true;
		smallestDistance = 0;
		for(int i = 0; i < otherObjectPositions.Count; i++){
			Vector2 previousDefaultObjectLocation = otherObjectPositions[i];
			float positionDistance = (newObjectPos - previousDefaultObjectLocation).magnitude;
			if (i == 0){
				//set smallest distance for the first time
				smallestDistance = positionDistance;
			}
			if( positionDistance < Config_CoinTask.objectToObjectBuffer ){
				if(smallestDistance > positionDistance) {
					smallestDistance = positionDistance;
				}
				isFarEnough = false;
			}
		}
		
		return isFarEnough;
	}

	public List<Vector2> GenerateSpecialObjectPositions (List<Vector2> orderedDefaultObjectLocationsXZ, int numSpecialObjects){
		List<Vector2> specialPositions = new List<Vector2> ();


		//int specialIndex;

		List<Vector2> orderedDefaultPositionsCopy = new List<Vector2> ();
		int numDefault = orderedDefaultObjectLocationsXZ.Count;
		
		//copy the list (ONLY THE POSITIONS WE WANT TO FILL) so we can delete from it...
		for (int i = 0; i < numDefault; i++) {
			Vector2 currPosition = orderedDefaultObjectLocationsXZ[i];
			Vector2 positionCopy = new Vector2(currPosition.x, currPosition.y);
			orderedDefaultPositionsCopy.Add(positionCopy);
		}







		int numDefaultPositions = orderedDefaultPositionsCopy.Count;
		
		//If there are only two special objects, DONT allow the last chest to have an object.
		//This will make the treasure chests less predictable when there are two and three item trials.
//		if(numSpecialObjects == 2){
//			numDefaultPositions -= 1;
//		}
		
		List<int> randomIndices = UsefulFunctions.GetRandomIndexOrder( numDefaultPositions );
		//DON'T allow 1,1,1,0 special object arrangement. For predictability.
		if( numSpecialObjects == 3){
			while(randomIndices[randomIndices.Count - 1] == 3){ //if the last random index (which will be the empty chest) is the last chest, try again. 
				randomIndices = UsefulFunctions.GetRandomIndexOrder( numDefaultPositions );
			}
		}
		
		
		for (int i = 0; i < numSpecialObjects; i++) {

			//if number of special objects exceeds the number of free spots, we'll get stuck.
			//...so exit the loop instead.
			if(i >= numDefault){
				break;
			}

			/*int numDefaultPositions = orderedDefaultPositionsCopy.Count;

			//If there are only two special objects, DONT allow the last chest to have an object.
			//This will make the treasure chests less predictable when there are two and three item trials.
			if(numSpecialObjects == 2){
				numDefaultPositions -= 1;
			}

			List<int> randomIndices = UsefulFunctions.GetRandomIndexOrder( numDefaultPositions );
			//DON'T allow 1,1,1,0 special object arrangement. For predictability.
			if( numSpecialObjects == 3){
				while(randomIndices[randomIndices.Count - 1] == 3){ //if the last random index (which will be the empty chest) is the last chest, try again. 
					randomIndices = UsefulFunctions.GetRandomIndexOrder( numDefaultPositions );
				}
			}
*/
			int randomIndex = randomIndices[i];
			Vector2 currPosition = orderedDefaultPositionsCopy[randomIndex];



			/*if(i != 0){
				for(int j = 1; j < randomIndices.Count; j++){
					randomIndex = randomIndices[j];
					currPosition = orderedDefaultPositionsCopy[randomIndex];
					
					//check against all other special item locations...
					bool isDistanceBigEnough = true;
					for(int k = 0; k < specialPositions.Count; k++){
						//if distance is not big enough...
						float currDistance = (currPosition - specialPositions[k]).magnitude;
						if( currDistance < Config_CoinTask.specialObjectBufferMult*Config_CoinTask.objectToObjectBuffer ) {

							isDistanceBigEnough = false;
							continue;
							
							
						}
					}

					if(isDistanceBigEnough){
						break;
					}
				}

			}*/



			specialPositions.Add (currPosition);
			
			//remove it from the parameter list so that no two special objects are in the same spot.
			//this will not change the original list, as the list was passed by copy.
			//orderedDefaultPositionsCopy.RemoveAt(randomIndex);

		}
		
		return specialPositions;
	}

	public float GetMaxDefaultObjectColliderBoundXZ(){

		InGameDefaultObject.SetActive (true);

		Collider defaultCollider = InGameDefaultObject.GetComponent<Collider> ();
		Vector3 bounds = defaultCollider.bounds.size;
		float maxBound = bounds.x;
		if (bounds.z > bounds.x){
			maxBound = bounds.z;
		}

		InGameDefaultObject.SetActive (false);

		return maxBound;
	}

}
