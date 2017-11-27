using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpawnInsideCircle : MonoBehaviour {
	SphereCollider spawnTrigger;
	public GameObject spawnObj;
	// Use this for initialization
	void Start () {
		spawnTrigger = GetComponent<SphereCollider> ();
		for (int i = 0; i < 2250; i++) {
			Vector3 randSphere = Random.insideUnitSphere;
			Vector3 spawnPosition = new Vector3 (spawnTrigger.transform.position.x + (randSphere.x * spawnTrigger.radius), 
				spawnTrigger.transform.position.y, spawnTrigger.transform.position.z + ( randSphere.z * spawnTrigger.radius));
			GameObject spawn= Instantiate (spawnObj, spawnPosition, Quaternion.identity) as GameObject;
			spawn.name = randSphere.ToString ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
