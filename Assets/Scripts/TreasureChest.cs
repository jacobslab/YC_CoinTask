using UnityEngine;
using System.Collections;

public class TreasureChest : MonoBehaviour {

	public Transform pivotA;
	public Transform pivotB;
	public Transform top;
	public Transform treasureSpawnPoint;


	float angleToOpen = 150.0f; //degrees

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public IEnumerator Open(GameObject opener){
		float distOpenerToPivotA = (pivotA.position - opener.transform.position).magnitude;
		float distOpenerToPivotB = (pivotB.position - opener.transform.position).magnitude;

		Vector3 pivotPos = transform.position;
		string closePivotName = ""; //actually want to use the closer pivot as our opener reference for Logging
		if (distOpenerToPivotA > distOpenerToPivotB) { //use the further away pivot
			pivotPos = pivotA.position;
			closePivotName = pivotB.name;
		} 
		else {
			pivotPos = pivotB.position;
			closePivotName = pivotA.name;
			angleToOpen = -angleToOpen;
		}

		GetComponent<TreasureChestLogTrack> ().LogOpening (closePivotName); 

		Quaternion origRotation = top.rotation;
		top.RotateAround(pivotPos, -transform.right, angleToOpen); //rotate to get the desired rotation
		Quaternion desiredRotation = top.transform.rotation;

		yield return 0;
	}

}
