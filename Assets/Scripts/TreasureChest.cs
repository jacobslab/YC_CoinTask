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
		if (distOpenerToPivotA > distOpenerToPivotB) { //use the further away pivot
			pivotPos = pivotA.position;
			Debug.Log("using pivot A");
		} 
		else {
			pivotPos = pivotB.position;
			angleToOpen = -angleToOpen;
			Debug.Log("using pivot B");
		}


		Quaternion origRotation = top.rotation;
		top.RotateAround(pivotPos, -transform.right, angleToOpen); //rotate to get the desired rotation
		Quaternion desiredRotation = top.transform.rotation;

		//TODO: nicely animate the opening...
		/*top.rotation = origRotation; //rotate back to original

		//TAKEN FROM PLAYER CONTROLS
		float ELAPSEDTIME = 0.0f;

		float rotateRate = 1.0f / Config_CoinTask.rotateToSpecialObjectTime;
		float tElapsed = 0.0f;
		float rotationEpsilon = 0.01f;

		while (Mathf.Abs(top.rotation.eulerAngles.y - desiredRotation.eulerAngles.y) >= rotationEpsilon){

			
			tElapsed += (Time.deltaTime * rotateRate);
			ELAPSEDTIME += Time.deltaTime;
			//will spherically interpolate the rotation for config.spinTime seconds
			//top.rotation = Quaternion.Slerp(origRotation, desiredRotation, tElapsed); //SLERP ALWAYS TAKES THE SHORTEST PATH.
			//top.RotateAround(pivotPos, Vector3.forward, rotateRate*Time.deltaTime);

			yield return 0;
		}
		
		
		
		top.rotation = desiredRotation;*/

		yield return 0;
	}

}
