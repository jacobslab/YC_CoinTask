using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectController : MonoBehaviour {


	//instantiated somewhere (hidden) in the scene at all times. for calculating default object bounds --> requires an active object, and we don't want to instantiate one every time we ask for the bounds.
	//this object does *not* need to be logged.
	//it should also have a special name in the scene.
	public GameObject InGameDefaultObject;

	public GameObject DefaultObject; //the prefab used to instantiate the other default objects.
	public GameObject ExplosiveObject;
	public List<GameObject> CurrentTrialSpecialObjects;


	//experiment singleton
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	//object array & list
	List<GameObject> gameObjectList_Spawnable;
	//List<GameObject> gameObjectList_Spawned; //a list to keep track of the objects currently in the scene



	// Use this for initialization
	void Start () {
		gameObjectList_Spawnable = new List<GameObject> ();
		CurrentTrialSpecialObjects = new List<GameObject> ();
		CreateSpecialObjectList (gameObjectList_Spawnable);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void CreateSpecialObjectList(List<GameObject> gameObjectListToFill){
		gameObjectListToFill.Clear();
		Object[] prefabs;
		#if MRIVERSION
		if(Config_CoinTask.isPractice){
			prefabs = Resources.LoadAll("Prefabs/MRIPracticeObjects");
		}
		else{
			prefabs = Resources.LoadAll("Prefabs/Objects");
		}
		#else
			prefabs = Resources.LoadAll("Prefabs/Objects");
		#endif
		for (int i = 0; i < prefabs.Length; i++) {
			gameObjectListToFill.Add((GameObject)prefabs[i]);
		}
	}

	//used in replay
	void CreateCompleteSpawnableList (List<SpawnableObject> spawnableListToFill){
		spawnableListToFill.Clear();
		Object[] specialPrefabs = Resources.LoadAll("Prefabs/Objects");
		Object[] otherPrefabs = Resources.LoadAll("Prefabs/OtherSpawnables");
        Object[] mriPrefabs = Resources.LoadAll("Prefabs/MRIPracticeObjects");

        for (int i = 0; i < specialPrefabs.Length; i++) {
			SpawnableObject spawnable = ( (GameObject)specialPrefabs[i] ).GetComponent<SpawnableObject>();
			spawnableListToFill.Add(spawnable);
		}
		for (int i = 0; i < otherPrefabs.Length; i++) {
			SpawnableObject spawnable = ( (GameObject)otherPrefabs[i] ).GetComponent<SpawnableObject>();
			spawnableListToFill.Add(spawnable);
		}

        for (int i = 0; i < mriPrefabs.Length; i++)
        {
            SpawnableObject spawnable = ((GameObject)mriPrefabs[i]).GetComponent<SpawnableObject>();
            spawnableListToFill.Add(spawnable);
        }
    }

	//used in replay
	public GameObject ChooseSpawnableObject(string objectName){
		List<SpawnableObject> allSpawnables = new List<SpawnableObject>(); //note: this is technically getting instantiated twice now... as it's instantiated in CREATE as well.
		CreateCompleteSpawnableList (allSpawnables);

		for (int i = 0; i < allSpawnables.Count; i++) {

            Debug.Log("all spawnable " + allSpawnables[i].ToString());
            Debug.Log("all spawnable " + allSpawnables[i].GetName());
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
		if (gameObjectList_Spawnable.Count == 0) {
			Debug.Log ("No MORE objects to pick! Recreating object list.");
			CreateSpecialObjectList(gameObjectList_Spawnable); //IN ORDER TO REFILL THE LIST ONCE ALL OBJECTS HAVE BEEN USED
			if(gameObjectList_Spawnable.Count == 0){
				Debug.Log ("No objects to pick at all!"); //if there are still no objects in the list, then there weren't any to begin with...
				return null;
			}
		}


		int randomObjectIndex = Random.Range(0, gameObjectList_Spawnable.Count);
		GameObject chosenObject = gameObjectList_Spawnable[randomObjectIndex];
		gameObjectList_Spawnable.RemoveAt(randomObjectIndex);
		
		return chosenObject;
	}


	public float GenerateRandomRotationY(){
		float randomRotY = Random.Range (0, 360);
		return randomRotY;
	}

	//special positions get passed in so that the default object can get a special tag for later use for spawning special objects
	public void SpawnDefaultObjects(List<Vector2> defaultPositions, List<Vector2> specialPositions){
		for(int i = 0; i < defaultPositions.Count; i++){
			Vector2 currPos = defaultPositions[i];
			SpawnDefaultObject( currPos, specialPositions, i );
		}
	}

	//special positions get passed in so that the default object can get a special tag for later use for spawning special objects
	public GameObject SpawnDefaultObject (Vector2 positionXZ, List<Vector2> specialPositions, int index) {
		Vector3 objPos = new Vector3(positionXZ.x, DefaultObject.transform.position.y, positionXZ.y);
		GameObject newObj = Instantiate(DefaultObject, objPos, DefaultObject.transform.rotation) as GameObject;
        Debug.Log("new obj is " + newObj.gameObject.name);
        SpawnableObject newSpawnableObj;
        //SpawnableObject newSpawnableObj = newObj.GetComponent<SpawnableObject>(); 
        //if (newObj.gameObject.name.Contains("Pedestal"))
        //{
        //    newSpawnableObj = newObj.transform.GetChild(0).gameObject.GetComponent<SpawnableObject>(); //we get the child as it is on a pedestal
        //}
        //else
        //{
            newSpawnableObj = newObj.GetComponent<SpawnableObject>();
        //}
		newSpawnableObj.SetNameID(newObj.transform, index);
		
		if( specialPositions.Contains(positionXZ) ){
			newObj.tag = "DefaultSpecialObject";
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
            Debug.Log("halving the local scale of spawned obj");
            newObject.transform.localScale = new Vector3(newObject.transform.localScale.x / 2f, newObject.transform.localScale.y / 2f, newObject.transform.localScale.z / 2f);
			float randomRot = GenerateRandomRotationY();
			newObject.transform.RotateAround(newObject.transform.position, Vector3.up, randomRot);

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
				Debug.Log("Tried " + maxNumTries + " times to place default objects!");
				Debug.Log("DISTANCE: " + currentBiggestSmallestDistance + " POSITION: " + smallestDistancePosition);
				defaultPositions.Add(smallestDistancePosition);
			}
			else{
				defaultPositions.Add(randomEnvPositionVec2);
			}

		}

		//insertion sort by distance
		Vector2 distancePosXZ = new Vector2 (distancePos.x, distancePos.z);
		defaultPositions = SortByNextClosest(defaultPositions, distancePosXZ);

		return defaultPositions;
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

		//InGameDefaultObject.SetActive (true);

		Collider defaultCollider = InGameDefaultObject.GetComponent<Collider> ();
		Vector3 bounds = defaultCollider.bounds.size;
		float maxBound = bounds.x;
		if (bounds.z > bounds.x){
			maxBound = bounds.z;
		}

	//	InGameDefaultObject.SetActive (false);

		return maxBound;
	}

}
