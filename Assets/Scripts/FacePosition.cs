using UnityEngine;
using System.Collections;

public class FacePosition : MonoBehaviour {

	//if false, we will face *away* from the player
	//public bool ShouldFaceTowardsPosition;
	public Transform TargetPositionTransform;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		FaceThePosition ();
	}

	void FaceThePosition(){
		transform.LookAt (TargetPositionTransform);
	}
}
