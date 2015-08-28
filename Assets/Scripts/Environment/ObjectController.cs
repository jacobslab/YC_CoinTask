using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectController : MonoBehaviour {

	//ground plane -- used as spawn reference point
	public GameObject GroundPlane;


	//experiment singleton
	Experiment_CoinTask experiment { get { return Experiment_CoinTask.Instance; } }

	//object array & list
	List<GameObject> gameObjectList_Spawnable;
	//List<GameObject> gameObjectList_Spawned; //a list to keep track of the objects currently in the scene

	//TODO: incorporate into random spawning so that two objects don't spawn in super similar locations consecutively
	Vector3 lastSpawnPos;


	// Use this for initialization
	void Start () {
		gameObjectList_Spawnable = new List<GameObject> ();

		CreateObjectList (gameObjectList_Spawnable);
		lastSpawnPos = experiment.avatar.transform.position;
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


	//for more generic object spawning -- such as in Replay!
	public GameObject SpawnObject( GameObject objToSpawn, Vector3 spawnPos ){
		lastSpawnPos = spawnPos;
		GameObject spawnedObj = Instantiate(objToSpawn, spawnPos, objToSpawn.transform.rotation) as GameObject;

		return spawnedObj;
	}

	//spawn random object at a specified location
	public GameObject SpawnRandomObjectXY (Vector3 spawnPosition){
		GameObject objToSpawn = ChooseRandomObject ();
		if (objToSpawn != null) {
			float spawnPosY = objToSpawn.transform.position.y; //use the prefab's height

			spawnPosition.y = spawnPosY;

			GameObject newObject = Instantiate(objToSpawn, spawnPosition, objToSpawn.transform.rotation) as GameObject;

			lastSpawnPos = newObject.transform.position;

			float randomRot = GenerateRandomRotationY();
			newObject.transform.RotateAround(newObject.transform.position, Vector3.up, randomRot);
			
			return newObject;
		}
		else{
			return null;
		}
	}

	//for spawning a random object at a random location
	public GameObject SpawnRandomObjectXY(){
		GameObject objToSpawn = ChooseRandomObject ();
		if (objToSpawn != null) {

			float spawnPosY = objToSpawn.transform.position.y;
			Vector3 spawnPos = experiment.environmentController.GetRandomPositionWithinWallsXZ( Config_CoinTask.bufferBetweenObjectsAndWall );
			spawnPos = new Vector3(spawnPos.x, spawnPosY, spawnPos.z);

			GameObject newObject = Instantiate(objToSpawn, spawnPos, objToSpawn.transform.rotation) as GameObject;

			lastSpawnPos = newObject.transform.position;

			float randomRot = GenerateRandomRotationY();
			newObject.transform.RotateAround(newObject.transform.position, Vector3.up, randomRot);

			return newObject;
		}
		else{
			return null;
		}
	}

	//NO LONGER USED. TODO: evaluate if it seems useful for other things in the future...
	void MakeObjectFacePlayer(GameObject obj){
		if (obj.transform.position == experiment.avatar.transform.position) { //make sure the object is not directly on top of the avatar.
			Debug.Log("Object is directly on top of the avatar! Cannot face avatar.");
			return;
		}

		//make object face the player
		Vector3 directionToAvatar = experiment.avatar.transform.position - obj.transform.position;
		
		float dotProd = Vector3.Dot(directionToAvatar, obj.transform.forward);
		float theta = Mathf.Acos( dotProd / ( directionToAvatar.magnitude*obj.transform.forward.magnitude ) );
		obj.transform.RotateAround(obj.transform.position, Vector3.up, theta);
		
		if(obj.transform.forward.normalized != directionToAvatar.normalized){
			obj.transform.RotateAround(obj.transform.position, Vector3.up, 180.0f);
		}
	}

}
