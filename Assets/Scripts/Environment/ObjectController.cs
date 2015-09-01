using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectController : MonoBehaviour {

	public GameObject DefaultObject;
	public List<SpawnableObject> CurrentTrialSpecialObjects;


	//experiment singleton
	Experiment_CoinTask experiment { get { return Experiment_CoinTask.Instance; } }

	//object array & list
	List<GameObject> gameObjectList_Spawnable;
	//List<GameObject> gameObjectList_Spawned; //a list to keep track of the objects currently in the scene



	// Use this for initialization
	void Start () {
		gameObjectList_Spawnable = new List<GameObject> ();
		CurrentTrialSpecialObjects = new List<SpawnableObject> ();
		CreateObjectList (gameObjectList_Spawnable);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void CreateObjectList(List<GameObject> gameObjectListToFill){
		gameObjectListToFill.Clear();
		Object[] prefabs = Resources.LoadAll("Prefabs/Objects");
		for (int i = 0; i < prefabs.Length; i++) {
			gameObjectListToFill.Add((GameObject)prefabs[i]);
		}
	}

	void CreateSpawnableList (List<SpawnableObject> spawnableListToFill){
		spawnableListToFill.Clear();
		Object[] prefabs = Resources.LoadAll("Prefabs/Objects");
		for (int i = 0; i < prefabs.Length; i++) {
			SpawnableObject spawnable = ( (GameObject)prefabs[i] ).GetComponent<SpawnableObject>();
			spawnableListToFill.Add(spawnable);
		}
	}

	public GameObject ChooseSpawnableObject(string objectName){
		List<SpawnableObject> allSpawnables = new List<SpawnableObject>(); //note: this is technically getting instantiated twice now... as it's instantiated in CREATE as well.
		CreateSpawnableList (allSpawnables);

		for (int i = 0; i < allSpawnables.Count; i++) {
			if(allSpawnables[i].GetName() == objectName){
				return allSpawnables[i].gameObject;
			}
		}
		return null;
	}


	GameObject ChooseRandomObject(){
		if (gameObjectList_Spawnable.Count == 0) {
			Debug.Log ("No MORE objects to pick! Recreating object list.");
			CreateObjectList(gameObjectList_Spawnable); //IN ORDER TO REFILL THE LIST ONCE ALL OBJECTS HAVE BEEN USED
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

	//for use in trial with configuration setup of the grid
	public void SpawnDefaultObjects(List<Vector2> GridIndices){
		for(int i = 0; i < GridIndices.Count; i++){
			Vector2 currIndices = GridIndices[i];
			Vector3 objPos = experiment.environmentController.myGrid.GetGridPosition( (int)currIndices.x, (int)currIndices.y );
			objPos = new Vector3(objPos.x, DefaultObject.transform.position.y, objPos.z);
			GameObject newObj = Instantiate(DefaultObject, objPos, DefaultObject.transform.rotation) as GameObject;

			GridItem newGridItem = newObj.GetComponent<GridItem>();
			newGridItem.rowIndex = (int)currIndices.x;
			newGridItem.colIndex = (int)currIndices.y;
		}
	}


	//for more generic object spawning -- such as in Replay!
	public GameObject SpawnObject( GameObject objToSpawn, Vector3 spawnPos ){
		GameObject spawnedObj = Instantiate(objToSpawn, spawnPos, objToSpawn.transform.rotation) as GameObject;

		return spawnedObj;
	}

	//spawn random object at a specified location
	public GameObject SpawnSpecialObjectXY (Vector2 gridIndices, Vector3 spawnPosition){
		GameObject objToSpawn = ChooseRandomObject ();
		if (objToSpawn != null) {
			float spawnPosY = objToSpawn.transform.position.y; //use the prefab's height

			spawnPosition.y = spawnPosY;

			GameObject newObject = Instantiate(objToSpawn, spawnPosition, objToSpawn.transform.rotation) as GameObject;

			float randomRot = GenerateRandomRotationY();
			newObject.transform.RotateAround(newObject.transform.position, Vector3.up, randomRot);

			CurrentTrialSpecialObjects.Add(newObject.GetComponent<SpawnableObject>());

			newObject.GetComponent<GridItem>().rowIndex = (int)gridIndices.x;
			newObject.GetComponent<GridItem>().colIndex = (int)gridIndices.y;

			return newObject;
		}
		else{
			return null;
		}
	}

	//for spawning a random object at a random location
	/*public GameObject SpawnRandomObjectXY(){
		GameObject objToSpawn = ChooseRandomObject ();
		if (objToSpawn != null) {

			float spawnPosY = objToSpawn.transform.position.y;
			Vector3 spawnPos = experiment.environmentController.GetRandomPositionWithinWallsXZ( Config_CoinTask.bufferBetweenObjectsAndWall );
			spawnPos = new Vector3(spawnPos.x, spawnPosY, spawnPos.z);

			GameObject newObject = Instantiate(objToSpawn, spawnPos, objToSpawn.transform.rotation) as GameObject;

			float randomRot = GenerateRandomRotationY();
			newObject.transform.RotateAround(newObject.transform.position, Vector3.up, randomRot);

			return newObject;
		}
		else{
			return null;
		}
	}*/

}
